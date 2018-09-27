using System;
using System.Collections.Generic;
using System.Text;
using CommunityBot.Configuration;
using System.Linq;

namespace CommunityBot.Features.Lists
{
    public class ListManager
    {
        private static readonly String listManagerLookup = "list_manager_lookup.txt";
        private static readonly String errorMsg = "Sorry, something went wrong";
        public static Dictionary<String, Func<String[], String>> validOperations = new Dictionary<String, Func<String[], String>>()
        {
            { "-c", CreateList },
            { "-a", Add },
            { "-l", OutputList },
            { "-r", Remove },
            { "-cl", Clear }
        };

        private static List<CustomList> lists = new List<CustomList>();

        public static ListManager()
        {
            lists = ReadContents();
        }

        public static String Manage(params String[] input)
        {
            SplitArray(input, 0, out String command, out String[] values);
            return validOperations[command](values);
        }

        public static String CreateList(params String[] input)
        {
            if (input.Length != 1) { return errorMsg; }
            lists.Add(new CustomList(input[0]));
            WriteContents();
            return $"Created list '{input[0]}'";
        }

        public static CustomList GetList(params String[] input)
        {
            return lists.Find(l => l.name.Equals(input[0]));
        }

        public static String RemoveList(params String[] input)
        {
            if (input.Length != 1) { return errorMsg; }
            lists.Remove(GetList(input[0]));
            return $"Removed list '{input[0]}'";
        }

        public static String Add(String[] input)
        {
            if ( input.Length < 2 ) { return errorMsg; }
            SplitArray(input, out String name, out String[] values);
            GetList(name).AddRange(values);
            return "Added item" + (input.Length > 2 ? " s" : "");
        }

        public static String Remove(String[] input)
        {
            if (input.Length != 2) { return errorMsg; }
            SplitArray(input, out String name, out String[] values);
            GetList(name).Remove(values[0]);
            return $"Removed '{values[0]}' from the list";
        }

        public static String OutputList(String[] input)
        {
            if (input.Length != 1) { return errorMsg; }
            CustomList list = GetList(input[0]);
            if (list == null) { return errorMsg; }
            StringBuilder output = new StringBuilder();
            output.Append("+--------------------\n");
            for (int i=0; i<list.contents.Count; i++)
            {
                output.Append(" | ");
                output.Append((i+1));
                output.Append(":\t'");
                output.Append(list.contents[i]);
                output.Append("'\n");
            }
            output.Append("+--------------------");
            return output.ToString();
        }

        public static String Clear(String[] input)
        {
            if (input.Length != 1) { return errorMsg; }
            GetList(input[0]).Clear();
            return $"Cleared list '{input[0]}'";
        }

        private static void SplitArray(String[] input, out String seperated, out String[] values)
        {
            SplitArray(input, input.Length-1, out seperated, out values);
        }

        private static void SplitArray(String[] input, int index, out String seperated, out String[] values)
        {
            seperated = input[index];
            values = new String[input.Length-1];
            for (int i=0;  i<input.Length; i++)
            {
                if (i < index)
                {
                    values[i] = input[i];
                }
                else if (i > index)
                {
                    values[i-1] = input[i];
                }
            }
        }

        public static void WriteContents()
        {
            if (lists.Count == 0) { return; }

            List<String> listNames = (List<String>)ReadContents().Select(l => l.name);
            if (listNames == null) { listNames = new List<string>(); }

            foreach (CustomList l in lists)
            {
                listNames.Add(l.name);
            }
            DataStorage.StoreObject(listNames, listManagerLookup, false);
        }

        public static List<CustomList> ReadContents()
        {
            var listNames = DataStorage.RestoreObject<List<String>>(listManagerLookup);
            if (listNames != null)
            {
                lists = new List<CustomList>();
                foreach (String s in listNames)
                {
                    CustomList l = new CustomList(s);
                    l.ReadContents();
                    lists.Add(l);
                }
                return lists;
            }
            return null;
        }
    }
}
