using System.Linq;
using System.Threading.Tasks;
using CommunityBot.ConfigServerAccount;
using Discord;
using Discord.Commands;

namespace CommunityBot.Modules
{
    public class ServerSetup : ModuleBase<SocketCommandContext>

    {
        [Command("build")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task BuildExistingServer()
        {
            var guild = Global.Client.Guilds.ToList();

            foreach (var t in guild)
            {
                ServerAccounts.GetServerAccount(t);
            } 
                await ReplyAsync("I have added all servers into the file.");
        }

        [Command("prefix")]
        public async Task CheckPrefix()
        {
            var guild = ServerAccounts.GetServerAccount(Context.Guild);
            await ReplyAsync($"current prefix: `{guild.Prefix}`");
        }

        [Command("setPrefix")]
        [Alias("setpref")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task SetPrefix([Remainder]string prefix)
        {
            try
            {
                if (prefix.Length >= 5)
                {
                    await ReplyAsync($" Please choose prefix using up to 4 characters");
                    return;
                }

                var guild = ServerAccounts.GetServerAccount(Context.Guild);
                guild.Prefix = prefix;
                ServerAccounts.SaveServerAccounts();            
                    await ReplyAsync($"Prefix is now: `{guild.Prefix}`");
            }
            catch
            {
                //
            }
        }


        [Command("offLog")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetServerActivivtyLogOff()
        {
            var guild = ServerAccounts.GetServerAccount(Context.Guild);
            guild.LogChannelId = 0;
            guild.ServerActivityLog = 0;
            ServerAccounts.SaveServerAccounts();

            await ReplyAsync($"No more Logging");

        }

        /// <summary>
        /// by saying "SetLog" it will create a   channel itself, you may move and rname it
        /// by saying "SetLog ID" it will set channel "ID" as Logging Channel
        /// by saying "SetLog" again, it will turn off Logging, but will not delete it from the file
        /// </summary>
        /// <param name="logChannel"></param>
        /// <returns></returns>
        [Command("SetLog")]
        [Alias("SetLogs")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetServerActivivtyLog(ulong logChannel = 0)
        {
            var guild = ServerAccounts.GetServerAccount(Context.Guild);

            if (logChannel != 0)
            {
                try
                {
                    var channel = Context.Guild.GetTextChannel(logChannel);
                    guild.LogChannelId = channel.Id;
                    guild.ServerActivityLog = 1;
                    ServerAccounts.SaveServerAccounts();

                }
                catch
                {
//
                }

                return;
            }

            switch (guild.ServerActivityLog)
            {
                case 1:
                    guild.ServerActivityLog = 0;
                    guild.LogChannelId = 0;
                    ServerAccounts.SaveServerAccounts();


                        await ReplyAsync($"No more logging any activity now\n");

                    return;
                case 0:
                    try
                    {
                        try
                        {
                            var tryChannel = Context.Guild.GetTextChannel(guild.LogChannelId);
                            if (tryChannel.Name != null)
                            {
                                guild.LogChannelId = tryChannel.Id;
                                guild.ServerActivityLog = 1;
                                ServerAccounts.SaveServerAccounts();

                                await ReplyAsync(
                                    $"Now we log everything to {tryChannel.Mention}, you may rename and move it.");
                            }
                        }
                        catch
                        {

                            var channel = Context.Guild.CreateTextChannelAsync("OctoLogs");
                            guild.LogChannelId = channel.Result.Id;
                            guild.ServerActivityLog = 1;
                            ServerAccounts.SaveServerAccounts();

                            await ReplyAsync(
                                $"Now we log everything to {channel.Result.Mention}, you may rename and move it.");
                        }

                    }
                    catch
                    {
//
                    }

                    break;
            }

        }



        [Command("SetRoleOnJoin")]
        [Alias("RoleOnJoin")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task SetRoleOnJoin(string role = null)
        {

            string text;
            var guild = ServerAccounts.GetServerAccount(Context.Guild);
            if (role == null)
            {
                guild.RoleOnJoin = null;
                text = $"No one will get role on join from me!";
            }
            else
            {
                guild.RoleOnJoin = role;
                text = $"Everyone will now be getting {role} role on join!";
            }

            ServerAccounts.SaveServerAccounts();
            await ReplyAsync(text);

        }
    }
}
