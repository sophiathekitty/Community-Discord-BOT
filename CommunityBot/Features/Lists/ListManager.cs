using System;
using System.Collections.Generic;
using System.Text;
using CommunityBot.Configuration;
using System.Linq;

namespace CommunityBot.Features.Lists
{
    public static class ListManager
    {
        private static readonly String listManagerLookup = "list_manager_lookup.json";
        public static readonly String wrongInputErrorMsg = "Wrong input";
        public static readonly String stdErrorMsg = "Oops, something went wrong";
        public static readonly String unknownCommandErrorMsg = "Unknown command";

        private static readonly Dictionary<String, Func<String[], String>> validOperations = new Dictionary<String, Func<String[], String>>
        {
            { "-g", GetAll },
            { "-c", CreateList },
            { "-a", Add },
            { "-i", Insert },
            { "-l", OutputList },
            { "-r", Remove },
            { "-rl", RemoveList },
            { "-cl", Clear }
        };

        private static List<CustomList> lists = new List<CustomList>();

        static ListManager()
        {
            ReadContents();
            if (lists == null)
            {
                lists = new List<CustomList>();
            }
        }

        public static String Manage(params String[] input)
        {
            SplitArray(input, 0, out String command, out String[] values);
            var result = "";
            try
            {
                result = validOperations[command](values);
            }
            catch (KeyNotFoundException e)
            {
                return unknownCommandErrorMsg;
            }
            return result;
        }

        public static String GetAll(params String[] input)
        {
            var output = new StringBuilder();
            output.Append("+--------------------\n");
            foreach (CustomList l in lists)
            {
                output.Append(" |\t");
                output.Append(l.name);
                output.Append("\t|\t");
                int count = l.Count();
                output.Append(count);
                output.Append(String.Format(" item{0}\n", count == 1 ? "" : "s"));
            }
            output.Append("+--------------------");
            return output.ToString();
        }

        public static String CreateList(params String[] input)
        {
            if (input.Length != 1) { return stdErrorMsg; }
            if (GetList(input[0]) != null) { return $"List {input[0]} already exists"; }

            lists.Add(new CustomList(input[0]));

            WriteContents();

            return $"Created list '{input[0]}'";
        }

        public static CustomList GetList(params String[] input)
        {
            return lists.Find(l => l.name.Equals(input[0]));
        }

        public static String Add(String[] input)
        {
            if (input.Length < 2) { return stdErrorMsg; }

            SplitArray(input, out String name, out String[] values);

            GetList(name).AddRange(values);

            return "Added item" + (input.Length > 2 ? "s" : "");
        }

        public static String Insert(String[] input)
        {
            if (input.Length < 3) { return stdErrorMsg; }

            SplitArray(input, out String name, out String[] values);
            SplitArray(values, 0, out String indexString, out values);
            int index = 0;
            try
            {
                index = int.Parse(indexString);
            }
            catch (FormatException e)
            {
                return stdErrorMsg;
            }
            GetList(name).InsertRange(index, values);
            return "Inserted value" + (input.Length > 3 ? "s" : "");
        }

        public static String RemoveList(params String[] input)
        {
            if (input.Length != 1) { return stdErrorMsg; }

            CustomList l = GetList(input[0]);
            l.Delete();
            lists.Remove(l);

            WriteContents();

            return $"Removed list '{input[0]}'";
        }

        public static String Remove(String[] input)
        {
            if (input.Length != 2) { return stdErrorMsg; }

            SplitArray(input, out String name, out String[] values);

            GetList(name).Remove(values[0]);

            return $"Removed '{values[0]}' from the list";
        }

        public static String OutputList(String[] input)
        {
            if (input.Length != 1) { return stdErrorMsg; }

            CustomList list = GetList(input[0]);

            if (list == null) { return $"List {input[0]} doesn't exist"; }

            if (list.Count() == 0)
            {
                return $"The list '{input[0]}' is empty";
            }

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
            if (input.Length != 1) { return stdErrorMsg; }

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
            List<String> listNames = lists.Select(l => l.name).ToList<String>();
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
