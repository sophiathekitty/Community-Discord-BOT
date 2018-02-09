using CommunityBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityBot.Configuration
{
    internal static class BotSettings
    {
        internal static BotConfig config;

        private static readonly string configFile = "config.json";

        static BotSettings()
        {
            if(DataStorage.LocalFileExists(configFile))
            {
                config = DataStorage.RestoreObject<BotConfig>(configFile);
            }
            else
            {
                // Setting up defaults
                config = new BotConfig()
                {
                    Prefix = "$",
                    Token = "YOUR-TOKEN-HERE"
                };
                DataStorage.StoreObject(config, configFile, useIndentations: true);
            }
        }
    }
}
