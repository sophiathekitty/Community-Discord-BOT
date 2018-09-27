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
        public static void CreateListTest()
        {
            CustomList expected = new CustomList(name);

            ListManager.Manage(new String[] { "-c", name });
            CustomList actual = ListManager.GetList(name);
            
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
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
        }
    }
}
