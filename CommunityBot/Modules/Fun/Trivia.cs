using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityBot.Features.Trivia;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace CommunityBot.Modules.Fun
{
    public class Trivia : ModuleBase<SocketCommandContext>
    {
        [Command("Trivia")]
        public async Task NewTrivia()
        {
            var msg = await Context.Channel.SendMessageAsync("", false, TriviaGames.TrivaStartingEmbed().Build());
            Global.TriviaGames.Add(new TriviaGame(msg.Id, Context.User.Id));
            await msg.AddReactionAsync(TriviaGames.ReactOptions["1"]);
            await msg.AddReactionAsync(TriviaGames.ReactOptions["2"]);
            await msg.AddReactionAsync(TriviaGames.ReactOptions["3"]);
            await msg.AddReactionAsync(TriviaGames.ReactOptions["4"]);
            await msg.AddReactionAsync(TriviaGames.ReactOptions["ok"]);
        }                                       
    }
}
