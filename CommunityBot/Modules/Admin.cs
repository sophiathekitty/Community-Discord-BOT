using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CommunityBot.Modules
{
    public class Admin : ModuleBase<SocketCommandContext>
    {
        [Command("kick")]
        [Remarks("Kick A User")]
        [RequireUserPermission(Discord.GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser user)
        {
            await user.KickAsync();
        }

        [Command("nickname")]
        [Remarks("Set A User's Nickname")]
        [RequireUserPermission(Discord.GuildPermission.ManageNicknames)]
        public async Task Nickname(SocketGuildUser username, [Remainder]string name)
        {
            await Context.Guild.GetUser(username.Id).ModifyAsync(x => x.Nickname = name);
        }

        [Command("announce")]
        [Remarks("Make A Announcement")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Announce([Remainder]string announcement)
        {

            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("Announcement By " + Context.Message.Author.ToString());
            embed.WithDescription(announcement);
            embed.WithColor(0, 125, 255);
            embed.WithCurrentTimestamp();

            await Context.Channel.SendMessageAsync("", false, embed);
            await Context.Message.DeleteAsync();
        }

        [Command("echo")]
        [Remarks("Make The Bot Say A Message")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Echo([Remainder] string message)
        {

            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("Message by: " + Context.Message.Author.Username);
            embed.WithDescription(message);
            embed.WithColor(0, 125, 255);
            embed.WithCurrentTimestamp();

            await Context.Channel.SendMessageAsync("", false, embed);
            await Context.Message.DeleteAsync();
        }
    }
}
