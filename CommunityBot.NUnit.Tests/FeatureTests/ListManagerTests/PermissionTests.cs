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
            
            var e = Assert.Throws<ListPermissionException>(
                () => Manage(DifferentUserInfo, new[] { "-a", TestListItem, TestListName })
            );

            Assert.AreEqual(expected, e.Message);
        }

        [Test]
        public static void PublicListPermissionTest()
        {
            var expected = new CustomList(TestDataStorage, TestUserInfo, ListPermission.PUBLIC, TestListName);
            expected.Add(TestListItem);

            Manage(new[] { "-cp", TestListName });
            
            Assert.DoesNotThrow(
                () => Manage(DifferentUserInfo, new[] { "-a", TestListItem, TestListName })
            );

            var actual = listManager.GetList(TestListName);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void GetAllPublicPermissionTest()
        {
            var expected = ListPermission.PUBLIC;

            Manage(new[] { "-cp", TestListName });

            var actual = Manage(new[] { "-gp" }).permission;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void GetAllPrivatePermissionTest()
        {
            var expected = ListPermission.PRIVATE;

            Manage(new[] { "-cp", TestListName });

            var actual = Manage(new[] { "-g" }).permission;

            Assert.AreEqual(expected, actual);
        }
        
        private static IEnumerable<object[]> PermissionByRoleTestCases
        {
            get
            {
                yield return new object[] { ListPermission.PUBLIC, new[] { "-a", TestListItem, TestListName }};
                yield return new object[] { ListPermission.READ, new[] { "-l", TestListName }};
            }
        }

        [Test, TestCaseSource("PermissionByRoleTestCases")]
        public static void ChangePermissionByRoleTest(object[] args)
        {
            var permission = (ListPermission) args[0];
            var commandArgs = (string[]) args[1];

            var modifier = ListHelper.ValidPermissions
                .Where(vp => vp.Value == permission)
                .Select(vp => vp.Key)
                .FirstOrDefault();

            var expectedExceptionMessage = String.Format(ListErrorMessage.Permission.NoPermission_list, TestListName);

            Manage(new[] { "-c", TestListName });
            Manage(new[] { "-a", TestListItem, TestListName });
            
            var e = Assert.Throws<ListPermissionException>(
                () => Manage(DifferentUserInfo, commandArgs)
            );

            Assert.AreEqual(expectedExceptionMessage, e.Message);

            Manage(new[] { "-m", TestListName, OwnerRoleName, modifier });
            
            Assert.DoesNotThrow(
                () => Manage(DifferentUserInfo, commandArgs)
            );
        }

        [Test]
        public static void ChangePermissionByRoleListTest()
        {
            var modifier = ListHelper.ValidPermissions
                .Where(vp => vp.Value == ListPermission.LIST)
                .Select(vp => vp.Key)
                .FirstOrDefault();

            Manage(new[] { "-c", TestListName });
            Manage(new[] { "-a", TestListItem, TestListName });

            var result = Manage(DifferentUserInfo, new[] { "-gp", TestListName });

            Assert.IsFalse(result.outputString.Contains(TestListName));

            Manage(new[] { "-m", TestListName, OwnerRoleName, modifier });

            result = Manage(DifferentUserInfo, new[] { "-gp", TestListName });

            Assert.IsTrue(result.outputString.Contains(TestListName));
        }
    }
}
