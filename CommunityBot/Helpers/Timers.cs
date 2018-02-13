using System;
using System.Threading.Tasks;
using System.Timers;
using Discord.WebSocket;

namespace CommunityBot.Helpers
{
    internal static class Timers
    {
        private static DiscordSocketClient _client;
        private static Timer loopingtimer;

        internal static Task StartTimer()
        {
            var twoHoursInMiliSeconds = 720000;
            loopingtimer = new Timer()
            {
                Interval = twoHoursInMiliSeconds,
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

