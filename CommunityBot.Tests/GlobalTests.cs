using Castle.Core.Resource;
using Discord;
using Moq;
using NUnit.Framework;

namespace CommunityBot.Tests
{
    public class GlobalTests
    {
        [Test]
        public void FirstUnitTest()
        {
            Assert.Pass();
        }

        [Test]
        public void ReplacePlaceholderString_NoNicknameTest()
        {
            const string expected = "Username";
            var guildUser = CreateMockGuildUser(null, expected);
            TestReplacePlaceholderString("<username>", expected, guildUser);
        }

        [Test]
        public void ReplacePlaceholderString_ValidUsernameTest()
        {
            const string expected = "Nickname";
            var guildUser = CreateMockGuildUser(expected, "Username");
            TestReplacePlaceholderString("<username>", expected, guildUser);
        }

        [Test]
        public void ReplacePlaceholderString_GuildNameTest()
        {
            const string expected = "TestGuildName";
            var guildUser = CreateMockGuildUser("Jon", "jj36", expected);
            TestReplacePlaceholderString("<guildname>", expected, guildUser);
        }

        [Test]
        public void ReplacePlaceholderString_UserMentionTest()
        {
            const string expected = "@JonDoe";
            var guildUser = CreateMockGuildUser("Jon", "jj36", userMention: "@JonDoe");
            TestReplacePlaceholderString("<usermention>", expected, guildUser);
        }

        [Test]
        public void ReplacePlaceholderString_Multiple()
        {
            const string expected = "Hello, Peter! Welcome to MyCoolGuild!";
            var guildUser = CreateMockGuildUser("Peter", "spelos", "MyCoolGuild");
            TestReplacePlaceholderString("Hello, <username>! Welcome to <guildname>!", expected, guildUser);
        }

        [Test]
        public void ReplacePlaceholderString_MultipleNoNickname()
        {
            const string expected = "Hello, spelos! Welcome to MyCoolGuild!";
            var guildUser = CreateMockGuildUser(null, "spelos", "MyCoolGuild");
            TestReplacePlaceholderString("Hello, <username>! Welcome to <guildname>!", expected, guildUser);
        }

        private static void TestReplacePlaceholderString(string input, string expected, IMock<IGuildUser> user)
        {
            var actual = input.ReplacePlacehoderStrings(user.Object);
            Assert.AreEqual(expected, actual);
        }

        private static Mock<IGuildUser> CreateMockGuildUser(string nickname, string username, string guildName = "MyGuild", string userMention = "@User")
        {
            var guildUser = new Mock<IGuildUser>();
            guildUser.Setup(gUser => gUser.Nickname).Returns(nickname);
            guildUser.Setup(gUser => gUser.Username).Returns(username);
            guildUser.Setup(gUser => gUser.Mention).Returns(userMention);
            guildUser.Setup(gUser => gUser.Guild.Name).Returns(guildName);
            return guildUser;
        }
    }
}
