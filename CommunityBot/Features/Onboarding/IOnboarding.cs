using Discord;

namespace CommunityBot.Features.Onboarding
{
    public interface IOnboarding
    {
        void JoinedGuild(IGuild guild);
    }
}
