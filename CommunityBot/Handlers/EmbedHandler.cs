using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityBot.Helpers;
using Discord;

namespace CommunityBot.Handlers
{
    public static class EmbedHandler
    {
        /// <summary>
        /// Create a new embed
        /// </summary>
        /// <param name="title">Title of the embed</param>
        /// <param name="body">Embed content</param>
        /// <param name="type">Type of the Embed (Error, Info, Exception, Success) -> Sets the color</param>
        /// <returns></returns>
        public static Embed CreateEmbed(string title, string body, EmbedMessageType type)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle(title);
            embed.WithDescription(body);

            switch (type)
            {
                case EmbedMessageType.Info:
                    embed.WithColor(new Color(52, 152, 219));
                    break;
                case EmbedMessageType.Success:
                    embed.WithColor(new Color(22, 160, 133));
                    break;
                case EmbedMessageType.Error:
                    embed.WithColor(new Color(192, 57, 43));
                    break;
                case EmbedMessageType.Exception:
                    embed.WithColor(new Color(230, 126, 34));
                    break;
                default:
                    embed.WithColor(new Color(149, 165, 166));
                    break;
            }

            return embed;
        }
    }
}
