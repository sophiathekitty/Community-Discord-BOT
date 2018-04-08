using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityBot.Features.Economy;
using CommunityBot.Features.RepeatedTasks;
using Discord.WebSocket;

namespace CommunityBot
{
    internal static class Global
    {
        internal static DiscordSocketClient Client { get; set; }
        internal static Dictionary<ulong, string> MessagesIdToTrack { get; set; }
        internal static Random Rng { get; set; } = new Random();
        internal static Slot slot = new Slot();
        internal static RepeatedTaskHandler TaskHander = new RepeatedTaskHandler();
        // Global Helper methods

        internal static string GetRandomDidYouKnow()
        {
            return Constants.DidYouKnows[Rng.Next(0, Constants.DidYouKnows.Length)];
        }
    }
}
