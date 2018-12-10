using CommunityBot.Features.Economy;
using NUnit.Framework;
using Moq;
using CommunityBot.Features.GlobalAccounts;
using CommunityBot.Entities;
using System;

namespace CommunityBot.NUnit.Tests.FeatureTests.Economy
{
    public class DailyTests
    {
        [Test]
        public void UserGetsMiuniesAfterADay()
        {
            const ulong userId = 123456789;
            const ulong miuniesBefore = 100;
            const ulong miuniesExpected = miuniesBefore + Constants.DailyMuiniesGain;
            var testUser = new GlobalUserAccount(userId)
            {
                LastDaily = DateTime.Now.AddDays(-2),
                Miunies = miuniesBefore
            };

            var globalUserAccountsMock = new Mock<IGlobalUserAccountProvider>();
            globalUserAccountsMock.Setup(a => a.GetById(userId)).Returns(testUser);

            IDailyMiunies dailyService = new Daily(globalUserAccountsMock.Object);

            dailyService.GetDaily(userId);

            Assert.AreEqual(testUser.Miunies, miuniesExpected);
        }

        [Test]
        public void ThrowsWhenAskedForDailyWithinADay()
        {
            const ulong userId = 987654321;
            const int expectedHours = 7;
            var testUser = new GlobalUserAccount(userId)
            {
                LastDaily = DateTime.UtcNow.AddHours(-7)
            };

            var globalUserAccountsMock = new Mock<IGlobalUserAccountProvider>();
            globalUserAccountsMock.Setup(a => a.GetById(userId)).Returns(testUser);

            IDailyMiunies dailyService = new Daily(globalUserAccountsMock.Object);

            var exception = Assert.Throws<InvalidOperationException>(() => dailyService.GetDaily(userId));
            Assert.AreEqual(exception.Message, Constants.ExDailyTooSoon);
            var sinceLastDaily = (TimeSpan)exception.Data["sinceLastDaily"];
            Assert.AreEqual(expectedHours, (int)sinceLastDaily.TotalHours);
        }
    }
}
