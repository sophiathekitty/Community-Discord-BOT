

using System;
using System.IO;
using System.Threading.Tasks;
using Discord;

namespace CommunityBot
{
    internal class Logger
    {
        internal static Task Log(LogMessage logMessage)
        {
            string message = String.Concat(DateTime.Now.ToShortTimeString(), " [", logMessage.Source, "] ", logMessage.Message);
            LogConsole(message, logMessage.Severity);
            LogFile(message);
            return Task.CompletedTask;
        }

        private static void LogFile(string message)
        {
            if (!Global.LogIntoFile) return;

            var fileName = $"{DateTime.Today.Day}-{DateTime.Today.Month}-{DateTime.Today.Year}.log";
            var folder = Constants.LogFolder;

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            StreamWriter sw = File.AppendText($"{folder}/{fileName}");
            sw.WriteLine(message);
            sw.Close();
        }

        private static void LogConsole(string message, LogSeverity severity)
        {
            if (!Global.LogIntoConsole) return;

            Console.ForegroundColor = SeverityToConsoleColor(severity);
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static ConsoleColor SeverityToConsoleColor(LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.Critical:
                    return ConsoleColor.Red;
                case LogSeverity.Debug:
                    return ConsoleColor.Blue;
                case LogSeverity.Error:
                    return ConsoleColor.Yellow;
                case LogSeverity.Info:
                    return ConsoleColor.Blue;
                case LogSeverity.Verbose:
                    return ConsoleColor.Green;
                case LogSeverity.Warning:
                    return ConsoleColor.Magenta;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}
