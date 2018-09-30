using System;
using System.Collections.Generic;
using System.Text;
using CommunityBot.Configuration;
using System.IO;
using Discord.WebSocket;

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

        public static readonly List<String> permissionStrings = new List<String>()
        {
            "private",
            "public",
            "read only",
            "view only"
        };

        public static readonly Dictionary<string, ListPermission> validPermissions = new Dictionary<string, ListPermission>
        {
            { "-p", ListPermission.PRIVATE },
            { "-pu", ListPermission.PUBLIC },
            { "-r", ListPermission.READ },
            { "-l", ListPermission.LIST }
        };

        public String name { get; set; }
        public List<String> contents { get; set; }
        public ulong ownerId { get; set; }
        public ListPermission permission { get; set; }

        public CustomList(ulong ownerId, ListPermission permission, String name)
        {
            this.ownerId = ownerId;
            this.permission = permission;
            this.name = name;
            contents = new List<string>();
        }

        public void Add(String item)
        {
            contents.Add(item);
            SaveList();
        }

        public void AddRange(String[] collection)
        {
            contents.AddRange(collection);
            SaveList();
        }

        public void Insert(int index, String item)
        {
            contents.Insert(index, item);
            SaveList();
        }

        public void InsertRange(int index, String[] collection)
        {
            contents.InsertRange(index, collection);
            SaveList();
        }

        public void Remove(String item)
        {
            contents.Remove(item);
            SaveList();
        }

        public void Clear()
        {
            contents.Clear();
            SaveList();
        }

        public int Count()
        {
            return contents.Count;
        }

        public void Delete()
        {
            String resourceFolder = Constants.ResourceFolder;
            String path = String.Concat(resourceFolder, "/", this.name, ".json");
            if (!File.Exists(path)) { return; }
            File.Delete(path);
        }

        public void SaveList()
        {
            DataStorage.StoreObject(this, $"{this.name}.json", false);
        }
        
        public static CustomList RestoreList(string name)
        {
            return DataStorage.RestoreObject<CustomList>($"{name}.json");
        }

        public bool EqualContents(List<String> list)
        {
            if (this.contents.Count != list.Count) { return false; }
            for (int i=0; i<this.contents.Count; i++)
            {
                if (!this.contents[i].Equals(list[i])) { return false; }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CustomList)) { return false; }
            CustomList comp = (CustomList)obj;
            return (EqualContents(comp.contents) && comp.name.Equals(this.name));
        }

        public bool IsAllowedToList(ulong userId)
        {
            return (    this.ownerId == userId
                    ||  this.permission == CustomList.ListPermission.PUBLIC
                    ||  this.permission == CustomList.ListPermission.READ
                    ||  this.permission == CustomList.ListPermission.LIST );
        }

        public bool IsAllowedToRead(ulong userId)
        {
            return (    this.ownerId == userId
                    ||  this.permission == CustomList.ListPermission.PUBLIC
                    ||  this.permission == CustomList.ListPermission.READ );
        }

        public bool IsAllowedToWrite(ulong userId)
        {
            return (    this.ownerId == userId
                    ||  this.permission == CustomList.ListPermission.PUBLIC );
        }
    }
}
