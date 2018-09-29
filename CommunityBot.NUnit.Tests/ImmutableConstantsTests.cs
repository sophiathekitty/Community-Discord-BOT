using System;
using NUnit.Framework;

namespace CommunityBot.Tests
{
    public static class ImmutableConstantsTests
    {
        [Test]
        public static void ConstantArrayIsImmutableTest()
        {
            Assert.Throws<NotSupportedException>(() => Constants.DidYouKnows.Add("Hello, World!"));
            Assert.Throws<NotSupportedException>(() => Constants.DidYouKnows.RemoveAt(0));
            Assert.Throws<NotSupportedException>(() => Constants.DidYouKnows[0] = "Hello, World!");
            Assert.Throws<NotSupportedException>(() => Constants.DidYouKnows.Clear());
        }
    }
}
