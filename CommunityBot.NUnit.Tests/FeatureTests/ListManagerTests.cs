using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CommunityBot.Features.Lists;

namespace CommunityBot.NUnit.Tests.FeatureTests
{
    public class ListManagerTests
    {
        private static readonly String name = "testname";

        [Test]
        public static void UnknownCommandTest()
        {
            var expected = ListManager.unknownCommandErrorMsg;
            var actual = ListManager.Manage(new String[] { "-aabadf", "123" });

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void CreateListTest()
        {
            CustomList expected = new CustomList(name);

            ListManager.Manage(new String[] { "-c", name });
            CustomList actual = ListManager.GetList(name);
            
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);

            ListManager.RemoveList(name);
        }

        [Test]
        public static void RemoveListTest()
        {
            ListManager.Manage(new String[] { "-c", name });
            ListManager.Manage(new string[] { "-r", name });
            CustomList actual = ListManager.GetList(name);
            
            Assert.IsNull(actual);
        }

        [Test]
        public static void AddItemTest()
        {
            String item = "New item";
            CustomList expected = new CustomList(name);
            expected.Add(item);

            ListManager.Manage(new String[] { "-c", name });
            ListManager.Manage(new String[] { "-a", item, name });
            CustomList actual = ListManager.GetList(name);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);

            ListManager.RemoveList(name);
        }

        [Test]
        public static void RemoveItemTest()
        {
            String item = "New item";
            CustomList expected = new CustomList(name);
            expected.Add(item + " 0");
            expected.Add(item + " 2");

            ListManager.Manage(new String[] { "-c", name });
            ListManager.Manage(new String[] { "-a", item + " 0", name });
            ListManager.Manage(new String[] { "-a", item + " 1", name });
            ListManager.Manage(new String[] { "-a", item + " 2", name });
            ListManager.Manage(new String[] { "-r", item + " 1", name });
            CustomList actual = ListManager.GetList(name);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);

            ListManager.RemoveList(name);
        }

        [Test]
        public static void RemoveItemMultipleTimesTest()
        {
            for (int i = 0; i < 10; i++)
            {
                RemoveItemTest();
            }
        }

        [Test]
        public static void ClearListTest()
        {
            String item = "New item";
            CustomList expected = new CustomList(name);

            ListManager.Manage(new String[] { "-c", name });
            ListManager.Manage(new String[] { "-a", item + " 0", name });
            ListManager.Manage(new String[] { "-a", item + " 1", name });
            ListManager.Manage(new String[] { "-a", item + " 2", name });
            ListManager.Manage(new String[] { "-cl", name });
            CustomList actual = ListManager.GetList(name);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);

            ListManager.RemoveList(name);
        }
    }
}
