using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CommunityBot.Features.Lists;

namespace CommunityBot.NUnit.Tests.FeatureTests
{
    public static class ListManagerTests
    {
        private static readonly String Name = "testname";

        [Test]
        public static void UnknownCommandTest()
        {
            Assert.Throws<ListManagerException>(() => ListManager.Manage(new[] { "-aabadf", "123" }));
        }

        [Test]
        public static void CreateListTest()
        {
            CustomList expected = new CustomList(Name);

            ListManager.Manage(new[] { "-c", Name });
            CustomList actual = ListManager.GetList(Name);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void RemoveListTest()
        {
            string expected = String.Format(ListErrorMessage.ListDoesNotExist_list, Name);

            ListManager.Manage(new[] { "-c", Name });
            ListManager.Manage(new[] { "-rl", Name });

            ListManagerException e = Assert.Throws<ListManagerException>(() => ListManager.GetList(Name));
            Assert.AreEqual(e.Message, expected);
        }

        [Test]
        public static void NoListsTest()
        {
            string expected = ListErrorMessage.NoLists;
            ListManagerException e = Assert.Throws<ListManagerException>(() => ListManager.Manage(new[] { "-g" }));
            Assert.AreEqual(e.Message, expected);
        }

        [Test]
        public static void AddItemTest()
        {
            String item = "New item";
            CustomList expected = new CustomList(Name);
            expected.Add(item);

            ListManager.Manage(new[] { "-c", Name });
            ListManager.Manage(new[] { "-a", item, Name });
            CustomList actual = ListManager.GetList(Name);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void InsertItemTest()
        {
            String item = "New item";
            CustomList expected = new CustomList(Name);
            expected.Add($"{item} 1");
            expected.Add($"{item} 2");
            expected.Add($"{item} 3");

            ListManager.Manage(new[] { "-c", Name });
            ListManager.Manage(new[] { "-i", "0", $"{item} 3", Name });
            ListManager.Manage(new[] { "-i", "0", $"{item} 2", Name });
            ListManager.Manage(new[] { "-i", "0", $"{item} 1", Name });
            CustomList actual = ListManager.GetList(Name);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void RemoveItemTest()
        {
            String item = "New item";
            CustomList expected = new CustomList(Name);
            expected.Add(item + " 0");
            expected.Add(item + " 2");

            ListManager.Manage(new[] { "-c", Name });
            ListManager.Manage(new[] { "-a", $"{item} 0", Name });
            ListManager.Manage(new[] { "-a", $"{item} 1", Name });
            ListManager.Manage(new[] { "-a", $"{item} 2", Name });
            ListManager.Manage(new[] { "-r", $"{item} 1", Name });
            CustomList actual = ListManager.GetList(Name);

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
            CustomList expected = new CustomList(Name);

            ListManager.Manage(new[] { "-c", Name });
            ListManager.Manage(new[] { "-a", $"{item} 0", Name });
            ListManager.Manage(new[] { "-a", $"{item} 1", Name });
            ListManager.Manage(new[] { "-a", $"{item} 2", Name });
            ListManager.Manage(new[] { "-cl", Name });
            CustomList actual = ListManager.GetList(Name);
            
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TearDown]
        public static void TearDown()
        {
            try
            {
                ListManager.RemoveList(Name);
            }
            catch (ListManagerException) { }
        }
    }
}
