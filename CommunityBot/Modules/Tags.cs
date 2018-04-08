using CommunityBot.Preconditions;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityBot.Features.GlobalAccounts;
using CommunityBot.Handlers;
using Discord;
using CommunityBot.Entities;

namespace CommunityBot.Modules
{
    [Group("Tag"), Alias("ServerTag", "Tags", "T", "ServerTags")]
    [RequireContext(ContextType.Guild)]
    public class ServerTags : ModuleBase<SocketCommandContext>
    {
        [Command(""), Priority(-1)]
        public async Task ShowTag(string tagName)
        {
            var guildAcc = GlobalGuildAccounts.GetGuildAccount(Context.Guild.Id);
            var response = TagFunctions.GetTag(tagName, guildAcc);
            await ReplyAsync(response);
        }

        [Command("new"), Alias("add")]
        public async Task AddTag(string tagName, [Remainder] string tagContent)
        {
            var guildAcc = GlobalGuildAccounts.GetGuildAccount(Context.Guild.Id);
            var response = TagFunctions.AddTag(tagName, tagContent, guildAcc);
            await ReplyAsync(response);
        }

        [Command("update")]
        public async Task UpdateTag(string tagName, [Remainder] string tagContent)
        {
            var guildAcc = GlobalGuildAccounts.GetGuildAccount(Context.Guild.Id);
            var response = TagFunctions.UpdateTag(tagName, tagContent, guildAcc);
            await ReplyAsync(response);
        }

        [Command("remove")]
        public async Task RemoveTag(string tagName, [Remainder] string tagContent)
        {
            var guildAcc = GlobalGuildAccounts.GetGuildAccount(Context.Guild.Id);
            var response = TagFunctions.RemoveTag(tagName, guildAcc);
            await ReplyAsync(response);
        }

        [Command("list")]
        public async Task ListTags()
        {
            var guildAcc = GlobalGuildAccounts.GetGuildAccount(Context.Guild.Id);
            var emb = TagFunctions.BuildTagListEmbed(guildAcc);
            await ReplyAsync("", false, emb);
        }
    }

    [Group("PersonalTags"), Alias("PersonalTag", "PTags", "PTag", "PT")]
    [RequireContext(ContextType.Guild)]
    public class PersonalTags : ModuleBase<SocketCommandContext>
    {
        [Command(""), Priority(-1)]
        public async Task ShowTag(string tagName)
        {
            var userAcc = GlobalUserAccounts.GetUserAccount(Context.User.Id);
            var response = TagFunctions.GetTag(tagName, userAcc);
            await ReplyAsync(response);
        }

        [Command("new"), Alias("add")]
        public async Task AddTag(string tagName, [Remainder] string tagContent)
        {
            var userAcc = GlobalUserAccounts.GetUserAccount(Context.User.Id);
            var response = TagFunctions.AddTag(tagName, tagContent, userAcc);
            await ReplyAsync(response);
        }

        [Command("update")]
        public async Task UpdateTag(string tagName, [Remainder] string tagContent)
        {
            var userAcc = GlobalUserAccounts.GetUserAccount(Context.User.Id);
            var response = TagFunctions.UpdateTag(tagName, tagContent, userAcc);
            await ReplyAsync(response);
        }

        [Command("remove")]
        public async Task RemoveTag(string tagName, [Remainder] string tagContent)
        {
            var userAcc = GlobalUserAccounts.GetUserAccount(Context.User.Id);
            var response = TagFunctions.RemoveTag(tagName, userAcc);
            await ReplyAsync(response);
        }

        [Command("list")]
        public async Task ListTags()
        {
            var userAcc = GlobalUserAccounts.GetUserAccount(Context.User.Id);
            var emb = TagFunctions.BuildTagListEmbed(userAcc);
            await ReplyAsync("", false, emb);
        }
    }


    internal static class TagFunctions
    {
        internal static string AddTag(string tagName, string tagContent, IGlobalAccount account)
        {
            var response = "A tag with that name already exists!\n" +
                           "If you want to override it use `update <tagName> <tagContent>`";
            if (account.Tags.ContainsKey(tagName) == false)
            {
                account.Tags.Add(tagName, tagContent);
                if (account is GlobalGuildAccount)
                    GlobalGuildAccounts.SaveAccounts(account.Id);
                else GlobalUserAccounts.SaveAccounts(account.Id);
                response = $"Successfully added tag `{tagName}`.";
            }

            return response;
        }

        internal static Embed BuildTagListEmbed(IGlobalAccount account)
        {
            var tags = account.Tags;
            var embB = new EmbedBuilder().WithTitle("No tags set up yet... add some! =)");
            if (tags.Count > 0) embB.WithTitle("Here are all available tags:");

            foreach (var tag in tags)
            {
                embB.AddField(tag.Key, tag.Value, true);
            }

            return embB.Build();
        }

        internal static string GetTag(string tagName, IGlobalAccount account)
        {
            if (account.Tags.ContainsKey(tagName) == false)
                return "A tag with that name doesn't exists!";
            return account.Tags[tagName];
        }

        internal static string RemoveTag(string tagName, IGlobalAccount account)
        {
            if (account.Tags.ContainsKey(tagName) == false)
                return "You can't remove a tag that doesn't exist...";

            account.Tags.Remove(tagName);
            if (account is GlobalGuildAccount)
                GlobalGuildAccounts.SaveAccounts(account.Id);
            else GlobalUserAccounts.SaveAccounts(account.Id);

            return $"Successfully removed the tag {tagName}!";
        }

        internal static string UpdateTag(string tagName, string tagContent, IGlobalAccount account)
        {
            if (account.Tags.ContainsKey(tagName) == false)
                return "You can't update a tag that doesn't exist...";

            account.Tags[tagName] = tagContent;
            if (account is GlobalGuildAccount)
                GlobalGuildAccounts.SaveAccounts(account.Id);
            else GlobalUserAccounts.SaveAccounts(account.Id);

            return $"Successfully updated the tag {tagName}!";
        }
    }
}
