using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Webhook;
using System.Timers;
using Discord.WebSocket;

namespace CommunityBot
{
    internal static class Timers
    {
        static DiscordSocketClient _client;
        private static Timer loopingtimer;

        internal static Task StartTimer()
        {
            loopingtimer = new Timer()
            {
                Interval = 7200000,
                AutoReset = true,
                Enabled = true
            };
            loopingtimer.Elapsed += OnTimerTicked;

            Console.WriteLine("Started Timer(s)");
            return Task.CompletedTask;
        }

        private static void OnTimerTicked(object sender, ElapsedEventArgs e)
        {
            var general = _client.GetChannel(403278466746810370) as SocketTextChannel;
            general.SendMessageAsync("If you have any problems with your code, please follow the instructions in <#406360393489973248>!");   
        }
    }
}

