using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityBot.Features.Lists
{
    public static class ListErrorMessage
    {
        public static string ListDoesNotExist_list = "List '{0}' does not exist.";
        public static string ListAlreadyExists_list = "List '{0}' already exists.";
        public static string ListIsEmpty_list = "The list '{0}' is empty";
        public static string NoLists = "There are no lists";
        public static string UnknownCommand_command = "Unknown command '{0}'.";
        public static string UnknownError = "Oops, something went wrong";
    }
}
