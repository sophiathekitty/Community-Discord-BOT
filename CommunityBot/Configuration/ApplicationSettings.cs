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
                    "-cs=<number>           : message cache size per channel (defaults to 0)"
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
        }

    }
}
