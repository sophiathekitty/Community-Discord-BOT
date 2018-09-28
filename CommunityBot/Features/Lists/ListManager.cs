using System;
using System.Collections.Generic;
using System.Text;
using CommunityBot.Configuration;
using System.Linq;

namespace CommunityBot.Features.Lists
{
    public static class ListManager
    {
        private static readonly string ListManagerLookup = "list_manager_lookup.json";
        public static readonly string WrongInputErrorMsg = "Wrong input";
        public static readonly string StdErrorMsg = "Oops, something went wrong";
        public static readonly string UnknownCommandErrorMsg = "Unknown command";

        private static readonly Dictionary<string, Func<string[], string>> validOperations = new Dictionary<string, Func<string[], string>>
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

        public static string Manage(params string[] input)
        {
            var sa = SeperateArray(input, 0);
            string command = sa.seperated;
            string[] values = sa.array;

            var result = "";
            try
            {
                result = validOperations[command](values);
            }
            catch (KeyNotFoundException)
            {
                throw GetListManagerException(ListErrorMessage.UnknownCommand_command, command);
            }
            return result;
        }

        public static string GetAll(params string[] input)
        {
            if (lists.Count == 0) { throw GetListManagerException(ListErrorMessage.NoLists); }
            var output = new StringBuilder();
            output.Append("+--------------------\n");
            foreach (CustomList l in lists)
            {
                output.Append(" |\t");
                output.Append(l.name);
                output.Append("\t|\t");
                int count = l.Count();
                output.Append(count);
                output.Append(string.Format(" item{0}\n", count == 1 ? "" : "s"));
            }
            output.Append("+--------------------");
            return output.ToString();
        }

        public static string CreateList(params string[] input)
        {
            if (input.Length != 1)
            {
                throw GetListManagerException(ListErrorMessage.WrongFormat);
            }
            try
            {
                GetList(input[0]);
            }
            catch (ListManagerException)
            {
                lists.Add(new CustomList(input[0]));

                WriteContents();

                return $"Created list '{input[0]}'";
            }
            throw GetListManagerException(ListErrorMessage.ListAlreadyExists_list, input[0]);
            //return "";
            //throw new ListManagerException(ListErrorMessage.ListAlreadyExists_list);
        }

        public static CustomList GetList(params string[] input)
        {
            CustomList list = lists.Find(l => l.name.Equals(input[0]));
            if (list == null)
            {
                throw GetListManagerException(ListErrorMessage.ListDoesNotExist_list, input[0]);
            }
            return list;
        }

        public static string Add(string[] input)
        {
            if (input.Length < 2)
            {
                throw GetListManagerException(ListErrorMessage.WrongFormat);
            }

            var sa = SeperateArray(input);
            string name = sa.seperated;
            string[] values = sa.array;

            GetList(name).AddRange(values);
            
            return $"Added {GetNounPlural("item", (values.Length))}";
        }

        public static string Insert(string[] input)
        {
            if (input.Length < 3) { throw GetListManagerException(); }

            var sa = SeperateArray(input);
            string name = sa.seperated;
            string[] values = sa.array;

            sa = SeperateArray(values, 0);
            string indexstring = sa.seperated;
            values = sa.array;

            int index = 0;
            try
            {
                index = int.Parse(indexstring);
            }
            catch (FormatException e)
            {
                throw GetListManagerException();
            }
            GetList(name).InsertRange(index, values);
            return $"Inserted {GetNounPlural("value", values.Length)}";
        }

        public static string RemoveList(params string[] input)
        {
            if (input.Length != 1)
            {
                throw GetListManagerException(ListErrorMessage.WrongFormat);
            }

            CustomList l = GetList(input[0]);
            l.Delete();
            lists.Remove(l);

            WriteContents();

            return $"Removed list '{input[0]}'";
        }

        public static string Remove(string[] input)
        {
            if (input.Length != 2)
            {
                throw GetListManagerException(ListErrorMessage.WrongFormat);
            }

            var sa = SeperateArray(input);
            string name = sa.seperated;
            string[] values = sa.array;

            GetList(name).Remove(values[0]);

            return $"Removed '{values[0]}' from the list";
        }

        public static string OutputList(string[] input)
        {
            if (input.Length != 1)
            {
                throw GetListManagerException(ListErrorMessage.WrongFormat);
            }

            CustomList list = GetList(input[0]);

            if (list.Count() == 0)
            {
                throw GetListManagerException(ListErrorMessage.ListIsEmpty_list, input[0]);
            }

            StringBuilder output = new StringBuilder();
            output.Append("+--------------------\n");
            for (int i = 0; i < list.contents.Count; i++)
            {
                output.Append(" | ");
                output.Append((i + 1));
                output.Append(":\t'");
                output.Append(list.contents[i]);
                output.Append("'\n");
            }
            output.Append("+--------------------");
            return output.ToString();
        }

        public static string Clear(string[] input)
        {
            if (input.Length != 1)
            {
                throw GetListManagerException(ListErrorMessage.WrongFormat);
            }

            GetList(input[0]).Clear();

            return $"Cleared list '{input[0]}'";
        }

        private static SeperatedArray SeperateArray(string[] input)
        {
            return SeperateArray(input, input.Length - 1);
        }

        private static SeperatedArray SeperateArray(string[] input, int index)
        {
            var sa = new SeperatedArray();
            sa.seperated = input[index];
            sa.array = new string[input.Length - 1];
            for (int i = 0; i < input.Length; i++)
            {
                if (i < index)
                {
                    sa.array[i] = input[i];
                }
                else if (i > index)
                {
                    sa.array[i - 1] = input[i];
                }
            }
            return sa;
        }

        private struct SeperatedArray
        {
            public string seperated { get; set; }
            public string[] array { get; set; }
        }

        private static String GetNounPlural(string s, int count)
        {
            return $"{s}{(count < 2 ? "" : "s")}";
        }

        public static void WriteContents()
        {
            List<string> listNames = lists.Select(l => l.name).ToList<string>();
            DataStorage.StoreObject(listNames, ListManagerLookup, false);
        }

        public static List<CustomList> ReadContents()
        {
            var listNames = DataStorage.RestoreObject<List<string>>(ListManagerLookup);
            if (listNames != null)
            {
                lists = new List<CustomList>();
                foreach (string s in listNames)
                {
                    CustomList l = new CustomList(s);
                    l.ReadContents();
                    lists.Add(l);
                }
                return lists;
            }
            return null;
        }

        public static ListManagerException GetListManagerException()
        {
            return GetListManagerException(ListErrorMessage.UnknownError);
        }

        public static ListManagerException GetListManagerException(string message, params string[] parameters)
        {
            message = String.Format(message, parameters);
            return new ListManagerException(message);
        }
    }
}
