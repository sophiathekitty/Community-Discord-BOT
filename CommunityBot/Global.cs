using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityBot.Features.Economy;
using Discord.Commands;
using Discord.WebSocket;

namespace CommunityBot
{
    internal static class Global
    {
        internal static DiscordSocketClient Client { get; set; }
        internal static Dictionary<ulong, string> MessagesIdToTrack { get; set; }
        internal static Random Rng { get; set; } = new Random();


        internal static Slot slot = new Slot();
        // Global Helper methods

        internal static string GetRandomDidYouKnow()
        {
            return Constants.DidYouKnows[Rng.Next(0, Constants.DidYouKnows.Length)];
        }
        
        internal static string ReplacePlacehoderStrings(string messageString, SocketGuildUser user)
        {
            return messageString.Replace("<username>", user.Username)
                .Replace("<usermention>", user.Mention)
                .Replace("<guildname>", user.Guild.Name)
                .Replace("<botmention>", Client.CurrentUser.Mention)
                .Replace("<botdiscriminator>", Client.CurrentUser.Discriminator)
                .Replace("<botname>", Client.CurrentUser.Username);
        }
    }
}
