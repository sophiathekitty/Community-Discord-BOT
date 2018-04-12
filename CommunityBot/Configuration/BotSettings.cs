using CommunityBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityBot.Helpers;
using Newtonsoft.Json;

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

        public static ActionResult SetCommandPrefix(string prefix)
        {
            var result = new ActionResult();
            
            config.Prefix = prefix;

            var saveSettingsResult = SaveSettings();

            result.Merge(saveSettingsResult);

            return result;
        }

        private static ActionResult SaveSettings()
        {
            var result = new ActionResult();
            
            try
            {
                DataStorage.StoreObject(config, configFile, useIndentations: true);
            }
            catch (Exception)
            {
                result.AddAlert(new Alert("Settings error", "Could not save the Settings", LevelEnum.Exception));
            }

            return result;
        }
    }
}
