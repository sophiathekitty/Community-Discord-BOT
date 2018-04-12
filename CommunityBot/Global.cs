using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityBot.Features.Economy;
using Discord.Commands;
using CommunityBot.Features.RepeatedTasks;
using Discord.WebSocket;

namespace CommunityBot
{
    public static class Global
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
        
        internal static string ReplacePlacehoderStrings(string messageString, SocketGuildUser user)
        {
            return messageString.Replace("<username>", user.Nickname ?? user.Username)
                .Replace("<usermention>", user.Mention)
                .Replace("<guildname>", user.Guild.Name)
                .Replace("<botmention>", Client.CurrentUser.Mention)
                .Replace("<botdiscriminator>", Client.CurrentUser.Discriminator)
                .Replace("<botname>", Client.CurrentUser.Username);
        }

        public static string GetMiuniesCountReaction(ulong value, string mention)
        {
            if (value > 100000)
            {
                return $"Holy shit, {mention}! You're either cheating or you're really dedicated.";
            }
            if (value > 50000)
            {
                return $"Damn, you must be here often, {mention}. Do you have a crush on me or something?";
            }
            if (value > 20000)
            {
                return $"That's enough to buy a house... In Miunie land... \n\nIt's a real place, shut up, {mention}!";
            }
            if (value > 10000)
            {
                return $"{mention} is kinda getting rich. Do we rob them or what?";
            }
            if (value > 5000)
            {
                return $"Is it just me or is {mention} taking this economy a little too seriously?";
            }
            if (value > 2500)
            {
                return $"Great, {mention}! Now you can give all those miunies to your superior mistress, ME.";
            }
            if (value > 1100)
            {
                return $"{mention} is showing their wealth on the internet again.";
            }
            if (value > 800)
            {
                return $"Alright, {mention}. Put the miunies in the back and nobody gets hurt.";
            }
            if (value > 550)
            {
                return $"I like how {mention} think that's impressive.";
            }
            if (value > 200)
            {
                return $"Outch, {mention}! If I knew that is all you've got, I would just DM you the amount. Embarrassing!";
            }
            if (value == 0)
            {
                return $"Yea, {mention} is broke. What a surprise.";
            }

            return "The whole concept of miunies is fake. I hope you know that";
        }
    }
}
