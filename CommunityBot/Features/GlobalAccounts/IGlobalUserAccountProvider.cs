using CommunityBot.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityBot.Features.GlobalAccounts
{
    public interface IGlobalUserAccountProvider
    {
        GlobalUserAccount GetById(ulong userId);
        void SaveByIds(params ulong[] userId);
    }
}
