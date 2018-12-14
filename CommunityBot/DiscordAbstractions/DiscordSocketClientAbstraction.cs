using Discord;
using Discord.WebSocket;

namespace CommunityBot.DiscordAbstractions
{
    public class DiscordSocketClientAbstraction : IDiscordSocketClient
    {
        private readonly DiscordSocketClient discordSocketClient;

        public DiscordSocketClientAbstraction(DiscordSocketClient discordSocketClient)
        {
            this.discordSocketClient = discordSocketClient;
        }

        public ISelfUser GetCurrentUser() => discordSocketClient.CurrentUser;
    }
}