using System;
using System.Collections.Generic;
using CommunityBot.Features.GlobalAccounts;

namespace CommunityBot.Features.Economy
{
    public class Daily : IDailyMiunies
    {
        private readonly IGlobalUserAccountProvider globalUserAccountProvider;

        public Daily(IGlobalUserAccountProvider globalUserAccountProvider)
        {
            this.globalUserAccountProvider = globalUserAccountProvider;
        }
        
        public void GetDaily(ulong userId)
        {
            var account = globalUserAccountProvider.GetById(userId);
            var sinceLastDaily = DateTime.UtcNow - account.LastDaily;

            if (sinceLastDaily.TotalHours < 24)
            {
                var e = new InvalidOperationException(Constants.ExDailyTooSoon);
                e.Data.Add("sinceLastDaily", sinceLastDaily);
                throw e;
            }

            account.Miunies += Constants.DailyMuiniesGain;
            account.LastDaily = DateTime.UtcNow;

            globalUserAccountProvider.SaveByIds(userId);
        }
    }
}
