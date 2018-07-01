using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;

namespace CommunityBot.ConfigServerAccount
{
    public static class ServerAccounts
    {
         private static readonly List<ServerSettings> ServerAccountsList;

        private const string ServerAccountsFile = @"ServerAccounts.json";

        static ServerAccounts()
        {
            if (ServerDataStorage.SaveExists(ServerAccountsFile))
                ServerAccountsList = ServerDataStorage.LoadServerSettings(ServerAccountsFile).ToList();
            else
            {
                ServerAccountsList = new List<ServerSettings>();
                SaveServerAccounts();
            }
        }

        public static void SaveServerAccounts()
        {
            ServerDataStorage.SaveServerSettings(ServerAccountsList, ServerAccountsFile);
        }

        public static ServerSettings GetServerAccount(SocketGuild guild)
        {
            return GetOrCreateServerAccount(guild.Id, guild.Name);
        }

        public static ServerSettings GetServerAccount(IGuildChannel guild)
        {
            return GetOrCreateServerAccount(guild.Guild.Id, guild.Guild.Name);
        }

        private static ServerSettings GetOrCreateServerAccount(ulong id, string name)
        {
            var result = from a in ServerAccountsList
                         where a.ServerId == id
                         select a;
            var account = result.FirstOrDefault() ?? CreateServerAccount(id, name);

            return account;
        }

       


        internal static List<ServerSettings> GetAllServerAccounts()
        {
            return ServerAccountsList.ToList();
        }

            internal static List<ServerSettings> GetFilteredServerAccounts(Func<ServerSettings, bool> filter)
            {
               
                return ServerAccountsList.Where(filter).ToList();
            }


        private static ServerSettings CreateServerAccount(ulong id, string name)
        {
            var newAccount = new ServerSettings
            {
                ServerName = name,
                ServerId = id,
                Prefix = "*",
                ServerActivityLog = 0,
                Language = "en"
            };

            ServerAccountsList.Add(newAccount);
            SaveServerAccounts();
            return newAccount;
        }
    }
}
