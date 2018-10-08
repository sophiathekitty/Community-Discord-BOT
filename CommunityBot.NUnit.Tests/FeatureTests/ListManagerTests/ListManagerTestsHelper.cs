using CommunityBot.Configuration;
using CommunityBot.Features.Lists;
using Discord;
using Discord.WebSocket;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using static CommunityBot.Features.Lists.ListException;
using static CommunityBot.Helpers.ListHelper;
using System.Linq;

namespace CommunityBot.NUnit.Tests.FeatureTests.ListManagerTests
{
    public class ListManagerTestsHelper
    {
        protected static readonly ulong TestUserId = 10;

        protected static readonly string EveryoneRoleName       = "everyone";
        protected static readonly ulong EveryoneRoleId          = 50;
        protected static readonly string TestRoleName           = "myTestRole";
        protected static readonly ulong TestRoleId              = 60;

        protected static readonly string TestListName           = "testname";
        protected static readonly string TestListItem           = "item";

        protected static readonly UserInfo TestUserInfo         = new UserInfo(TestUserId, new ulong[] { EveryoneRoleId, TestRoleId });
        protected static readonly IDataStorage TestDataStorage  = new JsonDataStorage();
        
        protected static ListManager listManager;

        [OneTimeSetUp]
        protected static void Setup()
        {
            listManager = new ListManager(GetDiscordSocketClient(), TestDataStorage);
        }

        [TearDown]
        protected static void TearDown()
        {
            try
            {
                listManager.RemoveList(TestUserInfo, TestListName);
            }
            catch (ListManagerException) { }
        }

        protected static ListOutput Manage(params string[] args)
        {
            return listManager.Manage(TestUserInfo, args);
        }

        protected static DiscordSocketClient GetDiscordSocketClient()
        {
            
            var client = new Mock<DiscordSocketClient>();
            //var everyoneRole = new Mock<SocketRole>();
            var testRole = new Mock<IRole>();
            var roles = new Collection<IRole>();
            var guild = new Mock<IGuild>();
            var guilds = new Collection<IGuild>();
            //everyoneRole.Setup(r => r.Name).Returns(EveryoneRoleName);
            testRole.Setup(r => r.Name).Returns(TestRoleName);
            roles.Add(testRole.Object);

            guild.Setup(g => g.Roles).Returns(roles);
            guilds.Add(guild.Object);

            client.Setup(c => c.Guilds).Returns(guilds);
            return client.Object;
        }
    }
}
