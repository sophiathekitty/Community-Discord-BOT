using System;
using System.Collections.Generic;
using System.Text;
using CommunityBot.Configuration;

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
        }

        public void AddRange(String[] collection)
        {
            contents.AddRange(collection);
        }

        public void Insert(int index, String item)
        {
            contents.Insert(index, item);
        }

        public void InsertRange(int index, String[] collection)
        {
            contents.InsertRange(index, collection);
        }

        public void Remove(String item)
        {
            contents.Remove(item);
        }

        public void Clear()
        {
            contents.Clear();
        }

        public int Count()
        {
            return contents.Count;
        }

        public void WriteContents()
        {
            DataStorage.StoreObject(contents, this.name + ".txt", false);
        }

        public List<String> ReadContents()
        {
            var list = DataStorage.RestoreObject<List<String>>(this.name + ".txt");
            foreach(String s in list)
            {
                Console.WriteLine(s);
            }
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
