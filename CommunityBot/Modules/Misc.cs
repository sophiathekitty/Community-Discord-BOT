using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        CommandHandler handler = new CommandHandler();

        [Command("help")]
        public async Task Help()
        {

            await Context.Channel.SendMessageAsync("Check your DMs.");

            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();

            var builder = new EmbedBuilder()
            {
                Title = "Help",
                Description = "These are the commands you can use",
                Color = new Color(114, 137, 218)

            };

            foreach (var module in handler._service.Modules)
            {
                string description = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                        description += $"{cmd.Aliases.First()}\n";
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }
            await dmChannel.SendMessageAsync("", false, builder.Build());
        }

        [Command("command")]
        [Remarks("Shows what a specific command does and what parameters it takes.")]
        public async Task HelpAsync(string command)
        {
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            var result = handler._service.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync($"Sorry, I couldn't find a command like {command}.");
                return;
            }

            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"Here are commands related to {command}"
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;

                builder.AddField(x =>
                {
                    x.Name = string.Join(", ", cmd.Aliases);
                    x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                                $"Remarks: {cmd.Remarks}";
                    x.IsInline = false;
                });
            }
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}
