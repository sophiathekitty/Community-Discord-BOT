using Discord;

namespace CommunityBot.DiscordAbstractions
{
    public interface IDiscordSocketClient
    {
        ISelfUser GetCurrentUser();
    }
}