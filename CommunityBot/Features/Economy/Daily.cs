using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityBot.Entities;
using CommunityBot.Features.GlobalAccounts;

namespace CommunityBot.Features.Economy
{
    public static class Daily
    {
        public enum DailyResult { Success, AlreadyRecieved }

        public static DailyResult GetDaily(ulong userId)
        {
            var account = GlobalUserAccounts.GetUserAccount(userId);
            var difference = DateTime.Now - account.LastDaily;

            if (difference.TotalHours < 24) return DailyResult.AlreadyRecieved;

            account.Miunies += Constants.DailyMuiniesGain;
            account.LastDaily = DateTime.UtcNow;
            GlobalUserAccounts.SaveAccounts(userId);
            return DailyResult.Success;
        }
    }
}
