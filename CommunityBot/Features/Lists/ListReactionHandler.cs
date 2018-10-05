using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunityBot.Features.Lists
{
    public class ListReactionHandler
    {
        public async Task HandleReactionAdded(ListManager listManager, Cacheable<IUserMessage, ulong> cacheMessage, SocketReaction reaction)
        {
            if (ListManager.ListenForReactionMessages.ContainsKey(reaction.MessageId))
            {
                reaction.Message.Value.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                if (ListManager.ListenForReactionMessages[reaction.MessageId] == reaction.User.Value.Id)
                {
                    if (reaction.Emote.Name == ListManager.ControlEmojis["up"].Name)
                    {
                        await HandleMovement(reaction, cacheMessage.Value.Content, true);
                    }
                    else if (reaction.Emote.Name == ListManager.ControlEmojis["down"].Name)
                    {
                        await HandleMovement(reaction, cacheMessage.Value.Content, false);
                    }
                    else if (reaction.Emote.Name == ListManager.ControlEmojis["check"].Name)
                    {
                        var seperatedMessage = SepereateMessageByLines(cacheMessage.Value.Content);
                        foreach (string s in seperatedMessage)
                        {
                            if (ContainsLineIndicator(s))
                            {
                                ListManager.ListenForReactionMessages.Remove(reaction.MessageId);
                                reaction.Message.Value.RemoveAllReactionsAsync();

                                var listName = GetItemNameFromLine(s);
                                var context = new SocketCommandContext(Global.Client, reaction.Message.Value);
                                var output = listManager.HandleIO(context, new[] { "-l", listName });
                                reaction.Message.Value.ModifyAsync(msg => { msg.Content = output.outputString; msg.Embed = output.outputEmbed; });
                            }
                        }
                    }
                }
            }
        }

        private string GetItemNameFromLine(string line)
        {
            var firstCol = line.Split('|', StringSplitOptions.RemoveEmptyEntries)[0];
            firstCol = firstCol.Remove(0, 1);
            var i = firstCol.Length;
            char c;
            do
            {
                c = firstCol[--i];
            } while (c == ' ');
            return firstCol.Substring(0, i + 1);
        }

        private async Task HandleMovement(SocketReaction reaction, string message, bool dirUp)
        {
            var seperatedMessage = SepereateMessageByLines(message);
            reaction.Message.Value.ModifyAsync(msg => msg.Content = PerformMove(seperatedMessage, dirUp));
        }

        private string[] SepereateMessageByLines(string message)
        {
            return message.Split('\n');
        }

        private string PerformMove(string[] messageLines, bool dirUp)
        {
            var newMessageLines = new string[messageLines.Length];
            messageLines.CopyTo(newMessageLines, 0);


            for (int i = 0; i < messageLines.Length; i++)
            {
                var line = messageLines[i];
                var substringStart = line.Length - ListManager.LineIndicator.Length;
                if (substringStart < 0) { continue; }

                var subLine = line.Substring(substringStart);
                if (subLine.Equals(ListManager.LineIndicator))
                {
                    var newIndex = i + (dirUp ? -2 : 2);
                    if (newIndex > 7 && newIndex < messageLines.Length - 1)
                    {
                        newMessageLines[i] = line.Substring(0, substringStart);
                        newMessageLines[newIndex] = messageLines[newIndex] + ListManager.LineIndicator;
                    }
                    break;
                }
            }
            var newMessage = new StringBuilder();
            foreach (string s in newMessageLines)
            {
                newMessage.Append($"{s}\n");
            }
            return newMessage.ToString();
        }

        private bool ContainsLineIndicator(string line)
        {
            var substringLength = line.Length - ListManager.LineIndicator.Length;
            if (substringLength < 0) { return false; }

            var subLine = line.Substring(substringLength);
            return (subLine.Equals(ListManager.LineIndicator));
        }
    }
}
