using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Discord;
using CommunityBot.Configuration;
using CommunityBot.Features.Lists;
using Discord.Commands;
using Moq;
using static CommunityBot.Features.Lists.ListException;
using static CommunityBot.Helpers.ListHelper;
using Discord.WebSocket;
using System.Linq;
using System.Collections.ObjectModel;

namespace CommunityBot.NUnit.Tests.FeatureTests
{
    public static class ListManagerTests
    {
        private static readonly string TestListName = "testname";
        private static readonly string TestListItem = "item";
        private static readonly ulong EveryoneRoleId = 10;
        private static readonly UserInfo userInfo = new UserInfo(10, new ulong[] { EveryoneRoleId });
        private static readonly IDataStorage dataStorage = new JsonDataStorage();

        private static ListManager listManager;

        [OneTimeSetUp]
        public static void Setup()
        {
            listManager = new ListManager(GetDiscordSocketClient(), dataStorage);
        }

        [Test]
        public static void UnknownCommandTest()
        {
            Assert.Throws<ListManagerException>(() => Manage(new[] { "-aabadf", "123" }));
        }

        [Test]
        public static void CreatePrivateListTest()
        {
            CustomList expected = new CustomList(dataStorage, userInfo, ListPermission.PRIVATE, TestListName);

            Manage(new[] { "-c", TestListName });

            CustomList actual = listManager.GetList(TestListName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void PrivateListPermissionTest()
        {
            var expected = String.Format(ListErrorMessage.Permission.NoPermission_list, TestListName);

            Manage(new[] { "-c", TestListName });

            var differentUserInfo = new UserInfo(userInfo.Id + 1, userInfo.RoleIds);
            var e = Assert.Throws<ListManagerException>(
                () => listManager.Manage(differentUserInfo, new[] { "-a", TestListItem, TestListName })
            );

            Assert.AreEqual(expected, e.Message);
        }

        [Test]
        public static void CreatePublicListTest()
        {
            CustomList expected = new CustomList(dataStorage, userInfo, ListPermission.PUBLIC, TestListName);

            Manage(new[] { "-cp", TestListName });

            CustomList actual = listManager.GetList(TestListName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void PublicListPermissionTest()
        {
            var expected = new CustomList(dataStorage, userInfo, ListPermission.PUBLIC, TestListName);
            expected.Add(TestListItem);

            Manage(new[] { "-cp", TestListName });
            
            var differentUserInfo = new UserInfo(userInfo.Id + 1, userInfo.RoleIds);
            Assert.DoesNotThrow(
                () => listManager.Manage(differentUserInfo, new[] { "-a", TestListItem, TestListName })
            );

            var actual = listManager.GetList(TestListName);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void RemoveListTest()
        {
            string expected = String.Format(ListErrorMessage.General.ListDoesNotExist_list, TestListName);

            Manage(new[] { "-c", TestListName });
            Manage(new[] { "-rl", TestListName });

            var e = Assert.Throws<ListManagerException>(
                () => listManager.GetList(TestListName)
            );

            Assert.AreEqual(expected, e.Message);
        }

        [Test]
        public static void RemoveNonExistentListTest()
        {
            var excpected = String.Format(ListErrorMessage.General.ListDoesNotExist_list, TestListName);

            var e = Assert.Throws<ListManagerException>(
                () => Manage(new[] { "-rl", TestListName })
            );

            Assert.AreEqual(excpected, e.Message);
        }

        [Test]
        public static void NoListsTest()
        {
            string expected = ListErrorMessage.General.NoLists;

            var e = Assert.Throws<ListManagerException>(
                () => Manage(new[] { "-g" })
            );

            Assert.AreEqual(expected, e.Message);
        }

        [Test]
        public static void AddItemTest()
        {
            CustomList expected = new CustomList(dataStorage, userInfo, ListPermission.PRIVATE, TestListName);
            expected.Add(TestListItem);

            Manage(new[] { "-c", TestListName });
            Manage(new[] { "-a", TestListItem, TestListName });

            CustomList actual = listManager.GetList(TestListName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void InsertItemTest()
        {
            CustomList expected = new CustomList(dataStorage, userInfo, ListPermission.PRIVATE, TestListName);
            expected.Add($"{TestListItem} 1");
            expected.Add($"{TestListItem} 2");
            expected.Add($"{TestListItem} 3");

            Manage(new[] { "-c", TestListName });
            Manage(new[] { "-i", "1", $"{TestListItem} 3", TestListName });
            Manage(new[] { "-i", "1", $"{TestListItem} 2", TestListName });
            Manage(new[] { "-i", "1", $"{TestListItem} 1", TestListName });

            CustomList actual = listManager.GetList(TestListName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void InsertWithIndexOutOfBoundsTest()
        {
            var expected = String.Format(ListErrorMessage.General.IndexOutOfBounds_list, TestListName);

            Manage(new[] { "-c", TestListName });
            var e = Assert.Throws<ListManagerException>(
                () => Manage(new[] { "-i", "0", $"{TestListItem} 3", TestListName })
            );

            Assert.AreEqual(expected, e.Message);
        }

        [Test]
        public static void InsertWithWrongIndexTypeTest()
        {
            var expected = ListErrorMessage.General.WrongInputForIndex;

            Manage(new[] { "-c", TestListName });
            var e = Assert.Throws<ListManagerException>(
                () => Manage(new[] { "-i", "wrong_index", $"{TestListItem} 3", TestListName })
            );

            Assert.AreEqual(expected, e.Message);
        }

        [Test]
        public static void RemoveItemTest()
        {
            CustomList expected = new CustomList(dataStorage, userInfo, ListPermission.PRIVATE, TestListName);
            expected.Add(TestListItem + " 0");
            expected.Add(TestListItem + " 2");
            
            Manage(new[] { "-c", TestListName });
            Manage(new[] { "-a", $"{TestListItem} 0", TestListName });
            Manage(new[] { "-a", $"{TestListItem} 1", TestListName });
            Manage(new[] { "-a", $"{TestListItem} 2", TestListName });
            Manage(new[] { "-r", $"{TestListItem} 1", TestListName });

            CustomList actual = listManager.GetList(TestListName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void RemoveItemMultipleTimesTest()
        {
            for (int i = 0; i < 10; i++)
            {
                RemoveItemTest();
                TearDown();
            }
        }

        [Test]
        public static void ClearListTest()
        {
            CustomList expected = new CustomList(dataStorage, userInfo, ListPermission.PRIVATE, TestListName);

            Manage(new[] { "-c", TestListName });
            Manage(new[] { "-a", $"{TestListItem} 0", TestListName });
            Manage(new[] { "-a", $"{TestListItem} 1", TestListName });
            Manage(new[] { "-a", $"{TestListItem} 2", TestListName });
            Manage(new[] { "-cl", TestListName });

            CustomList actual = listManager.GetList(TestListName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void GetAllPublicPermissionTest()
        {
            var expected = ListPermission.PUBLIC;

            listManager.CreateListPublic(userInfo, new[] { TestListName });

            var actual = Manage(new[] { "-gp" }).permission;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void GetAllPrivatePermissionTest()
        {
            var expected = ListPermission.PRIVATE;

            listManager.CreateListPrivate(userInfo, new[] { TestListName });

            var actual = Manage(new[] { "-g" }).permission;

            Assert.AreEqual(expected, actual);
        }

        [TearDown]
        public static void TearDown()
        {
            try
            {
                listManager.RemoveList(userInfo, TestListName);
            }
            catch (ListManagerException) { }
        }

        private static ListOutput Manage(params string[] args)
        {
            return listManager.Manage(userInfo, args);
        }

        private static DiscordSocketClient GetDiscordSocketClient()
        {
            var roleName = "myRole";
            var client = new Mock<DiscordSocketClient>();
            var role = new Mock<IRole>();
            var roles = new Collection<SocketRole>();
            var guild = new Mock<IGuild>();
            var guilds = new Collection<SocketGuild>();

            role.Setup(r => r.Name).Returns(roleName);
            roles.Add(role.Object as SocketRole);

            guild.Setup(g => g.Roles).Returns(roles);
            guilds.Add(guild.Object as SocketGuild);
            return client.Object;
        }
    }
}
