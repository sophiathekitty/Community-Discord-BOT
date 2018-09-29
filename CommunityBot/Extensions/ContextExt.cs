using CommunityBot.Entities;
using CommunityBot.Features.GlobalAccounts;
using Discord.Commands;
using Discord.WebSocket;

namespace CommunityBot.Extensions
{
    public class ContextExt : SocketCommandContext
    {
        public readonly GlobalUserAccount UserAccount {get;}
        
        public ContextExt(DiscordSocketClient client, SocketUserMessage msg) : base(client, msg)
        {
            if (User is null) 
            { 
                return; 
            }
            
            UserAccount = GlobalUserAccounts.GetUserAccount(User);
            
            var commandUsedInformation = new CommandInformation(msg.Content, msg.CreatedAt.DateTime);
            
            UserAccount.AddCommandToHistory(commandUsedInformation);

            GlobalUserAccounts.SaveAccounts(UserAccount.Id);
        }
    }
}
