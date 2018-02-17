using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Discord.Webhook;
using Discord.WebSocket;
using System.Text;
using System.Threading.Tasks;
using CommunityBot.Preconditions;

namespace CommunityBot.Modules
{
    class Voice : ModuleBase<SocketCommandContext>
    {
        [Command("Voice", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        [Cooldown(10, true)]
        public async Task CreateTemporaryVoiceChannel(int lifetimeInMinutes = 0)
        {
            lifetimeInMinutes = lifetimeInMinutes * 60000;
            var MaxTimeInMiliseconds = 60 * 60000;
            if (lifetimeInMinutes == 0)
            {
                var use = await Context.Channel.SendMessageAsync("Use: ``!Voice {Time In Minutes}``");
                await Task.Delay(5000);
                await use.DeleteAsync();
            }
            if (lifetimeInMinutes >= MaxTimeInMiliseconds)
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("!Voice");
                embed.AddField("use", "!Voice {Ammount in minutes}");
                // empty field to fix Text collisions
                embed.AddField("", "");
                // empty field to fix Text collisions
                embed.AddField("maximum time", "60 minutes");
                embed.WithCurrentTimestamp();
                embed.WithColor(0, 0, 255);
                await Context.Channel.SendMessageAsync("", embed: embed);
            }
            if (lifetimeInMinutes == 1)
            {
                await Context.Message.DeleteAsync();
                var v = await Context.Guild.CreateVoiceChannelAsync(name: $"{Context.User.Username}'s Voice Channel ({lifetimeInMinutes / 60000} minute)");
                var me = await Context.Channel.SendMessageAsync($"A voice channel has been created! {Context.User.Mention}!");
                await me.ModifyAsync(m => { m.Content = $"A voice channel has been created! {Context.User.Mention}! {lifetimeInMinutes / 60000} minute left"; });
                await Task.Delay(lifetimeInMinutes);
                await v.DeleteAsync();
                await me.DeleteAsync();
                return;
            }
            if (lifetimeInMinutes >= 2 && lifetimeInMinutes <= MaxTimeInMiliseconds)
            {
                await Context.Message.DeleteAsync();
                var v = await Context.Guild.CreateVoiceChannelAsync(name: $"{Context.User.Username}'s Voice Channel ({lifetimeInMinutes / 60000} minutes)");
                var me = await Context.Channel.SendMessageAsync($"A voice channel has been created! {Context.User.Mention}! {lifetimeInMinutes / 60000} minutes left");
                await Task.Delay(lifetimeInMinutes);
                await v.DeleteAsync();
                await me.ModifyAsync(x => { x.Content = $"{Context.User.Mention}, {lifetimeInMinutes * 60000} minutes have passed, the voice channel has been deleted"; });
            }
        }
    }
}
