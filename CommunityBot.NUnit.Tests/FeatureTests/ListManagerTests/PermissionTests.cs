using CommunityBot.Configuration;
using CommunityBot.Features.Lists;
using CommunityBot.Helpers;
using Discord;
using Discord.WebSocket;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using static CommunityBot.Features.Lists.ListException;
using static CommunityBot.Helpers.ListHelper;

namespace CommunityBot.NUnit.Tests.FeatureTests.ListManagerTests
{
    public class PermissionTests : ListManagerTestsHelper
    {
        [Test]
        public static void PrivateListPermissionTest()
        {
            var expected = String.Format(ListErrorMessage.Permission.NoPermission_list, TestListName);

            Manage(new[] { "-c", TestListName });

            var differentUserInfo = new UserInfo(TestUserInfo.Id + 1, TestUserInfo.RoleIds);
            var e = Assert.Throws<ListManagerException>(
                () => listManager.Manage(differentUserInfo, new[] { "-a", TestListItem, TestListName })
            );

            Assert.AreEqual(expected, e.Message);
        }

        [Test]
        public static void PublicListPermissionTest()
        {
            var expected = new CustomList(TestDataStorage, TestUserInfo, ListPermission.PUBLIC, TestListName);
            expected.Add(TestListItem);

            Manage(new[] { "-cp", TestListName });

            var differentUserInfo = new UserInfo(TestUserInfo.Id + 1, TestUserInfo.RoleIds);
            Assert.DoesNotThrow(
                () => listManager.Manage(differentUserInfo, new[] { "-a", TestListItem, TestListName })
            );

            var actual = listManager.GetList(TestListName);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void GetAllPublicPermissionTest()
        {
            var expected = ListPermission.PUBLIC;

            listManager.CreateListPublic(TestUserInfo, new[] { TestListName });

            var actual = Manage(new[] { "-gp" }).permission;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void GetAllPrivatePermissionTest()
        {
            var expected = ListPermission.PRIVATE;

            listManager.CreateListPrivate(TestUserInfo, new[] { TestListName });

            var actual = Manage(new[] { "-g" }).permission;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void ChangePermissionByRoleTest()
        {
            var expectedResult = new CustomList(TestDataStorage, TestUserInfo, ListPermission.PUBLIC, TestListName);

            var modifier = ListHelper.ValidPermissions
                .Where(vp => vp.Value == ListPermission.PUBLIC)
                .Select(vp => vp.Key)
                .FirstOrDefault();

            var expectedExceptionMessage = String.Format(ListErrorMessage.Permission.NoPermission_list, TestListName);

            Manage(new[] { "-c", TestListName });

            var differentUserInfo = new UserInfo(TestUserInfo.Id + 1, TestUserInfo.RoleIds);
            var e = Assert.Throws<ListManagerException>(
                () => listManager.Manage(differentUserInfo, new[] { "-a", TestListItem, TestListName })
            );

            Assert.AreEqual(expectedExceptionMessage, e.Message);

            Manage(new[] { "-m", TestListName, TestRoleName, modifier });

            Assert.DoesNotThrow(
                () => listManager.Manage(differentUserInfo, new[] { "-a", TestListItem, TestListName })
            );
        }
    }
}
