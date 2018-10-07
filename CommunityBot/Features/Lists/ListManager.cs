using System;
using System.Collections.Generic;
using System.Text;
using CommunityBot.Configuration;
using System.Linq;
using Discord;
using CommunityBot.Helpers;
using System.Threading.Tasks;
using static CommunityBot.Features.Lists.ListException;
using static CommunityBot.Helpers.ListHelper;
using CommunityBot.Extensions;
using Discord.WebSocket;

namespace CommunityBot.Features.Lists
{
    public class ListManager
    {
        private static readonly string ListManagerLookup = "list_manager_lookup.json";

        public static readonly string LineIndicator = " <--";

        public static Dictionary<ulong, ulong> ListenForReactionMessages = new Dictionary<ulong, ulong>();

        public static readonly IReadOnlyDictionary<string, Emoji> ControlEmojis = new Dictionary<string, Emoji>
        {
            {"up", new Emoji("⬆") },
            {"down", new Emoji("⬇") },
            {"check", new Emoji("✅") }
        };

        private static IReadOnlyDictionary<string, Func<UserInfo, string[], ListOutput>> ValidOperations;

        private static List<CustomList> Lists = new List<CustomList>();
        private readonly IDataStorage dataStorage;
        private readonly DiscordSocketClient client;

        public ListManager(DiscordSocketClient client, IDataStorage dataStorage)
        {
            this.client = client;
            this.dataStorage = dataStorage;

            ValidOperations = new Dictionary<string, Func<UserInfo, string[], ListOutput>>  //Func<UserInfo, string[], ListOutput>
            {
                { "-m", ModifyPermission },
                { "-g", (userInfo, args) => GetAllPrivate(userInfo) },
                { "-gp", (userInfo, args) => GetAllPublic(userInfo) },
                { "-c", CreateListPrivate },
                { "-cp", CreateListPublic },
                { "-a", Add },
                { "-i", Insert },
                { "-l", OutputListPrivate },
                { "-lp", OutputListPublic },
                { "-r", Remove },
                { "-rl", RemoveList },
                { "-cl", Clear }
            };

            RestoreOrCreateLists();
        }

        public ListOutput HandleIO(UserInfo userInfo, ulong messageId, params string[] input)
        {
            ListOutput output;
            try
            {
                output = Manage(userInfo, input);
            }
            catch (ListManagerException e)
            {
                output = GetListOutput(e.Message, ListPermission.PUBLIC);
            }
            catch (ListPermissionException e)
            {
                output = GetListOutput(e.Message, ListPermission.PUBLIC);
            }
            if (output.listenForReactions)
            {
                ListManager.ListenForReactionMessages.Add(messageId, userInfo.Id);
            }
            return output;
        }

        public ListOutput Manage(UserInfo userInfo, params string[] input)
        {
            var sa = SeperateArray(input, 0);
            string command = sa.seperated[0];
            string[] values = sa.array;

            ListOutput result;
            try
            {
                result = ValidOperations[command](userInfo, values);
            }
            catch (KeyNotFoundException)
            {
                throw GetListManagerException(ListErrorMessage.General.UnknownCommand_command, command);
            }
            return result;
        }

        public ListOutput ModifyPermission(UserInfo userInfo, params string[] input)
        {
            if (input.Length < 3 || input.Length % 2 == 0) { throw GetListManagerException(ListErrorMessage.General.WrongFormat); }

            var sa = SeperateArray(input, 0, 1, 2);
            var log = new StringBuilder();
            for (int i=0;i<input.Length; i+=3)
            {
                var listName    = sa.seperated[i+0];
                var roleName    = sa.seperated[i+1];
                var modifier    = sa.seperated[i+2];

                var list = GetList(listName);
                CheckPermissionModify(userInfo, list);

                var role = client.Guilds.First().Roles.Where(r => r.Name.Equals(roleName)).FirstOrDefault();
                if (role == default(SocketRole))
                {
                    throw GetListManagerException(ListErrorMessage.General.RoleDoesNotExist_rolename, roleName);
                }

                var permission = ValidPermissions[modifier];
                if (permission == null) { throw GetListManagerException(ListErrorMessage.General.WrongFormat); }

                bool result = list.SetPermissionByRole(role.Id, permission);
                var resultString = "";
                if (result)
                {
                    resultString = $"Changed permission of role '{roleName}' to {PermissionStrings[(int)permission]}";
                }
                else
                {
                    resultString = $"The permission of role '{roleName}' is already set to {PermissionStrings[(int)permission]}";
                }
                log.Append(resultString);
            }
            return GetListOutput(log.ToString());
        }

        public ListOutput GetAllPrivate(UserInfo userInfo)
        {
            return GetAll(userInfo, ListPermission.PRIVATE);
        }

        public ListOutput GetAllPublic(UserInfo userInfo)
        {
            return GetAll(userInfo, ListPermission.PUBLIC);
        }

        private ListOutput GetAll(UserInfo userInfo, ListPermission outputPermission)
        {
            if (Lists.Count == 0) { throw GetListManagerException(ListErrorMessage.General.NoLists); }

            Func<int, int, int> GetLonger = (i1, i2) => { return (i1 > i2 ? i1 : i2); };

            var tableValuesList = new Dictionary<String, String>();

            for (int i = 0; i < Lists.Count; i++)
            {
                CustomList l = Lists[i];
                if (l.IsAllowedToList(userInfo))
                {
                    tableValuesList.Add(l.Name, $"{l.Count()} {GetNounPlural("item", l.Count())}");
                }
            }
            var maxItemLength = 0;
            var tableValues = new string[tableValuesList.Count,2];
            for (int i=0; i<tableValues.GetLength(0); i++)
            {
                var keyPair = tableValuesList.ElementAt(i);
                tableValues[i, 0] = keyPair.Key;
                tableValues[i, 1] = keyPair.Value;
                maxItemLength = GetLonger(maxItemLength, keyPair.Key.Length);
                maxItemLength = GetLonger(maxItemLength, keyPair.Value.Length);
            }

            var header = new[] { "List name", "Item count" };
            foreach (string s in header)
            {
                maxItemLength = GetLonger(maxItemLength, s.Length);
            }
            var tableSettings = new MessageFormater.TableSettings("All lists", header, -(maxItemLength), true);
            string output = MessageFormater.CreateTable(tableSettings, tableValues);
            var count = 0;
            for (int i=0; i<output.Length; i++)
            {
                if (output[i] == '\n')
                {
                    if (count == 8)
                    {
                        output = output.Insert(i, LineIndicator);
                        break;
                    }
                    else
                    {
                        count++;
                    }
                }
            }

            var returnValue = GetListOutput(output, outputPermission);
            returnValue.listenForReactions = true;
            return returnValue;
        }

        public ListOutput CreateListPrivate(UserInfo userInfo, params string[] input)
        {
            return CreateList(userInfo, ListPermission.PRIVATE, input);
        }

        public ListOutput CreateListPublic(UserInfo userInfo, params string[] input)
        {
            return CreateList(userInfo, ListPermission.PUBLIC, input);
        }

        private ListOutput CreateList(UserInfo userInfo, ListPermission listPermissions, params string[] input)
        {
            if (input.Length == 0 || input.Length > 2)
            {
                throw GetListManagerException(ListErrorMessage.General.WrongFormat);
            }
            var listName = input[0];

            try
            {
                GetList(listName);
            }
            catch (ListManagerException)
            {
                var newList = new CustomList(dataStorage, userInfo, listPermissions, listName);
                newList.SaveList();

                Lists.Add(newList);

                WriteContents();

                var permissionAsString = PermissionStrings[(int)listPermissions];
                return GetListOutput($"Created {permissionAsString} list '{listName}'", listPermissions);
            }
            throw GetListManagerException(ListErrorMessage.General.ListAlreadyExists_list, listName);
        }

        public CustomList GetList(string name)
        {
            CustomList list = Lists.Find(l => l.Name.Equals(name));
            if (list == null)
            {
                throw GetListManagerException(ListErrorMessage.General.ListDoesNotExist_list, name);
            }
            return list;
        }

        public ListOutput Add(UserInfo userInfo, string[] input)
        {
            if (input.Length < 2)
            {
                throw GetListManagerException(ListErrorMessage.General.WrongFormat);
            }

            var sa = SeperateArray(input);
            string name = sa.seperated[0];
            string[] values = sa.array;

            var list = GetList(name);
            CheckPermissionWrite(userInfo, list);

            list.AddRange(values);

            string output = $"Added {GetNounPlural("item", (values.Length))}";
            return GetListOutput(output);
        }

        public ListOutput Insert(UserInfo userInfo, string[] input)
        {
            if (input.Length < 3) { throw GetListManagerException(); }

            var sa = SeperateArray(input);
            string name = sa.seperated[0];
            string[] values = sa.array;

            sa = SeperateArray(values, 0);
            string indexstring = sa.seperated[0];
            values = sa.array;

            int index = 0;
            try
            {
                index = int.Parse(indexstring) - 1;
            }
            catch (FormatException e)
            {
                throw GetListManagerException(ListErrorMessage.General.WrongInputForIndex);
            }

            var list = GetList(name);
            CheckPermissionWrite(userInfo, list);

            if (index < 0 || index > list.Count())
            {
                throw GetListManagerException(ListErrorMessage.General.IndexOutOfBounds_list, list.Name);
            }

            list.InsertRange(index, values);

            string output = $"Inserted {GetNounPlural("value", values.Length)}";
            return GetListOutput(output);
        }

        public ListOutput RemoveList(UserInfo userInfo, params string[] input)
        {
            if (input.Length != 1)
            {
                throw GetListManagerException(ListErrorMessage.General.WrongFormat);
            }

            CustomList list = GetList(input[0]);
            CheckPermissionWrite(userInfo, list);

            list.Delete();
            Lists.Remove(list);

            WriteContents();

            string output = $"Removed list '{input[0]}'";
            return GetListOutput(output);
        }

        public ListOutput Remove(UserInfo userInfo, string[] input)
        {
            if (input.Length != 2)
            {
                throw GetListManagerException(ListErrorMessage.General.WrongFormat);
            }

            var sa = SeperateArray(input);
            string name = sa.seperated[0];
            string[] values = sa.array;

            var list = GetList(name);
            CheckPermissionWrite(userInfo, list);

            list.Remove(values[0]);

            string output = $"Removed '{values[0]}' from the list";
            return GetListOutput(output);
        }

        public ListOutput OutputListPrivate(UserInfo userInfo, params string[] input)
        {
            return OutputList(userInfo, ListPermission.PRIVATE, input);
        }

        public ListOutput OutputListPublic(UserInfo userInfo, params string[] input)
        {
            return OutputList(userInfo, ListPermission.PUBLIC, input);
        }

        private ListOutput OutputList(UserInfo userInfo, ListPermission permission, params string[] input)
        {
            if (input.Length != 1)
            {
                throw GetListManagerException(ListErrorMessage.General.WrongFormat);
            }

            CustomList list = GetList(input[0]);
            
            CheckPermissionRead(userInfo, list);

            if (list.Count() == 0)
            {
                throw GetListManagerException(ListErrorMessage.General.ListIsEmpty_list, input[0]);
            }

            Func<int, int, int> GetLonger = (i1, i2) => { return (i1 > i2 ? i1 : i2); };

            var maxItemLength = 0;
            var values = new string[list.Count(), 1];
            for (int i = 0; i < list.Count(); i++)
            {
                var row = $"{(i+1).ToString()}: {list.Contents[i]}";
                values[i, 0] = row;
                maxItemLength = GetLonger(maxItemLength, row.Length);
            }
            maxItemLength = GetLonger(maxItemLength, list.Name.Length);

            var tableSettings = new MessageFormater.TableSettings(list.Name, -(maxItemLength), false);
            string output = MessageFormater.CreateTable(tableSettings, values);

            var outputPermission = permission == ListPermission.PRIVATE ? list.PermissionByRole.First().Value : permission;
            return GetListOutput(output, outputPermission);
        }

        public ListOutput Clear(UserInfo userInfo, string[] input)
        {
            if (input.Length != 1)
            {
                throw GetListManagerException(ListErrorMessage.General.WrongFormat);
            }

            var list = GetList(input[0]);
            CheckPermissionWrite(userInfo, list);

            list.Clear();

            string output = $"Cleared list '{input[0]}'";
            return GetListOutput(output);
        }

        private SeperatedArray SeperateArray(string[] input)
        {
            return SeperateArray(input, input.Length - 1);
        }

        private  SeperatedArray SeperateArray(string[] input, params int[] indices)
        {
            var sa = new SeperatedArray();
            sa.seperated = new string[indices.Length];
            for (int i=0; i<indices.Length; i++)
            {
                sa.seperated[i] = input[indices[i]];
            }
            sa.array = new string[input.Length - indices.Length];
            var currentIndex = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (!indices.Contains(i))
                {
                    sa.array[currentIndex++] = input[i];
                }
            }
            return sa;
        }

        private struct SeperatedArray : IEquatable<object>
        {
            public string[] seperated { get; set; }
            public string[] array { get; set; }
        }

        public void WriteContents()
        {
            List<string> listNames = Lists.Select(l => l.Name).ToList<string>();
            dataStorage.StoreObject(listNames, ListManagerLookup);
        }

        public void RestoreOrCreateLists()
        {
            Lists = new List<CustomList>();

            var listNames = dataStorage.RestoreObject<List<string>>(ListManagerLookup);
            if (listNames == null) { return; }
            
            foreach (string name in listNames)
            {
                var l = CustomList.RestoreList(dataStorage, name);
                if (l != null)
                {
                    l.SetDataStorage(this.dataStorage);
                    Lists.Add(l);
                }
            }
        }

        public ListManagerException GetListManagerException()
        {
            return GetListManagerException(ListErrorMessage.General.UnknownError);
        }

        public ListManagerException GetListManagerException(string message, params string[] parameters)
        {
            var formattedMessage = String.Format(message, parameters);
            return new ListManagerException(formattedMessage);
        }

        public ListPermissionException GetListPermissionException()
        {
            return GetListPermissionException(ListErrorMessage.Permission.NoPermission_list);
        }

        public ListPermissionException GetListPermissionException(string message, params string[] parameters)
        {
            var formattedMessage = String.Format(message, parameters);
            return new ListPermissionException(formattedMessage);
        }

        private void CheckPermissionList(UserInfo userInfo, CustomList list)
        {
            if (!list.IsAllowedToList(userInfo))
            {
                throw GetListManagerException(ListErrorMessage.Permission.NoPermission_list, list.Name);
            }
        }

        private void CheckPermissionRead(UserInfo userInfo, CustomList list)
        {
            if (!list.IsAllowedToRead(userInfo))
            {
                throw GetListManagerException(ListErrorMessage.Permission.NoPermission_list, list.Name);
            }
        }

        private void CheckPermissionWrite(UserInfo userInfo, CustomList list)
        {
            if (!list.IsAllowedToWrite(userInfo))
            {
                throw GetListManagerException(ListErrorMessage.Permission.NoPermission_list, list.Name);
            }
        }

        private void CheckPermissionModify(UserInfo userInfo, CustomList list)
        {
            if (!list.IsAllowedToModify(userInfo))
            {
                throw GetListManagerException(ListErrorMessage.Permission.NoPermission_list, list.Name);
            }
        }
    }
}
