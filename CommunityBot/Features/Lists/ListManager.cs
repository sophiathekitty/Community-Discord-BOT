using System;
using System.Collections.Generic;
using System.Text;
using CommunityBot.Configuration;
using System.Linq;
using CommunityBot.Helpers;
using System.Threading.Tasks;
using static CommunityBot.Features.Lists.ListException;
using static CommunityBot.Helpers.ListHelper;
using CommunityBot.Extensions;
using Discord;

namespace CommunityBot.Features.Lists
{
    public class ListManager
    {
        private static readonly string ListManagerLookup = "list_manager_lookup.json";

        public static readonly string LineIndicator = " <--";

        public static Dictionary<ulong, ulong> ListenForReactionMessages = new Dictionary<ulong, ulong>();

        private static IReadOnlyList<ManagerMethod> validOperations;

        public static IReadOnlyList<ManagerMethod> ValidOperations
        {
            get
            {
                return validOperations;
            }

            set
            {
                if (validOperations == null)
                {
                    validOperations = value;
                }
            }
        }        

        private static List<CustomList> Lists = new List<CustomList>();
        private readonly IDataStorage dataStorage;

        public ListManager(IDataStorage dataStorage)
        {
            this.dataStorage = dataStorage;

            var index = 0;
            ValidOperations = new List<ManagerMethod>  //Func<UserInfo, string[], ListOutput>
            {
                new ManagerMethod("-m",     ManagerMethodId.MODIFY,         (userInfo, availableRoles, args) => ModifyPermission(userInfo, availableRoles, args)),
                new ManagerMethod("-g",     ManagerMethodId.GETPRIVATE,     (userInfo, availableRoles, args) => GetAllPrivate(userInfo)),
                new ManagerMethod("-gp",    ManagerMethodId.GETPUBLIC,      (userInfo, availableRoles, args) => GetAllPublic(userInfo)),
                new ManagerMethod("-c",     ManagerMethodId.CREATEPRIVATE,  (userInfo, availableRoles, args) => CreateListPrivate(userInfo, args)),
                new ManagerMethod("-cp",    ManagerMethodId.CREATEPUBLIC,   (userInfo, availableRoles, args) => CreateListPublic(userInfo, args)),
                new ManagerMethod("-a",     ManagerMethodId.ADD,            (userInfo, availableRoles, args) => Add(userInfo, args)),
                new ManagerMethod("-i",     ManagerMethodId.INSERT,         (userInfo, availableRoles, args) => Insert(userInfo, args)),
                new ManagerMethod("-l",     ManagerMethodId.OUTPUTPRIVATE,  (userInfo, availableRoles, args) => OutputListPrivate(userInfo, args)),
                new ManagerMethod("-lp",    ManagerMethodId.OUTPUTPUBLIC,   (userInfo, availableRoles, args) => OutputListPublic(userInfo, args)),
                new ManagerMethod("-r",     ManagerMethodId.REMOVE,         (userInfo, availableRoles, args) => Remove(userInfo, args)),
                new ManagerMethod("-rl",    ManagerMethodId.REMOVELIST,     (userInfo, availableRoles, args) => RemoveList(userInfo, args)),
                new ManagerMethod("-cl",    ManagerMethodId.CLEAR,          (userInfo, availableRoles, args) => Clear(userInfo, args))
            };

            RestoreOrCreateLists();
        }

        public ListOutput HandleIO(UserInfo userInfo, Dictionary<string, ulong> availableRoles, ulong messageId, params string[] input)
        {
            ListOutput output;
            try
            {
                output = Manage(userInfo, availableRoles, input);
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

        public ListOutput Manage(UserInfo userInfo, Dictionary<string, ulong> availableRoles, params string[] input)
        {
            var sa = SeperateArray(input, 0);
            string command = sa.seperated[0];
            string[] values = sa.array;

            ListOutput result;
            try
            {
                result = ValidOperations
                    .Where(vo => vo.Shortcut == command)
                    .Select(vo => vo.Reference)
                    .First()(userInfo, availableRoles, values);
            }
            catch (InvalidOperationException)
            {
                throw GetListManagerException(ListErrorMessage.General.UnknownCommand_command, command);
            }
            return result;
        }

        private ListOutput ModifyPermission(UserInfo userInfo, Dictionary<string, ulong> availableRoles, params string[] input)
        {
            if (input.Length < 3 || input.Length % 2 == 0) { throw GetListManagerException(ListErrorMessage.General.WrongFormat); }

            var sa = SeperateArray(input, 0, 1, 2);
            var log = new StringBuilder();
            var listName = sa.seperated[0];
            for (int i=1;i<input.Length; i+=2)
            {
                var roleName    = sa.seperated[i+0];
                var modifier    = sa.seperated[i+1];

                var list = GetList(listName);
                CheckPermissionModify(userInfo, list);

                var roleId = availableRoles.Where(ar => ar.Key == roleName).FirstOrDefault().Value;

                //var role = client.Guilds.First().Roles.Where(r => r.Name.Equals(roleName)).FirstOrDefault();
                if (roleId == default(ulong))
                {
                    throw GetListManagerException(ListErrorMessage.General.RoleDoesNotExist_rolename, roleName);
                }

                var permission = ValidPermissions[modifier];
                if (permission == null) { throw GetListManagerException(ListErrorMessage.General.WrongFormat); }

                bool result = list.SetPermissionByRole(roleId, permission);
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

        private ListOutput GetAllPrivate(UserInfo userInfo)
        {
            return GetAll(userInfo, ListPermission.PRIVATE);
        }

        private ListOutput GetAllPublic(UserInfo userInfo)
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

        private ListOutput CreateListPrivate(UserInfo userInfo, params string[] input)
        {
            return CreateList(userInfo, ListPermission.PRIVATE, input);
        }

        private ListOutput CreateListPublic(UserInfo userInfo, params string[] input)
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

        private ListOutput Add(UserInfo userInfo, string[] input)
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

        private ListOutput Insert(UserInfo userInfo, string[] input)
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

        private ListOutput RemoveList(UserInfo userInfo, params string[] input)
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

        private ListOutput Remove(UserInfo userInfo, string[] input)
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

        private ListOutput OutputListPrivate(UserInfo userInfo, params string[] input)
        {
            return OutputList(userInfo, ListPermission.PRIVATE, input);
        }

        private ListOutput OutputListPublic(UserInfo userInfo, params string[] input)
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

        private ListOutput Clear(UserInfo userInfo, string[] input)
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

        private void WriteContents()
        {
            List<string> listNames = Lists.Select(l => l.Name).ToList<string>();
            dataStorage.StoreObject(listNames, ListManagerLookup);
        }

        private void RestoreOrCreateLists()
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

        private void CheckPermissionList(UserInfo userInfo, CustomList list)
        {
            if (!list.IsAllowedToList(userInfo))
            {
                throw GetListPermissionException(ListErrorMessage.Permission.NoPermission_list, list.Name);
            }
        }

        private void CheckPermissionRead(UserInfo userInfo, CustomList list)
        {
            if (!list.IsAllowedToRead(userInfo))
            {
                throw GetListPermissionException(ListErrorMessage.Permission.NoPermission_list, list.Name);
            }
        }

        private void CheckPermissionWrite(UserInfo userInfo, CustomList list)
        {
            if (!list.IsAllowedToWrite(userInfo))
            {
                throw GetListPermissionException(ListErrorMessage.Permission.NoPermission_list, list.Name);
            }
        }

        private void CheckPermissionModify(UserInfo userInfo, CustomList list)
        {
            if (!list.IsAllowedToModify(userInfo))
            {
                throw GetListPermissionException(ListErrorMessage.Permission.NoPermission_list, list.Name);
            }
        }
    }
}
