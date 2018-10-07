using CommunityBot.Extensions;
using CommunityBot.Features.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunityBot.Helpers
{
    public static class ListHelper
    {
        public enum ListPermission
        {
            PRIVATE,
            LIST,
            READ,
            PUBLIC
        };

        public static IReadOnlyList<String> PermissionStrings = new List<String>
        {
            "private",
            "view only",
            "read only",
            "public"
        };

        public static readonly IReadOnlyDictionary<string, ListPermission> ValidPermissions = new Dictionary<string, ListPermission>
        {
            { "-p", ListPermission.PRIVATE },
            { "-l", ListPermission.LIST },
            { "-r", ListPermission.READ },
            { "-pu", ListPermission.PUBLIC }
        };

        public struct UserInfo
        {
            public ulong Id { get; }
            public ulong[] RoleIds { get; }

            public UserInfo(ulong id, ulong[] roleIds)
            {
                this.Id = id;
                this.RoleIds = roleIds;
            }
        }

        public struct ListOutput : IEquatable<object>
        {
            public string outputString { get; set; }
            public Discord.Embed outputEmbed { get; set; }
            public bool listenForReactions { get; set; }
            public ListPermission permission { get; set; }
        }

        public static ListOutput GetListOutput(string s)
        {
            return new ListOutput { outputString = s, permission = ListPermission.PUBLIC };
        }

        public static ListOutput GetListOutput(string s, ListPermission p)
        {
            return new ListOutput { outputString = s, permission = p };
        }

        public static ListOutput GetListOutput(Discord.Embed e)
        {
            return new ListOutput { outputEmbed = e, permission = ListPermission.PUBLIC };
        }

        public static ListOutput GetListOutput(Discord.Embed e, ListPermission p)
        {
            return new ListOutput { outputEmbed = e, permission = p };
        }

        public static string GetNounPlural(string s, int count)
        {
            return $"{s}{(count < 2 ? "" : "s")}";
        }

        public static UserInfo GetUserInfoFromContext(MiunieCommandContext context)
        {
            var user = context.User as Discord.WebSocket.SocketGuildUser;
            var roleIds = user.Guild.Roles.Select(r => r.Id).ToArray();
            return new UserInfo(user.Id, roleIds);
        }
    }
}
