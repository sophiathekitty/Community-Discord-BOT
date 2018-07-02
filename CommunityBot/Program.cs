using Discord.WebSocket;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using CommunityBot.Configuration;
using CommunityBot.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;

namespace CommunityBot
{
    class Program
    {
        private DiscordSocketClient _client;
        private IServiceProvider _serviceProvider;
        private ApplicationSettings _appSettings;

        static void Main(string[] args)
        => new Program().StartAsync(args).GetAwaiter().GetResult();

        public async Task StartAsync(string[] args)
        {
            _appSettings = new ApplicationSettings(args);

            var discordSocketConfig = GetDiscordSocketConfig();
            if (discordSocketConfig == null) return;

            _client = new DiscordSocketClient(discordSocketConfig);

            _serviceProvider = ConfigureServices();

            _serviceProvider.GetRequiredService<DiscordEventHandler>().InitDiscordEvents();
            await _serviceProvider.GetRequiredService<CommandHandler>().InitializeAsync();

            while (!await AttemptLogin()){}

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private DiscordSocketConfig GetDiscordSocketConfig()
        {
            return new DiscordSocketConfig()
            {
                LogLevel = _appSettings.Verbose ? LogSeverity.Verbose : LogSeverity.Info,
                MessageCacheSize = _appSettings.ChacheSize,
                AlwaysDownloadUsers = true
            };
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

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_appSettings)
                .AddSingleton<ServerActivityLogger.ServerActivityLogger>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<DiscordEventHandler>()
                .BuildServiceProvider();     
        }
    }
}
