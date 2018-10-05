using CommunityBot.Entities;
using System;
using CommunityBot.Helpers;

namespace CommunityBot.Configuration
{
    internal static class BotSettings
    {
        internal static BotConfig config;

        private static readonly string configFile = "config.json";

        static BotSettings()
        {
            LoadConfig();
        }

        internal static void LoadConfig()
        {
            var dataStorage = InversionOfControl.Container.GetInstance<JsonDataStorage>();
            if (dataStorage.LocalFileExists(configFile))
            {
                config = dataStorage.RestoreObject<BotConfig>(configFile);
            }
            else
            {
                // Setting up defaults
                config = new BotConfig()
                {
                    Prefix = "$",
                    Token = "YOUR-TOKEN-HERE"
                };
                dataStorage.StoreObject(config, configFile, useIndentations: true);
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
                InversionOfControl.Container.GetInstance<JsonDataStorage>().StoreObject(config, configFile);
            }
            catch (Exception)
            {
                result.AddAlert(new Alert("Settings error", "Could not save the Settings", LevelEnum.Exception));
            }

            return result;
        }
    }
}
