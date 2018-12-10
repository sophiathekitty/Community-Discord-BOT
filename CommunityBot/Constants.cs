using System;
using System.Collections.Generic;

namespace CommunityBot
{
    public static class Constants
    {
        internal static readonly string ResourceFolder = "resources";
        internal static readonly string UserAccountsFolder = "users";
        internal static readonly string ServerAccountsFolder = "servers";
        internal static readonly string LogFolder = "logs";
        internal static readonly string InvisibleString = "\u200b";
        public const ulong DailyMuiniesGain = 250;
        public const int MessageRewardCooldown = 30;
        public const int MessageRewardMinLenght = 20;
        public const int MaxMessageLength = 2000;
        public static readonly Tuple<int, int> MessagRewardMinMax = Tuple.Create(1, 5);
        public static readonly int MinTimerIntervall = 3000;
        public const int MaxCommandHistoryCapacity = 5;
        public static readonly IList<string> DidYouKnows = new List<string> {
            "You can fork me on GitHub ;) xoxo <3",
            "If you don't know what to add, you can add some of my messages. :P",
            "Wanna see someone's Miunies? Add a mention to your cash command.",
            "I just love when a programmer PULL requests their code into me.",
            "Protection? I don't accept code just from anybody, alright?",
            "You get a couple Miunies for sending messages (with a short cooldown).", 
            "A lot of commands have shorter and easier to use aliases!"
        }.AsReadOnly();

        // Exception messages
        public static readonly string ExDailyTooSoon = "Cannot give daily sooner than 24 hours after the last one.";
    }
}
