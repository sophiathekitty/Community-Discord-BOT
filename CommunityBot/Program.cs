using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using CommunityBot.Configuration;
using CommunityBot.Features.Trivia;
using CommunityBot.Handlers;
using CommunityBot.Helpers;
using CommunityBot.Modules;
using Discord.Net;

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

        private async Task AttemptLogin(int attempt = 1)
        {
            try
            {
                await _client.LoginAsync(TokenType.Bot, BotSettings.config.Token);
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException == null)
                {
                    Console.WriteLine($"An HTTP Request exception occurred.\nMessage:\n{e.Message}");
                }
                else
                {
                    Global.WriteColoredLine($"An HTTP request ran into a problem:\n{e.InnerException.Message}",
                        ConsoleColor.Red);
                }

                var shouldTryAgain = GetTryAgainRequested(attempt);

                if (!shouldTryAgain) Environment.Exit(0);

                await AttemptLogin(attempt + 1);
            }
            catch (Exception)
            {
                Console.WriteLine("An exception occurred. Your token might not be configured, or it might be wrong.");
                Environment.Exit(0);
            }
        }

        private static bool GetTryAgainRequested(int attempt)
        {
            if (attempt >= 4) return false;

            Console.WriteLine($"\nDo you want to try again? (y/n) [{4 - attempt} {(4 - attempt == 1 ? "attempt" : "attempts")} left]");
            Global.WriteColoredLine("(not trying again closes the application)", ConsoleColor.Yellow);

            return Console.ReadKey().Key == ConsoleKey.Y;
        }
    }
}
