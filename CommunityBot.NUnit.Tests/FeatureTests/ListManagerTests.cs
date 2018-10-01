using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Discord;
using CommunityBot.Features.Lists;
using Discord.Commands;
using Moq;

namespace CommunityBot.NUnit.Tests.FeatureTests
{
    public static class ListManagerTests
    {
        private static readonly String Name = "testname";
        private static readonly ulong userId = 10;

        [Test]
        public static void UnknownCommandTest()
        {
            Assert.Throws<ListManagerException>(() => ListManager.Manage(userId, new[] { "-aabadf", "123" }));
        }

        [Test]
        public static void CreatePrivateListTest()
        {
            CustomList expected = new CustomList(userId, CustomList.ListPermission.PRIVATE, Name);

            ListManager.Manage(userId, new[] { "-c", Name });
            CustomList actual = ListManager.GetList(userId, Name);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void PrivateListPermissionTest()
        {
            var expected = String.Format(ListErrorMessage.NoPermission_list, Name);

            ListManager.Manage(userId, new[] { "-c", Name });

            var e = Assert.Throws<ListManagerException>(
                () => ListManager.Manage(userId + 1, new[] { "-a", "item", Name })
            );

            Assert.AreEqual(expected, e.Message);
        }

        [Test]
        public static void CreatePublicListTest()
        {
            CustomList expected = new CustomList(userId, CustomList.ListPermission.PUBLIC, Name);

            ListManager.Manage(userId, new[] { "-cp", Name });
            CustomList actual = ListManager.GetList(userId, Name);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void PublicListPermissionTest()
        {
            var item = "item";
            var expected = new CustomList(userId, CustomList.ListPermission.PUBLIC, Name);
            expected.Add(item);

            ListManager.Manage(userId, new[] { "-cp", Name });

            Assert.DoesNotThrow(
                () => ListManager.Manage(userId + 1, new[] { "-a", item, Name })
            );

            var actual = ListManager.GetList(userId, Name);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void RemoveListTest()
        {
            string expected = String.Format(ListErrorMessage.ListDoesNotExist_list, Name);

            ListManager.Manage(userId, new[] { "-c", Name });
            ListManager.Manage(userId, new[] { "-rl", Name });

            var e = Assert.Throws<ListManagerException>(
                () => ListManager.GetList(userId, Name)
            );
            Assert.AreEqual(expected, e.Message);
        }

        [Test]
        public static void RemoveNonExistentListTest()
        {
            var excpected = String.Format(ListErrorMessage.ListDoesNotExist_list, Name);
            var e = Assert.Throws<ListManagerException>(
                () => ListManager.Manage(userId, new[] { "-rl", Name })
            );
            Assert.AreEqual(excpected, e.Message);
        }

        [Test]
        public static void NoListsTest()
        {
            string expected = ListErrorMessage.NoLists;

            var e = Assert.Throws<ListManagerException>(
                () => ListManager.Manage(userId, new[] { "-g" })
            );

            Assert.AreEqual(expected, e.Message);
        }

        [Test]
        public static void AddItemTest()
        {
            String item = "New item";
            CustomList expected = new CustomList(userId, CustomList.ListPermission.PRIVATE, Name);
            expected.Add(item);

            ListManager.Manage(userId, new[] { "-c", Name });
            ListManager.Manage(userId, new[] { "-a", item, Name });
            CustomList actual = ListManager.GetList(userId, Name);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void InsertItemTest()
        {
            String item = "New item";
            CustomList expected = new CustomList(userId, CustomList.ListPermission.PRIVATE, Name);
            expected.Add($"{item} 1");
            expected.Add($"{item} 2");
            expected.Add($"{item} 3");

            ListManager.Manage(userId, new[] { "-c", Name });
            ListManager.Manage(userId, new[] { "-i", "1", $"{item} 3", Name });
            ListManager.Manage(userId, new[] { "-i", "1", $"{item} 2", Name });
            ListManager.Manage(userId, new[] { "-i", "1", $"{item} 1", Name });
            CustomList actual = ListManager.GetList(userId, Name);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void InsertWithIndexOutOfBoundsTest()
        {
            var item = "item";
            var expected = String.Format(ListErrorMessage.IndexOutOfBounds_list, Name);

            ListManager.Manage(userId, new[] { "-c", Name });
            var e = Assert.Throws<ListManagerException>(
                () => ListManager.Manage(userId, new[] { "-i", "0", $"{item} 3", Name })
            );

            Assert.AreEqual(expected, e.Message);
        }

        [Test]
        public static void InsertWithWrongIndexTypeTest()
        {
            var item = "item";
            var expected = ListErrorMessage.WrongInputForIndex;

            ListManager.Manage(userId, new[] { "-c", Name });
            var e = Assert.Throws<ListManagerException>(
                () => ListManager.Manage(userId, new[] { "-i", "wrong_index", $"{item} 3", Name })
            );

            Assert.AreEqual(expected, e.Message);
        }

        [Test]
        public static void RemoveItemTest()
        {
            String item = "New item";
            CustomList expected = new CustomList(userId, CustomList.ListPermission.PRIVATE, Name);
            expected.Add(item + " 0");
            expected.Add(item + " 2");

            ListManager.Manage(userId, new[] { "-c", Name });
            ListManager.Manage(userId, new[] { "-a", $"{item} 0", Name });
            ListManager.Manage(userId, new[] { "-a", $"{item} 1", Name });
            ListManager.Manage(userId, new[] { "-a", $"{item} 2", Name });
            ListManager.Manage(userId, new[] { "-r", $"{item} 1", Name });
            CustomList actual = ListManager.GetList(userId, Name);

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
            String item = "New item";
            CustomList expected = new CustomList(userId, CustomList.ListPermission.PRIVATE, Name);

            ListManager.Manage(userId, new[] { "-c", Name });
            ListManager.Manage(userId, new[] { "-a", $"{item} 0", Name });
            ListManager.Manage(userId, new[] { "-a", $"{item} 1", Name });
            ListManager.Manage(userId, new[] { "-a", $"{item} 2", Name });
            ListManager.Manage(userId, new[] { "-cl", Name });
            CustomList actual = ListManager.GetList(userId, Name);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void GetAllPublicPermissionTest()
        {
            var expected = CustomList.ListPermission.PUBLIC;

            ListManager.CreateListPublic(userId, new[] { Name });

            var actual = ListManager.Manage(userId, new[] { "-gp" }).permission;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void GetAllPrivatePermissionTest()
        {
            var expected = CustomList.ListPermission.PRIVATE;

            ListManager.CreateListPrivate(userId, new[] { Name });

            var actual = ListManager.Manage(userId, new[] { "-g" }).permission;

            Assert.AreEqual(expected, actual);
        }

        [TearDown]
        public static void TearDown()
        {
            try
            {
                ListManager.RemoveList(userId, Name);
            }
            catch (ListManagerException) { }
        }
    }
}
