using System;
using System.Collections.Generic;
using System.Text;
using CommunityBot.Configuration;
using System.IO;

namespace CommunityBot.Features.Lists
{
    public class CustomList
    {
        public String name { get; set; }
        public List<String> contents { get; set; }

        public CustomList(String name)
        {
            this.name = name;
            contents = new List<string>();
        }

        public void Add(String item)
        {
            contents.Add(item);
            WriteContents();
        }

        public void AddRange(String[] collection)
        {
            contents.AddRange(collection);
            WriteContents();
        }

        public void Insert(int index, String item)
        {
            contents.Insert(index, item);
            WriteContents();
        }

        public void InsertRange(int index, String[] collection)
        {
            contents.InsertRange(index, collection);
            WriteContents();
        }

        public void Remove(String item)
        {
            contents.Remove(item);
            WriteContents();
        }

        public void Clear()
        {
            contents.Clear();
            WriteContents();
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

        public void WriteContents()
        {
            DataStorage.StoreObject(contents, this.name + ".json", false);
        }

        public List<String> ReadContents()
        {
            var list = DataStorage.RestoreObject<List<String>>(this.name + ".json");
            if (list == null) { return null; }

            this.contents = list;
            return this.contents;
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
    }
}
