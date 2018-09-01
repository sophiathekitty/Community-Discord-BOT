using Castle.Core.Resource;
using CommunityBot.Configuration;
using Discord;
using Moq;
using Xunit;

namespace CommunityBot.Tests
{
    public class ApplicationSettingsTests
    {
        [Fact]
        public void HeadlessArgumentTest()
        {
            var settings = new ApplicationSettings(new []{ "-hl" });
            Assert.True(settings.Headless);
        }

        [Fact]
        public void VerboseArgumentTest()
        {
            var settings = new ApplicationSettings(new []{ "-vb" });
            Assert.True(settings.Verbose);
        }

        [Fact]
        public void CacheSizeArgumentTest()
        {
            const int expected = 999;
            var settings = new ApplicationSettings(new []{ $"-cs={expected}" });
            Assert.Equal(expected, settings.CacheSize);
        }

        [Fact]
        public void LogDestinationArgument_FileTest()
        {
            var settings = new ApplicationSettings(new []{ "-log=f" });
            Assert.True(settings.LogIntoFile);
            Assert.False(settings.LogIntoConsole);
        }

        [Fact]
        public void LogDestinationArgument_ConsoleTest()
        {
            var settings = new ApplicationSettings(new []{ "-log=c" });
            Assert.True(settings.LogIntoConsole);
            Assert.False(settings.LogIntoFile);
        }

        [Fact]
        public void LogDestinationArgument_ConsoleDefaultTest()
        {
            var settings = new ApplicationSettings(new []{ "" });
            Assert.True(settings.LogIntoConsole);
            Assert.False(settings.LogIntoFile);
        }

        [Fact]
        public void LogDestinationArgument_BothTest()
        {
            var settings = new ApplicationSettings(new []{ "-log=cf" });
            Assert.True(settings.LogIntoConsole);
            Assert.True(settings.LogIntoFile);
        }
    }
}
