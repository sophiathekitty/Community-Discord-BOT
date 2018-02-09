using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityBot.Modules
{
    public class Basics : ModuleBase<SocketCommandContext>
    {
        [Command("Hello")]
        public async Task SayHello()
        {
            // ===== Greet the user =====
            // Just respond to the Hello message.
            await Context.Channel.SendMessageAsync("Hi!");
        }
    }
}
