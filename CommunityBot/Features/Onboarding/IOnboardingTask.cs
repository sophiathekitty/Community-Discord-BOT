using Discord;

namespace CommunityBot.Features.Onboarding
{
    public interface IOnboardingTask
    {
        void OnJoined(IGuild guild);
    }
}
