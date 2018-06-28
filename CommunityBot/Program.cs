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

namespace CommunityBot
{
    class Program
    {
        private DiscordSocketClient _client;
        private CommandHandler _handler;

        static void Main(string[] args)
        => new Program().StartAsync(args).GetAwaiter().GetResult();

        public async Task StartAsync(string[] args)
        {
            if (args.Length > 0) args = args[0].Split(" ");
            if (args.Any(arg => new string[]{"-help", "-h", "-info", "-i"}.Contains(arg)))
            {
                Console.WriteLine(   
                    "Possible arguments you can provide are:\n" +
                    "-help | -h | -info -i  : shows this help\n" +
                    "-hl                    : run in headless mode (no output to console)\n" +
                    "-vb                    : run with verbose discord logging\n" +
                    "-token=<token>         : run with specific token instead of the saved one in bot configs\n" +
                    "-cs=<number>           : message cache size per channel (defaults to 0)"
                );
                return;
            }

            if (args.Contains("-hl")) Global.Headless = true;

            var logLevel = LogSeverity.Info;
            if (args.Contains("-vb"))
                logLevel = LogSeverity.Verbose;
            
            var chacheSize = 0;
            if (args.Any(arg => arg.StartsWith("-cs=")))
            {
                var numberString = args.FirstOrDefault(arg => arg.StartsWith("-cs=")).Replace("-cs=", "");
                int.TryParse(numberString, out chacheSize);
            }

            var discordSocketConfig = new DiscordSocketConfig()
            {
                LogLevel = logLevel,
                MessageCacheSize = chacheSize
            };

            _client = new DiscordSocketClient(discordSocketConfig);

            if (!Global.Headless)
            {
                _client.Log += Logger.Log;
            }

            _client.Ready += RepeatedTaskFunctions.InitRepeatedTasks;
            _client.ReactionAdded += OnReactionAdded;
            _client.MessageReceived += MessageRewardHandler.HandleMessageRewards;
            // Subscribe to other events here.
            _client.Ready += ServerBots.Init;

            // Use argument token if available
            var tokenString = args.FirstOrDefault(arg => arg.StartsWith("-token="));
            if (tokenString is null == false) 
            {
                BotSettings.config.Token = tokenString.Replace("-token=", "");
            }

            await InitializeCommandHandler();
            while (!await AttemptLogin()){}
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

        private async Task<bool> AttemptLogin()
        {
            try
            {
                await _client.LoginAsync(TokenType.Bot, BotSettings.config.Token);
                return true;
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

                var shouldTryAgain = GetTryAgainRequested();
                if (!shouldTryAgain) Environment.Exit(0);
                return false;
            }
            catch (Exception)
            {
                Console.WriteLine("An exception occurred. Your token might not be configured, or it might be wrong.");

                var shouldTryAgain = GetTryAgainRequested();
                if (!shouldTryAgain) Environment.Exit(0);
                BotSettings.LoadConfig();
                return false;
            }
        }
        
        private static bool GetTryAgainRequested()
        {
            if (Global.Headless) return false;

            Console.WriteLine("\nDo you want to try again? (y/n)");
            Global.WriteColoredLine("(not trying again closes the application)\n", ConsoleColor.Yellow);

            return Console.ReadKey().Key == ConsoleKey.Y;
        }
    }
}
