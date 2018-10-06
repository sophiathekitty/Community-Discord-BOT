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

namespace CommunityBot.NUnit.Tests.FeatureTests
{
    public static class ListManagerTests
    {
        private static readonly string TestListName = "testname";
        private static readonly string TestListItem = "item";
        private static readonly UserInfo userInfo = new UserInfo(10, new ulong[] { 10 });
        private static readonly IDataStorage dataStorage = new JsonDataStorage();
        private static readonly ListManager listManager = new ListManager(dataStorage);

        [Test]
        public static void UnknownCommandTest()
        {
            Assert.Throws<ListManagerException>(() => listManager.Manage(userInfo, new[] { "-aabadf", "123" }));
        }

        [Test]
        public static void CreatePrivateListTest()
        {
            CustomList expected = new CustomList(dataStorage, userInfo, ListPermission.PRIVATE, TestListName);

            listManager.Manage(userInfo, new[] { "-c", TestListName });
            CustomList actual = listManager.GetList(TestListName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void PrivateListPermissionTest()
        {
            var expected = String.Format(ListErrorMessage.Permission.NoPermission_list, TestListName);

            listManager.Manage(userInfo, new[] { "-c", TestListName });

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

            listManager.Manage(userInfo, new[] { "-cp", TestListName });
            CustomList actual = listManager.GetList(TestListName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void PublicListPermissionTest()
        {
            var expected = new CustomList(dataStorage, userInfo, ListPermission.PUBLIC, TestListName);
            expected.Add(TestListItem);

            listManager.Manage(userInfo, new[] { "-cp", TestListName });
            
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

            listManager.Manage(userInfo, new[] { "-c", TestListName });
            listManager.Manage(userInfo, new[] { "-rl", TestListName });

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
                () => listManager.Manage(userInfo, new[] { "-rl", TestListName })
            );
            Assert.AreEqual(excpected, e.Message);
        }

        [Test]
        public static void NoListsTest()
        {
            string expected = ListErrorMessage.General.NoLists;

            var e = Assert.Throws<ListManagerException>(
                () => listManager.Manage(userInfo, new[] { "-g" })
            );

            Assert.AreEqual(expected, e.Message);
        }

        [Test]
        public static void AddItemTest()
        {
            CustomList expected = new CustomList(dataStorage, userInfo, ListPermission.PRIVATE, TestListName);
            expected.Add(TestListItem);

            listManager.Manage(userInfo, new[] { "-c", TestListName });
            listManager.Manage(userInfo, new[] { "-a", TestListItem, TestListName });
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

            listManager.Manage(userInfo, new[] { "-c", TestListName });
            listManager.Manage(userInfo, new[] { "-i", "1", $"{TestListItem} 3", TestListName });
            listManager.Manage(userInfo, new[] { "-i", "1", $"{TestListItem} 2", TestListName });
            listManager.Manage(userInfo, new[] { "-i", "1", $"{TestListItem} 1", TestListName });
            CustomList actual = listManager.GetList(TestListName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void InsertWithIndexOutOfBoundsTest()
        {
            var expected = String.Format(ListErrorMessage.General.IndexOutOfBounds_list, TestListName);

            listManager.Manage(userInfo, new[] { "-c", TestListName });
            var e = Assert.Throws<ListManagerException>(
                () => listManager.Manage(userInfo, new[] { "-i", "0", $"{TestListItem} 3", TestListName })
            );

            Assert.AreEqual(expected, e.Message);
        }

        [Test]
        public static void InsertWithWrongIndexTypeTest()
        {
            var expected = ListErrorMessage.General.WrongInputForIndex;

            listManager.Manage(userInfo, new[] { "-c", TestListName });
            var e = Assert.Throws<ListManagerException>(
                () => listManager.Manage(userInfo, new[] { "-i", "wrong_index", $"{TestListItem} 3", TestListName })
            );

            Assert.AreEqual(expected, e.Message);
        }

        [Test]
        public static void RemoveItemTest()
        {
            CustomList expected = new CustomList(dataStorage, userInfo, ListPermission.PRIVATE, TestListName);
            expected.Add(TestListItem + " 0");
            expected.Add(TestListItem + " 2");

            listManager.Manage(userInfo, new[] { "-c", TestListName });
            listManager.Manage(userInfo, new[] { "-a", $"{TestListItem} 0", TestListName });
            listManager.Manage(userInfo, new[] { "-a", $"{TestListItem} 1", TestListName });
            listManager.Manage(userInfo, new[] { "-a", $"{TestListItem} 2", TestListName });
            listManager.Manage(userInfo, new[] { "-r", $"{TestListItem} 1", TestListName });
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

            listManager.Manage(userInfo, new[] { "-c", TestListName });
            listManager.Manage(userInfo, new[] { "-a", $"{TestListItem} 0", TestListName });
            listManager.Manage(userInfo, new[] { "-a", $"{TestListItem} 1", TestListName });
            listManager.Manage(userInfo, new[] { "-a", $"{TestListItem} 2", TestListName });
            listManager.Manage(userInfo, new[] { "-cl", TestListName });
            CustomList actual = listManager.GetList(TestListName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void GetAllPublicPermissionTest()
        {
            var expected = ListPermission.PUBLIC;

            listManager.CreateListPublic(userInfo, new[] { TestListName });

            var actual = listManager.Manage(userInfo, new[] { "-gp" }).permission;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void GetAllPrivatePermissionTest()
        {
            var expected = ListPermission.PRIVATE;

            listManager.CreateListPrivate(userInfo, new[] { TestListName });

            var actual = listManager.Manage(userInfo, new[] { "-g" }).permission;

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
    }
}
