using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace CommunityBot
{
    internal class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _client.MessageReceived += HandleCommandAsync;
            _client.UserJoined += _client_UserJoined;
            _client.UserLeft += _client_UserLeft;
        }

        private async Task _client_UserJoined(SocketGuildUser user)
        {
            if (user.Guild.Name == "Discord-BOT-Tutorial")
            {
                var dmChannel = await user.GetOrCreateDMChannelAsync();
                await dmChannel.SendMessageAsync($"{user.Mention}, Welcome to **{user.Guild.Name}**. try using ``@Community-Bot#8321 help`` for all the commands!");
            }
        }

        private async Task _client_UserLeft(SocketGuildUser user)
        {
            if (user.Guild.Name == "Discord-BOT-Tutorial")
            {
                var DiscordBotTutorial_General = _client.GetChannel(377879473644765185) as SocketTextChannel;
                await DiscordBotTutorial_General.SendMessageAsync($"{user.Username} ({user.Id}) left **{user.Guild.Name}**!");
            }
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            if (msg.Channel == msg.Author.GetOrCreateDMChannelAsync()) return;

            var context = new SocketCommandContext(_client, msg);
            if (context.User.IsBot) return;
            
            int argPos = 0;
            if (msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var cmdSearchResult = _service.Search(context, argPos);
                if (cmdSearchResult.Commands.Count == 0) return;

                var result = await _service.ExecuteAsync(context, argPos);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    string errTemplate = "{0}, Error: {1}.";
                    string errMessage = String.Format(errTemplate, context.User.Mention, result.ErrorReason);
                    await context.Channel.SendMessageAsync(errMessage);
                }
            }
        }
    }
}