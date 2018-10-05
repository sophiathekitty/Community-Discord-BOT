using System;
using System.Collections.Generic;
using System.Text;
using CommunityBot.Configuration;
using System.IO;
using Discord.WebSocket;
using System.Linq;
using Discord;

namespace CommunityBot.Features.Lists
{
    public class CustomList
    {
        public enum ListPermission
        {
            PRIVATE,
            PUBLIC,
            READ,
            LIST
        };

        public static IReadOnlyList<String> permissionStrings = new List<String>
        {
            "private",
            "public",
            "read only",
            "view only"
        };

        public static readonly IReadOnlyDictionary<string, ListPermission> validPermissions = new Dictionary<string, ListPermission>
        {
            { "-p", ListPermission.PRIVATE },
            { "-pu", ListPermission.PUBLIC },
            { "-r", ListPermission.READ },
            { "-l", ListPermission.LIST }
        };

        private static string RoleEveryone = Global.Client.Guilds.FirstOrDefault().EveryoneRole.Name;

        public string Name { get; set; }
        public List<String> Contents { get; set; }
        public ulong OwnerId { get; set; }
        public ListPermission Permission { get; set; }
        public readonly Dictionary<string, ListPermission> PermissionByRole = new Dictionary<string, ListPermission>();

        public CustomList(ulong ownerId, ListPermission permission, String name)
        {
            this.OwnerId = ownerId;
            PermissionByRole.Add(RoleEveryone, permission);
            this.Permission = permission;
            this.Name = name;
            Contents = new List<string>();
        }

        public IReadOnlyCollection<IRole> GetUserRoles(ulong userId)
        {
            var user = Global.Client.Guilds.FirstOrDefault().GetUser(userId);
            return (user as IGuildUser).Guild.Roles;
        }

        public void SetPermissionByRole(string role, ListPermission permission)
        {
            if (PermissionByRole.ContainsKey(role))
            {
                PermissionByRole[role] = permission;
            }
            PermissionByRole.Add(role, permission);
        }

        public void RemovePermissionByRole(string role)
        {
            if (!PermissionByRole.ContainsKey(role)) { return; }
            PermissionByRole.Remove(role);
        }

        public ListPermission GetPermissionByRole(string role)
        {
            if (!PermissionByRole.ContainsKey(role)) { return ListPermission.PRIVATE; }
            return PermissionByRole[role];
        }

        public void Add(String item)
        {
            Contents.Add(item);
            SaveList();
        }

        public void AddRange(String[] collection)
        {
            Contents.AddRange(collection);
            SaveList();
        }

        public void Insert(int index, String item)
        {
            Contents.Insert(index, item);
            SaveList();
        }

        public void InsertRange(int index, String[] collection)
        {
            Contents.InsertRange(index, collection);
            SaveList();
        }

        public void Remove(String item)
        {
            Contents.Remove(item);
            SaveList();
        }

        public void Clear()
        {
            Contents.Clear();
            SaveList();
        }

        public int Count()
        {
            return Contents.Count;
        }

        public void Delete()
        {
            string resourceFolder = Constants.ResourceFolder;
            var path = String.Concat(resourceFolder, "/", this.Name, ".json");
            if (!File.Exists(path)) { return; }
            File.Delete(path);
        }

        public void SaveList()
        {
            InversionOfControl.Container.GetInstance<JsonDataStorage>().StoreObject(this, $"{this.Name}.json", false);
        }
        
        public static CustomList RestoreList(string name)
        {
            return InversionOfControl.Container.GetInstance<JsonDataStorage>().RestoreObject<CustomList>($"{name}.json");
        }

        public bool EqualContents(List<String> list)
        {
            if (this.Contents.Count != list.Count) { return false; }
            for (int i=0; i<this.Contents.Count; i++)
            {
                if (!this.Contents[i].Equals(list[i])) { return false; }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CustomList)) { return false; }
            CustomList comp = (CustomList)obj;
            return (EqualContents(comp.Contents) && comp.Name.Equals(this.Name));
        }

        public bool IsAllowedToList(ulong userId)
        {
            return ( !(this.OwnerId != userId && GetPermissionByRole(RoleEveryone) == ListPermission.PRIVATE) );
        }

        public bool IsAllowedToRead(ulong userId)
        {
            var userRoleNames = GetUserRoles(userId).Select(x => x.Name);
            var validRoleNames = PermissionByRole
                .Where(x => x.Value > ListPermission.PRIVATE && x.Value < ListPermission.LIST)
                .Select(x => x.Key);

            return (ShareItem(userRoleNames, validRoleNames) || this.OwnerId == userId);
        }

        public bool IsAllowedToWrite(ulong userId)
        {
            var userRoleNames = GetUserRoles(userId).Select(x => x.Name);
            var validRoleNames = PermissionByRole
                .Where(x => x.Value > ListPermission.PRIVATE && x.Value < ListPermission.READ)
                .Select(x => x.Key);

            return (ShareItem(userRoleNames, validRoleNames) || this.OwnerId == userId);
        }

        private bool ShareItem(IEnumerable<string> a, IEnumerable<string> b)
        {
            foreach(string s in a)
            {
                if (b.Contains(s)) { return true; }
            }
            return false;
        }
    }
}
