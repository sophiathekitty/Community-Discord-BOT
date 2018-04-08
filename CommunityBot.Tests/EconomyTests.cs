using NUnit.Framework;

namespace CommunityBot.Tests
{
    public class EconomyTests
    {
        [Test]
        public void EconomyReactionTest([Values(0, 201, 551, 801, 1101, 2501, 5001, 10001, 20001, 50001, 100001)]ulong value)
        {
            var actual = Global.GetMiuniesCountReaction(value, string.Empty);
            Assert.NotNull(actual);
            Assert.AreNotEqual(string.Empty, actual);;
        }
    }
}
