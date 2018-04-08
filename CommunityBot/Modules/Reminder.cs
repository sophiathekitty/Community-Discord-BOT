using System;
using System.Globalization;
using System.Threading.Tasks;
using CommunityBot.Entities;
using CommunityBot.Features.GlobalAccounts;
using Discord;
using Discord.Commands;

namespace CommunityBot.Modules
{
    [Group("Reminder"), Alias("Remind", "r")]
    [Summary("Tell the bot to remind you in some amount of time. The bot will send you a DM with the text you specified.")]
    public class Reminder : ModuleBase<SocketCommandContext>
    {
        [Command(""), Alias("New", "Add"), Priority(0), Remarks("Add a reminder")]
        public async Task AddReminder([Remainder] string args)
        {
            string[] splittedArgs = null;
            if (args.Contains(" in ")) splittedArgs = args.Split(new string[] {" in "}, StringSplitOptions.None);
            if (splittedArgs == null || splittedArgs.Length < 2)
            {
                await ReplyAsync("I think you are confused about how to use this command... aren't you?\n" +
                                 "Let me REMIND you it is: `remind DO THE THING! :rage: in 2d 23h 3m 12s`\n" +
                                 "And the ` in ` before the timeparameters is very important you little dumbo you...");
                return;
            }

            var timeString = splittedArgs[splittedArgs.Length - 1];
            splittedArgs[splittedArgs.Length - 1] = "";
            var reminderString = string.Join(" in ", splittedArgs, 0, splittedArgs.Length - 1);
            string[] formats = {
                // Used to parse stuff like 1d14h2m11s and 1d 14h 2m 11s could add/remove more if needed
                "d'd'h'h'm'm's's'", "d'd 'h'h 'm'm 's's'",
                "h'h'm'm's's'", "h'h 'm'm 's's'",
                "m'm's's'", "m'm 's's'",
                "s's'",
                "d'd'h'd'm'm'", "d'd 'h'h 'm'm'",
                "d'd'h'h's's'", "d'd 'h'h 's's'",
                "d'd'm'm's's'", "d'd 'm'm 's's'",
                "h'h'm'm'", "h'h 'm'm'",
                "h'h's's'","h'h 's's'",
            };
            var timeDateTime = DateTime.UtcNow + TimeSpan.ParseExact(timeString, formats, CultureInfo.CurrentCulture);

            var newReminder = new ReminderEntry(timeDateTime, reminderString);

            GlobalUserAccounts.GetUserAccount(Context.User.Id).Reminders.Add(newReminder);
            GlobalUserAccounts.SaveAccounts(Context.User.Id);
        }

        [Command("List"), Priority(1), Remarks("List all your reminders")]
        public async Task ShowReminders()
        {
            var reminders = GlobalUserAccounts.GetUserAccount(Context.User.Id).Reminders;
            var embB = new EmbedBuilder()
                .WithTitle("Your Reminders (Times are in UTC / GMT+0)")
                .WithFooter("Did you know? " + Global.GetRandomDidYouKnow())
                .WithDescription("To delete a reminder ust use the command `reminder delete <number>` " +
                                 "and the number is the one to the left of the Dates inside the [].");

            for (var i = 0; i < reminders.Count; i++)
            {
                embB.AddField($"[{i+1}] {reminders[i].DueDate:f}", reminders[i].Description, true);
            }

            await ReplyAsync("", false, embB.Build(), null);
        }

        [Command("Delete"), Priority(1), Remarks("Delete one of your reminders")]
        public async Task DeleteReminder(int index)
        {
            var reminders = GlobalUserAccounts.GetUserAccount(Context.User.Id).Reminders;
            var responseString = "Duhh... maybe use `remind list` before you try to " +
                                 "delete a reminder that doesn't even exists?!";
            if (index > 0 && index <= reminders.Count)
            {
                reminders.RemoveAt(index - 1);
                GlobalUserAccounts.SaveAccounts(Context.User.Id);
                responseString = $"Deleted the reminder with index {index}!";
            }

            await ReplyAsync(responseString);
        }
    }
}
