using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using CommunityBot.Configuration;
using CommunityBot.Handlers;
using CommunityBot.Helpers;

namespace CommunityBot
{
    class Program
    {
        private DiscordSocketClient _client;
        private CommandHandler _handler;

        static void Main(string[] args)
        => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            var discordSocketConfig = new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Verbose
            };

            _client = new DiscordSocketClient(discordSocketConfig);
            _client.Log += Logger.Log;
            _client.Ready += Timers.StartTimer;
            _client.ReactionAdded += OnReactionAdded;
            // Subscribe to other events here.

            await InitializeCommandHandler();
            await AttemptLogin();
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private Task OnReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var msgList = Global.MessagesIdToTrack;
            if (msgList.ContainsKey(reaction.MessageId))
            {
                if (reaction.Emote.Name == "➕")
                {
                    var item = msgList.FirstOrDefault(k => k.Key == reaction.MessageId);
                    var embed = BlogHandler.SubscribeToBlog(reaction.User.Value.Id, item.Value);
                }
            }

            return Task.CompletedTask;
        }

        private async Task InitializeCommandHandler()
        {
            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);
        }

        private async Task AttemptLogin()
        {
            try
            {
                await _client.LoginAsync(TokenType.Bot, BotSettings.config.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine("The BOT Token is most likely incorrect.");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
    }
}
