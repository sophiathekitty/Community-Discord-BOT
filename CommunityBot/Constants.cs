using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityBot
{
    public static class Constants
    {
        internal static readonly string ResourceFolder = "resources";
        internal static readonly string UserAccountsFolder = "users";
        public const ulong DailyMuiniesGain = 250;
        public const int MessageRewardCooldown = 30;
        public const int MessageRewardMinLenght = 20;
        public static readonly Tuple<int, int> MessagRewardMinMax = Tuple.Create(1, 5);
        
        public static readonly string[] DidYouKnows = {
            "You can fork me on GitHub ;) xoxo <3",
            "If you don't know what to add, you can add some of my messages. :P",
            "Wanna see someone's Miunies? Add a mention to your cash command.",
            "I just love when a programmer PULL requests their code into me.",
            "Protection? I don't accept code just from anybody, alright?",
            "You get a couple Miunies for sending messages (with a short cooldown)."
        };
    }
}
