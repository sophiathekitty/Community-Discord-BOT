using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using CommunityBot.Configuration;
using CommunityBot.Features.Trivia;
using CommunityBot.Handlers;
using CommunityBot.Helpers;
using CommunityBot.Modules;

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
            _client.Ready += RepeatedTaskFunctions.InitRepeatedTasks;
            _client.ReactionAdded += OnReactionAdded;
            _client.MessageReceived += MessageRewardHandler.HandleMessageRewards;
            // Subscribe to other events here.
            _client.Ready += ServerBots.Init;

            await InitializeCommandHandler();
            await AttemptLogin();
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (!reaction.User.Value.IsBot)
            {
                var msgList = Global.MessagesIdToTrack ?? new Dictionary<ulong, string>();
                if (msgList.ContainsKey(reaction.MessageId))
                {
                    if (reaction.Emote.Name == "➕")
                    {
                        var item = msgList.FirstOrDefault(k => k.Key == reaction.MessageId);
                        var embed = BlogHandler.SubscribeToBlog(reaction.User.Value.Id, item.Value);
                    }
                }
                // Checks if the rection is associated with a running game and if it is 
                // from the same user who ran the command - if so it handles it
                await TriviaGames.HandleReactionAdded(cache, reaction);
            }
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
