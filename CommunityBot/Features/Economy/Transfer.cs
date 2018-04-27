using System;
using System.Collections.Generic;
using System.Text;
using CommunityBot.Features.GlobalAccounts;
using Discord;

namespace CommunityBot.Features.Economy
{
    internal static class Transfer
    {
        internal enum TransferResult { Success, SelfTransfer, NotEnoughMiunies }

        internal static TransferResult ToUser(IUser from, IUser to, ulong amount)
        {
            if (from.Id == to.Id) return TransferResult.SelfTransfer;

            var transferSource = GlobalUserAccounts.GetUserAccount(from.Id);

            if (transferSource.Miunies < amount) return TransferResult.NotEnoughMiunies;

            var transferTarget = GlobalUserAccounts.GetUserAccount(from.Id);

            transferSource.Miunies -= amount;
            transferTarget.Miunies += amount;

            GlobalUserAccounts.SaveAccounts(transferSource.Id, transferTarget.Id);

            return TransferResult.Success;
        }
    }
}
