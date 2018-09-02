using System;
using Xunit;

namespace CommunityBot.Tests
{
    public class ImmutableConstantsTests
    {
        [Fact]
        public static void ConstantArrayIsImmutableTest()
        {
            Assert.Throws<NotSupportedException>(() => Constants.DidYouKnows.Add("Hello, World!"));
            Assert.Throws<NotSupportedException>(() => Constants.DidYouKnows.RemoveAt(0));
            Assert.Throws<NotSupportedException>(() => Constants.DidYouKnows[0] = "Hello, World!");
            Assert.Throws<NotSupportedException>(() => Constants.DidYouKnows.Clear());
        }
    }
}
