using System;
using System.Linq;

namespace CommunityBot.Configuration
{
    public class ApplicationSettings
    {
        public readonly bool Headless;
        public readonly bool Verbose;
        public readonly int ChacheSize;
        public readonly bool LoggerDownloadingAttachment;
        public readonly bool LogIntoFile;
        public readonly bool LogIntoConsole;

        public ApplicationSettings(string [] args)
        {
           if (args.Length > 0) args = args[0].Split(" ");

            // Help argument handling -help / -h / -info / -i
            if (args.Any(arg => new[]{"-help", "-h", "-info", "-i"}.Contains(arg)))
            {
                Console.WriteLine(
                    "Possible arguments you can provide are:\n" +
                    "-help | -h | -info -i  : shows this help\n" +
                    "-hl                    : run in headless mode (no output to console)\n" +
                    "-vb                    : run with verbose discord logging\n" +
                    "-token=<token>         : run with specific token instead of the saved one in bot configs\n" +
                    "-cs=<number>           : message cache size per channel (defaults to 0)" +
                    "-log=<f | c>           : log into a (f)ile, (c)onsole  or both. Default is console"
                );
                return;
            }

            // Headless argument handling -hl
            if (args.Contains("-hl")) Headless = true;

            // Verbose argument handling -vb
         
            if (args.Contains("-vb"))
                Verbose = true;


            // Cachesize argument handling -cs=<cacheSize>
            var chacheSize = 500;
 
            if (args.Any(arg => arg.StartsWith("-cs=")))
            {
                var numberString = args.FirstOrDefault(arg => arg.StartsWith("-cs="))?.Replace("-cs=", "");
                int.TryParse(numberString, out chacheSize);
            }

            ChacheSize = chacheSize;
         
             // Token argument handling -token=YOUR.TOKEN.HERE
            
            var tokenString = args.FirstOrDefault(arg => arg.StartsWith("-token="));
            if (string.IsNullOrWhiteSpace(tokenString) == false)
            {
                BotSettings.config.Token = tokenString.Replace("-token=", "");
            }

            // Downloading Attachemnts for Activity Logger -att
            if (args.Contains("-att"))
            LoggerDownloadingAttachment = true;

            // Log output handling -log=<f | c>
            //f = file c = console
            // Default is (c)onsole
            LogIntoConsole = true;
            LogIntoFile = false;
            if (args.Any(arg => arg.StartsWith("-log=")))
            {
                var options = args.FirstOrDefault(arg => arg.StartsWith("-log="))?.Replace("-log=", "");
                if (options.Contains("f") && options.Contains("c"))
                {
                    LogIntoFile = true;
                    LogIntoConsole = true;
                }
                else if (options.Contains("f"))
                {
                    LogIntoConsole = false;
                    LogIntoFile = true;
                }
            }
            Global.LogIntoConsole = LogIntoConsole;
            Global.LogIntoFile = LogIntoFile;
        }

    }
}
