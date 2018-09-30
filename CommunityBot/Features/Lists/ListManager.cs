using System;
using System.Collections.Generic;
using System.Text;
using CommunityBot.Configuration;
using System.Linq;
using Discord;
using CommunityBot.Helpers;

namespace CommunityBot.Features.Lists
{
    public static class ListManager
    {
        private static readonly string ListManagerLookup = "list_manager_lookup.json";
        public static readonly string WrongInputErrorMsg = "Wrong input";
        public static readonly string StdErrorMsg = "Oops, something went wrong";
        public static readonly string UnknownCommandErrorMsg = "Unknown command";

        private static readonly Dictionary<string, Func<string[], ListOutput>> validOperations = new Dictionary<string, Func<string[], ListOutput>>
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

        public static ListOutput Manage(params string[] input)
        {
            var sa = SeperateArray(input, 0);
            string command = sa.seperated;
            string[] values = sa.array;

            ListOutput result;
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

        public static ListOutput GetAll(params string[] input)
        {
            if (lists.Count == 0) { throw GetListManagerException(ListErrorMessage.NoLists); }
            var tableValues = new string[lists.Count, 2];

            for (int i = 0; i < lists.Count; i++)
            {
                CustomList l = lists[i];
                tableValues[i, 0] = l.name;
                tableValues[i, 1] = $"{l.Count()} {GetNounPlural("item", l.Count())}";
            }
            var header = new[] { "List name", "Item count" };
            var tableSettings = new MessageFormater.TableSettings("All lists", header, -12, true);
            string output = MessageFormater.CreateTable(tableSettings, tableValues);

            var eb = new EmbedBuilder();
            eb.AddField("lists", output);
            return GetListOutput(output);
        }

        public static ListOutput CreateList(params string[] input)
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

                return GetListOutput($"Created list '{input[0]}'");
            }
            throw GetListManagerException(ListErrorMessage.ListAlreadyExists_list, input[0]);
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

        public static ListOutput Add(string[] input)
        {
            if (input.Length < 2)
            {
                throw GetListManagerException(ListErrorMessage.WrongFormat);
            }

            var sa = SeperateArray(input);
            string name = sa.seperated;
            string[] values = sa.array;

            GetList(name).AddRange(values);

            string output = $"Added {GetNounPlural("item", (values.Length))}";
            return GetListOutput(output);
        }

        public static ListOutput Insert(string[] input)
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
            string output = $"Inserted {GetNounPlural("value", values.Length)}";
            return GetListOutput(output);
        }

        public static ListOutput RemoveList(params string[] input)
        {
            if (input.Length != 1)
            {
                throw GetListManagerException(ListErrorMessage.WrongFormat);
            }

            CustomList l = GetList(input[0]);
            l.Delete();
            lists.Remove(l);

            WriteContents();
            string output = $"Removed list '{input[0]}'";
            return GetListOutput(output);
        }

        public static ListOutput Remove(string[] input)
        {
            if (input.Length != 2)
            {
                throw GetListManagerException(ListErrorMessage.WrongFormat);
            }

            var sa = SeperateArray(input);
            string name = sa.seperated;
            string[] values = sa.array;

            GetList(name).Remove(values[0]);

            string output = $"Removed '{values[0]}' from the list";
            return GetListOutput(output);
        }

        public static ListOutput OutputList(string[] input)
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

            var values = new string[list.Count(), 1];
            for (int i = 0; i < list.Count(); i++)
            {
                values[i, 0] = $"{i.ToString()}: {list.contents[i]}";
            }

            var tableSettings = new MessageFormater.TableSettings(list.name, -12, false);
            string output = MessageFormater.CreateTable(tableSettings, values);

            return GetListOutput(output);
        }

        public static ListOutput Clear(string[] input)
        {
            if (input.Length != 1)
            {
                throw GetListManagerException(ListErrorMessage.WrongFormat);
            }

            GetList(input[0]).Clear();

            string output = $"Cleared list '{input[0]}'";
            return GetListOutput(output);
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

        public struct ListOutput
        {
            public string outputString { get; set; }
            public Embed outputEmbed { get; set; }
        }

        public static ListOutput GetListOutput(string s)
        {
            return new ListOutput { outputString = s };
        }

        public static ListOutput GetListOutput(Embed e)
        {
            return new ListOutput { outputEmbed = e };
        }

        private static string GetNounPlural(string s, int count)
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
            var formattedMessage = String.Format(message, parameters);
            return new ListManagerException(formattedMessage);
        }
    }
}
