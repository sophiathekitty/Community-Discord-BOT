using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace CommunityBot
{
    internal static class Global
    {
        internal static DiscordSocketClient Client { get; set; }
        internal static Random Rng { get; set; } = new Random();

        // Global Helper methods

        internal static string GetRandomDidYouKnow()
        {
            return Constants.DidYouKnows[Rng.Next(0, Constants.DidYouKnows.Length)];
        }
    }
}
