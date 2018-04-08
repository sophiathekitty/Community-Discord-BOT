using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CommunityBot.Features.Economy;
using CommunityBot.Features.GlobalAccounts;
using Discord;
using Discord.Commands;

namespace CommunityBot.Modules
{
    public class Economy : ModuleBase<SocketCommandContext>
    {
        [Command("Daily")]
        [Alias("GetDaily", "ClaimDaily")]
        public async Task GetDaily()
        {
            var result = Daily.GetDaily(Context.User.Id);
            switch (result)
            {
                case Daily.DailyResult.AlreadyRecieved:
                    await ReplyAsync($"You already got your daily, {Context.User.Mention}.");
                    break;
                case Daily.DailyResult.Success:
                    await ReplyAsync($"Here's {Constants.DailyMuiniesGain} miunies, {Context.User.Mention}! Just for you...");
                    break;
            }
        }

        [Command("Miunies")]
        [Alias("Cash", "Money")]
        public async Task CheckMiunies()
        {
            var account = GlobalUserAccounts.GetUserAccount(Context.User.Id);
            await ReplyAsync(GetMiuniesReport(account.Miunies, Context.User.Mention));
        }

        [Command("Miunies")]
        [Alias("Cash", "Money")]
        public async Task CheckMiuniesOther(IGuildUser target)
        {
            var account = GlobalUserAccounts.GetUserAccount(target.Id);
            await ReplyAsync(GetMiuniesReport(account.Miunies, target.Mention));
        }

        [Command("Transfer")]
        [Remarks("Transferrs specified amount of your Minuies to the mentioned person.")]
        [Alias("Give", "Gift")]
        public async Task TransferMinuies(IGuildUser target, ulong amount)
        {
            if (Context.User.Id == target.Id)
            {
                await ReplyAsync($":negative_squared_cross_mark: You can\'t gift yourself...\n**And you KNOW it!**");
                return;
            }
            var transferSource = GlobalUserAccounts.GetUserAccount(Context.User.Id);
            if (transferSource.Miunies < amount)
            {
                await ReplyAsync($":negative_squared_cross_mark: You don\'t have that much Minuies! You only have {transferSource.Miunies}.");
                return;
            }
            var transferTarget = GlobalUserAccounts.GetUserAccount(target.Id);
            transferSource.Miunies -= amount;
            transferTarget.Miunies += amount;
            GlobalUserAccounts.SaveAccounts();
            await ReplyAsync($" :white_check_mark: {Context.User.Username} has given {target.Username} {amount} Minuies!");
        }

        public string GetMiuniesReport(ulong miunies, string mention)
        {
            return $"{mention} has **{miunies} miunies**! {Global.GetMiuniesCountReaction(miunies, mention)} \n\nDid you know?\n`{Global.GetRandomDidYouKnow()}`";
        }
    }
}
