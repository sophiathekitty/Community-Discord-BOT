using Castle.Core.Resource;
using CommunityBot.Configuration;
using Discord;
using Moq;
using NUnit.Framework;

namespace CommunityBot.Tests
{
    public class ApplicationSettingsTests
    {
        [Test]
        public void HeadlessArgumentTest()
        {
            var settings = new ApplicationSettings(new []{ "-hl" });
            Assert.True(settings.Headless);
        }

        [Test]
        public void VerboseArgumentTest()
        {
            var settings = new ApplicationSettings(new []{ "-vb" });
            Assert.True(settings.Verbose);
        }

        [Test]
        public void CacheSizeArgumentTest()
        {
            const int expected = 999;
            var settings = new ApplicationSettings(new []{ $"-cs={expected}" });
            Assert.AreEqual(expected, settings.CacheSize);
        }

        [Test]
        public void LogDestinationArgument_FileTest()
        {
            var settings = new ApplicationSettings(new []{ "-log=f" });
            Assert.True(settings.LogIntoFile);
            Assert.False(settings.LogIntoConsole);
        }

        [Test]
        public void LogDestinationArgument_ConsoleTest()
        {
            var settings = new ApplicationSettings(new []{ "-log=c" });
            Assert.True(settings.LogIntoConsole);
            Assert.False(settings.LogIntoFile);
        }

        [Test]
        public void LogDestinationArgument_ConsoleDefaultTest()
        {
            var settings = new ApplicationSettings(new []{ "" });
            Assert.True(settings.LogIntoConsole);
            Assert.False(settings.LogIntoFile);
        }

        [Test]
        public void LogDestinationArgument_BothTest()
        {
            var settings = new ApplicationSettings(new []{ "-log=cf" });
            Assert.True(settings.LogIntoConsole);
            Assert.True(settings.LogIntoFile);
        }
    }
}
