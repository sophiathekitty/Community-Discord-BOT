using System;
using CommunityBot.DiscordAbstractions;
using CommunityBot.Entities;
using CommunityBot.Features.Economy;
using CommunityBot.Features.GlobalAccounts;
using Discord;
using Moq;
using NUnit.Framework;

namespace CommunityBot.NUnit.Tests.FeatureTests.Economy
{
    public class TransferTests
    {
        // NOTE(Peter): These tests should probably be refactored.

        [Test]
        public void TransferToSameUserThrows()
        {
            const ulong userId = 999777111;
            var globalUserAccountProviderMock = new Mock<IGlobalUserAccountProvider>();
            var discordClientMock = new Mock<IDiscordSocketClient>();
            var miuniesTransfer = new Transfer(globalUserAccountProviderMock.Object, discordClientMock.Object);

            var exception = Assert.Throws<InvalidOperationException>(() => 
                miuniesTransfer.UserToUser(userId, userId, 50));
            Assert.AreEqual(Constants.ExTransferSameUser, exception.Message);
        }

        [Test]
        public void TransferToThisBotThrows()
        {
            const ulong userId = 555987;
            const ulong thisBotId = 123;
            var discordClientMock = GetDiscordSocketClientWithSelfUser(thisBotId);
            var globalUserAccountProviderMock = new Mock<IGlobalUserAccountProvider>();
            var miuniesTransfer = new Transfer(globalUserAccountProviderMock.Object, discordClientMock.Object);

            var exception = Assert.Throws<InvalidOperationException>(() => 
                miuniesTransfer.UserToUser(userId, thisBotId, 50));
            Assert.AreEqual(Constants.ExTransferToMiunie, exception.Message);
        }

        [Test]
        public void TransferMiuniesUserDoesNotHaveThrows()
        {
            const ulong userId = 3358;
            const ulong targetUserId = 9000;
            const ulong maxMiunies = 500;
            var discordClientMock = GetDiscordSocketClientWithSelfUser(100);
            var globalUserAccountProviderMock = new Mock<IGlobalUserAccountProvider>();
            globalUserAccountProviderMock
                .Setup(m => m.GetById(userId))
                .Returns(new GlobalUserAccount(userId) {Miunies = maxMiunies});
            var miuniesTransfer = new Transfer(globalUserAccountProviderMock.Object, discordClientMock.Object);

            var exception = Assert.Throws<InvalidOperationException>(() =>
                miuniesTransfer.UserToUser(userId, targetUserId, maxMiunies + 10));
            Assert.AreEqual(Constants.ExTransferNotEnoughFunds, exception.Message);
        }

        [Test]
        public void TransferMiuniesValidTransferFunctions()
        {
            const ulong userId = 6658;
            const ulong targetUserId = 7785;
            const ulong sourceMiunies = 500;
            const ulong targetMiunies = 255;
            const ulong transferAmount = 100;
            var sourceUser = new GlobalUserAccount(userId)
            {
                Miunies = sourceMiunies
            };
            var targetUser = new GlobalUserAccount(targetUserId)
            {
                Miunies = targetMiunies
            };
            var discordClientMock = GetDiscordSocketClientWithSelfUser(2);
            var globalUserAccountProviderMock = new Mock<IGlobalUserAccountProvider>();
            globalUserAccountProviderMock
                .Setup(m => m.GetById(userId))
                .Returns(sourceUser);
            globalUserAccountProviderMock
                .Setup(m => m.GetById(targetUserId))
                .Returns(targetUser);
            var miuniesTransfer = new Transfer(globalUserAccountProviderMock.Object, discordClientMock.Object);

            miuniesTransfer.UserToUser(userId, targetUserId, transferAmount);
            
            Assert.AreEqual(sourceMiunies - transferAmount, sourceUser.Miunies);
            Assert.AreEqual(targetMiunies + transferAmount, targetUser.Miunies);
            globalUserAccountProviderMock.Verify(m => m.SaveByIds(userId, targetUserId), Times.Once);
        }

        private static Mock<ISelfUser> GetSelfUserMock(ulong id)
        {
            var thisBotMock = new Mock<ISelfUser>();
            thisBotMock.Setup(m => m.Id).Returns(id);
            return thisBotMock;
        }

        private static Mock<IDiscordSocketClient> GetDiscordSocketClientWithSelfUser(ulong id)
        {
            var self = GetSelfUserMock(id);
            var discordClientMock = new Mock<IDiscordSocketClient>();
            discordClientMock.Setup(m => m.GetCurrentUser()).Returns(self.Object);
            return discordClientMock;
        }
    }
}