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
        protected static readonly string OwnerRoleName          = "myTestRole";
        protected static readonly ulong OwnerRoleId             = 60;
        protected static readonly string TestRoleName           = "myTestRole1";
        protected static readonly ulong TestRoleId              = 70;

        protected static readonly string TestListName           = "testname";
        protected static readonly string TestListItem           = "item";

        protected static readonly UserInfo TestUserInfo         = new UserInfo(TestUserId, new[] { EveryoneRoleId, OwnerRoleId });
        protected static readonly UserInfo DifferentUserInfo    = new UserInfo(TestUserId + 1, new[] { EveryoneRoleId, OwnerRoleId });
        protected static readonly IDataStorage TestDataStorage  = new JsonDataStorage();
        
        protected static ListManager listManager;

        [OneTimeSetUp]
        protected static void Setup()
        {
            listManager = new ListManager(TestDataStorage);
        }

        [TearDown]
        protected static void TearDown()
        {
            try
            {
                Manage(new[] { "-rl", TestListName });
            }
            catch (ListManagerException) { }
        }

        protected static ListOutput Manage(params string[] args)
        {
            return Manage(TestUserInfo, args);
        }

        protected static ListOutput Manage(UserInfo userInfo, params string[] args)
        {
            return listManager.Manage(userInfo, GetFakeRoles(), args);
        }

        protected static Dictionary<string, ulong> GetFakeRoles()
        {
            var roles = new Dictionary<string, ulong>
            {
                { EveryoneRoleName, EveryoneRoleId },
                { OwnerRoleName, OwnerRoleId },
                { TestRoleName, TestRoleId }
            };
            return roles;
        }
    }
}
