using CommunityBot.Configuration;
using CommunityBot.Entities;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityBot.Features.GlobalAccounts
{
    internal static class GlobalUserAccounts
    {
        private static readonly string saveFile = "users.json";
        private static readonly List<GlobalUserAccount> accounts;

        static GlobalUserAccounts()
        {
            if(DataStorage.LocalFileExists(saveFile))
            {
                accounts = DataStorage.RestoreObject<List<GlobalUserAccount>>(saveFile);
            }
            else
            {
                accounts = new List<GlobalUserAccount>();
                DataStorage.StoreObject(accounts, saveFile, useIndentations: false);
            }
        }

        internal static GlobalUserAccount GetUserAccount(ulong id)
        {
            var foundAccount = accounts.FirstOrDefault(a => a.Id == id);

            if(foundAccount == null)
            {
                foundAccount = new GlobalUserAccount
                {
                    Id = id
                };
                accounts.Add(foundAccount);
                SaveAccounts();
            }

            return foundAccount;
        }

        internal static GlobalUserAccount GetUserAccount(IUser user)
        {
            return GetUserAccount(user.Id);
        }

        internal static void SaveAccounts()
        {
            DataStorage.StoreObject(accounts, saveFile, useIndentations: false);
        }
    }
}
