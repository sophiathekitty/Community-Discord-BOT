using System;
using System.IO;

namespace CommunityBot.Features
{
    public class BlackBox
    {
        public BlackBox()
        {
            AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;
        }

        private void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            File.WriteAllText($"BlackBox-from-{DateTime.Now:MM-dd-yyyy-H;mm;ss}.log", exception.ToString());
            Environment.Exit(1);
        }
    }
}