using System;
using CommunityBot.DiscordAbstractions;
using CommunityBot.Features.GlobalAccounts;

namespace CommunityBot.Features.Economy
{
    public class Transfer : IMiuniesTransfer
    {
        private readonly IGlobalUserAccountProvider globalUserAccountProvider;
        private readonly IDiscordSocketClient discordClient;

        public Transfer(IGlobalUserAccountProvider globalUserAccountProvider, IDiscordSocketClient discordClient)
        {
            this.globalUserAccountProvider = globalUserAccountProvider;
            this.discordClient = discordClient;
        }

        public void UserToUser(ulong sourceUserId, ulong targetUserId, ulong amount)
        {
            if (sourceUserId == targetUserId) { throw new InvalidOperationException(Constants.ExTransferSameUser); }
            
            if (targetUserId == discordClient.GetCurrentUser().Id) { throw new InvalidOperationException(Constants.ExTransferToMiunie); }

            var transferSource = globalUserAccountProvider.GetById(sourceUserId);

            if (transferSource.Miunies < amount) { throw new InvalidOperationException(Constants.ExTransferNotEnoughFunds); }

            var transferTarget = globalUserAccountProvider.GetById(targetUserId);

            transferSource.Miunies -= amount;
            transferTarget.Miunies += amount;

            globalUserAccountProvider.SaveByIds(transferSource.Id, transferTarget.Id);
        }
    }
}
