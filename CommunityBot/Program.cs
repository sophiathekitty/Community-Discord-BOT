using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using CommunityBot.Configuration;

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
            // Subscribe to other events here.

            await InitializeCommandHandler();
            await AttemptLogin();
            await _client.StartAsync();
            await Task.Delay(-1);
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
