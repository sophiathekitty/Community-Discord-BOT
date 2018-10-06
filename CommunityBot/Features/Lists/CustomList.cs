using System;
using System.Collections.Generic;
using System.Text;
using CommunityBot.Configuration;
using System.IO;
using Discord.WebSocket;
using System.Linq;
using Discord;
using static CommunityBot.Helpers.ListHelper;

namespace CommunityBot.Features.Lists
{
    public class CustomList
    {
        
        

        //private readonly ulong RoleEveryoneId;

        public string Name { get; set; }
        public List<String> Contents { get; set; }
        public ulong OwnerId { get; set; }
        //public ListPermission Permission { get; set; }
        public readonly Dictionary<ulong, ListPermission> PermissionByRole = new Dictionary<ulong, ListPermission>();

        private IDataStorage dataStorage;
        //public IDataStorage DataStorage
        //{
        //    get { return DataStorage; }
        //    set { DataStorage = DataStorage ?? value; }
        //}
        //private readonly DiscordSocketClient client;

        public CustomList(IDataStorage dataStorage, UserInfo userInfo, ListPermission permission, String name)
        {
            this.dataStorage = dataStorage;
            this.OwnerId = userInfo.Id;
            //this.Permission = permission;
            this.Name = name;

            //RoleEveryone = this.client.Guilds.FirstOrDefault().EveryoneRole.Name;
            if (userInfo.RoleIds != null)
            {
                //RoleEveryoneId = userInfo.RoleIds.First();
                PermissionByRole.Add(userInfo.RoleIds.First(), permission);
            }
            Contents = new List<string>();
        }

        public void SetDataStorage(IDataStorage dataStorage)
        {
            this.dataStorage = dataStorage;
        }

        //public IReadOnlyCollection<IRole> GetUserRoles(ulong userId)
        //{
        //    var user = Global.Client.Guilds.FirstOrDefault().GetUser(userId);
        //    return (user as IGuildUser).Guild.Roles;
        //}

        public bool SetPermissionByRole(ulong roleId, ListPermission permission)
        {
            if (PermissionByRole.ContainsKey(roleId))
            {
                if (PermissionByRole[roleId] == permission) { return false; }

                PermissionByRole[roleId] = permission;
            }
            else
            {
                PermissionByRole.Add(roleId, permission);
            }
            SaveList();
            return true;
        }

        public bool RemovePermissionByRole(ulong roleId)
        {
            if (!PermissionByRole.ContainsKey(roleId)) { return false; }
            PermissionByRole.Remove(roleId);
            SaveList();
            return true;
        }

        public ListPermission GetPermissionByRole(ulong roleId)
        {
            if (!PermissionByRole.ContainsKey(roleId)) { return ListPermission.PRIVATE; }
            return PermissionByRole[roleId];
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
            this.dataStorage.StoreObject(this, $"{this.Name}.json");
        }
        
        public static CustomList RestoreList(IDataStorage dataStorage, string name)
        {
            return dataStorage.RestoreObject<CustomList>($"{name}.json");
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

        public bool IsAllowedToList(UserInfo userInfo)
        {
            return ( !(this.OwnerId != userInfo.Id && PermissionByRole.First().Value == ListPermission.PRIVATE) );
        }

        public bool IsAllowedToRead(UserInfo userInfo)
        {
            //var userRoleNames = GetUserRoles(userId).Select(x => x.Name);
            var validRoleIds = PermissionByRole
                .Where(x => x.Value > ListPermission.PRIVATE && x.Value < ListPermission.LIST)
                .Select(x => x.Key);

            return (ShareItem(userInfo.RoleIds, validRoleIds) || this.OwnerId == userInfo.Id);
        }

        public bool IsAllowedToWrite(UserInfo userInfo)
        {
            //var userRoleNames = GetUserRoles(userId).Select(x => x.Name);
            var validRoleIds = PermissionByRole
                .Where(x => x.Value > ListPermission.PRIVATE && x.Value < ListPermission.READ)
                .Select(x => x.Key);

            return (ShareItem(userInfo.RoleIds, validRoleIds) || this.OwnerId == userInfo.Id);
        }

        public bool IsAllowedToModify(UserInfo userInfo)
        {
            return (userInfo.Id == this.OwnerId);
        }

        private bool ShareItem(IEnumerable<ulong> a, IEnumerable<ulong> b)
        {
            foreach(ulong l in a)
            {
                if (b.Contains(l)) { return true; }
            }
            return false;
        }
    }
}
