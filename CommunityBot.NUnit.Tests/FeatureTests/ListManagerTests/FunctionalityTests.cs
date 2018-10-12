using Moq;
using System;
using Discord;
using System.Text;
using System.Linq;
using NUnit.Framework;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using CommunityBot.Configuration;
using CommunityBot.Features.Lists;
using System.Collections.ObjectModel;
using static CommunityBot.Helpers.ListHelper;
using static CommunityBot.Features.Lists.ListException;

namespace CommunityBot.NUnit.Tests.FeatureTests.ListManagerTests
{
    public class FunctionalityTests : ListManagerTestsHelper
    {
        [Test]
        public static void UnknownCommandTest()
        {
            Assert.Throws<ListManagerException>(() => Manage(new[] { "-aabadf", "123" }));
        }

        [Test]
        public static void CreatePrivateListTest()
        {
            CustomList expected = new CustomList(TestDataStorage, TestUserInfo, ListPermission.PRIVATE, TestListName);

            Manage(new[] { "-c", TestListName });

            CustomList actual = listManager.GetList(TestListName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void CreatePublicListTest()
        {
            CustomList expected = new CustomList(TestDataStorage, TestUserInfo, ListPermission.PUBLIC, TestListName);

            Manage(new[] { "-cp", TestListName });

            CustomList actual = listManager.GetList(TestListName);

            Assert.IsNotNull(actual);
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
            CustomList expected = new CustomList(TestDataStorage, TestUserInfo, ListPermission.PRIVATE, TestListName);
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
            CustomList expected = new CustomList(TestDataStorage, TestUserInfo, ListPermission.PRIVATE, TestListName);
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
            CustomList expected = new CustomList(TestDataStorage, TestUserInfo, ListPermission.PRIVATE, TestListName);
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
            CustomList expected = new CustomList(TestDataStorage, TestUserInfo, ListPermission.PRIVATE, TestListName);

            Manage(new[] { "-c", TestListName });
            Manage(new[] { "-a", $"{TestListItem} 0", TestListName });
            Manage(new[] { "-a", $"{TestListItem} 1", TestListName });
            Manage(new[] { "-a", $"{TestListItem} 2", TestListName });
            Manage(new[] { "-cl", TestListName });

            CustomList actual = listManager.GetList(TestListName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }
    }
}
