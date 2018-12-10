using System;
using System.Collections.Generic;
using System.Text;
using CommunityBot.Entities;

namespace CommunityBot.Features.GlobalAccounts
{
    public class GlobalUserAccountProvider : IGlobalUserAccountProvider
    {
        // NOTE(Peter): TODO: This provider at this point serves only to interface the static
        // GlobalUserAccounts class, however, further down the line, it would be ideal
        // to have a non-static implementation (either in this class, or a new one
        // inheriting from IGlobaUserAccountsProvider.

        // I hid the static class behind this interface/class pair to be able to mock it
        // inside unit tests.

        public GlobalUserAccount GetById(ulong userId)
        {
            return GlobalUserAccounts.GetUserAccount(userId);
        }

        public void SaveByIds(params ulong[] userIds)
        {
            GlobalUserAccounts.SaveAccounts(userIds);
        }
    }
}
