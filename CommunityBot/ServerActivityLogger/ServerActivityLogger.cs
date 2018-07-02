using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using CommunityBot.Features.GlobalAccounts;

namespace CommunityBot.ServerActivityLogger
{
    public class ServerActivityLogger
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.


        public static async Task ChannelDestroyed(IChannel arg)
        {
            try
            {
                var embed = new EmbedBuilder();
                embed.WithColor(14, 243, 247);

                if (arg is ITextChannel channel)
                {
                    var log = await channel.Guild.GetAuditLogAsync(1);
                    var audit = log.ToList();

                    var name = audit[0].Action == ActionType.ChannelDeleted ? audit[0].User.Mention : "error";
                    var auditLogData = audit[0].Data as ChannelDeleteAuditLogData;
                    embed.AddField("🚫 Channel Destroyed", $"Name: {arg.Name}\n" +
                                                           $"WHO: {name}\n" +
                                                           $"Type {auditLogData?.ChannelType}" +
                                                           $"NSFW: {channel.IsNsfw}\n" +
                                                           $"Category: {channel.GetCategoryAsync().Result.Name}\n" +
                                                           $"ID: {arg.Id}\n");

                    embed.WithTimestamp(DateTimeOffset.UtcNow);
                    embed.WithThumbnailUrl($"{audit[0].User.GetAvatarUrl()}");
                }


                if (arg is IGuildChannel currentIguildChannel)
                {
                    var guild = GlobalGuildAccounts.GetGuildAccount(currentIguildChannel.Guild.Id);
                    if (guild.ServerActivityLog == 1)
                    {
                        await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                            .SendMessageAsync("", false, embed.Build());
                    }
                }
            }
            catch
            {
                //
            }
        }

        public static async Task Client_ChannelDestroyed(IChannel arg)
        {
            ChannelDestroyed(arg);
            await Task.CompletedTask;
        }

        public static async Task ChannelCreated(IChannel arg)
        {
            try
            {
                if (!(arg is ITextChannel channel))
                    return;

                var log = await channel.Guild.GetAuditLogAsync(1);
                var audit = log.ToList();
                var name = audit[0].Action == ActionType.ChannelCreated ? audit[0].User.Mention : "error";
                var auditLogData = audit[0].Data as ChannelCreateAuditLogData;

                var embed = new EmbedBuilder();
                embed.WithColor(14, 243, 247);
                embed.AddField("📖 Channel Created", $"Name: {arg.Name}\n" +
                                                     $"WHO: {name}\n" +
                                                     $"Type: {auditLogData?.ChannelType.ToString()}\n" +
                                                     $"NSFWL {channel.IsNsfw}\n" +
                                                     $"Category: {channel.GetCategoryAsync().Result.Name}\n" +
                                                     $"ID: {arg.Id}\n");
                embed.WithTimestamp(DateTimeOffset.UtcNow);
                embed.WithThumbnailUrl($"{audit[0].User.GetAvatarUrl()}");


                var currentIGuildChannel = (IGuildChannel) arg;
                var guild = GlobalGuildAccounts.GetGuildAccount(currentIGuildChannel.Guild.Id);
                if (guild.ServerActivityLog == 1)
                {
                    await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("", false, embed.Build());
                }
            }
            catch
            {
//
            }

        }

        public static async Task Client_ChannelCreated(IChannel arg)
        {
            ChannelCreated(arg);
            await Task.CompletedTask;

        }

        public static async Task GuildMemberUpdated(SocketGuildUser before, SocketGuildUser after)
        {
            try
            {
                if (after == null || before == after || before.IsBot)
                    return;

                var guild = GlobalGuildAccounts.GetGuildAccount(before.Guild.Id);

                var embed = new EmbedBuilder();
                if (before.Nickname != after.Nickname)
                {
                    var log = await before.Guild.GetAuditLogsAsync(1).FlattenAsync();
                    var audit = log.ToList();
                    var beforeName = before.Nickname ?? before.Username;

                    var afterName = after.Nickname ?? after.Username;

                    embed.WithColor(255, 255, 0);
                    embed.WithTimestamp(DateTimeOffset.UtcNow);
                    embed.AddField("💢 Nickname Changed:",
                        $"User: **{before.Username} {before.Id}**\n" +
                        $"Server: **{before.Guild.Name}**\n" +
                        $"Before:\n" +
                        $"**{beforeName}**\n" +
                        $"After:\n" +
                        $"**{afterName}**");
                    if (audit[0].Action == ActionType.MemberUpdated)
                        embed.AddField("WHO:", $"{audit[0].User.Mention}\n");
                    embed.WithThumbnailUrl($"{after.GetAvatarUrl()}");

                    if (guild.ServerActivityLog == 1)
                    {
                        await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                            .SendMessageAsync("", false, embed.Build());
                    }
                }

                if (before.GetAvatarUrl() != after.GetAvatarUrl())
                {
                    embed.WithColor(255, 255, 0);
                    embed.WithTimestamp(DateTimeOffset.UtcNow);
                    embed.AddField("💢 Avatar Changed:",
                        $"User: **{before.Username} {before.Id}**\n" +
                        $"Server: **{before.Guild.Name}**\n" +
                        $"Before:\n" +
                        $"**{before.GetAvatarUrl()}**\n" +
                        $"After:\n" +
                        $"**{after.GetAvatarUrl()}**");
                    embed.WithThumbnailUrl($"{after.GetAvatarUrl()}");



                    if (guild.ServerActivityLog == 1)
                    {
                        await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                            .SendMessageAsync("", false, embed.Build());
                    }
                }

                if (before.Username != after.Username || before.Id != after.Id)
                {
                    embed.WithColor(255, 255, 0);
                    embed.WithTimestamp(DateTimeOffset.UtcNow);
                    embed.AddField("💢 USERNAME Changed:",
                        $"Server: **{before.Guild.Name}**\n" +
                        $"Before:\n" +
                        $"**{before.Username} {before.Id}**\n" +
                        $"After:\n" +
                        $"**{after.Username} {after.Id}**\n");
                    embed.WithThumbnailUrl($"{after.GetAvatarUrl()}");




                    if (guild.ServerActivityLog == 1)
                    {
                        await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                            .SendMessageAsync("", false, embed.Build());
                    }
                }

                if (before.Roles.Count != after.Roles.Count)
                {

                    string roleString;
                    var list1 = before.Roles.ToList();
                    var list2 = after.Roles.ToList();
                    var role = "";
                    if (before.Roles.Count > after.Roles.Count)
                    {
                        roleString = "Removed";
                        var differenceQuery = list1.Except(list2);
                        var socketRoles = differenceQuery as SocketRole[] ?? differenceQuery.ToArray();
                        for (var i = 0; i < socketRoles.Count(); i++)
                            role += socketRoles[i];
                    }
                    else
                    {
                        roleString = "Added";
                        var differenceQuery = list2.Except(list1);
                        var socketRoles = differenceQuery as SocketRole[] ?? differenceQuery.ToArray();
                        for (var i = 0; i < socketRoles.Count(); i++)
                            role += socketRoles[i];
                    }

                    var log = await before.Guild.GetAuditLogsAsync(1).FlattenAsync();
                    var audit = log.ToList();

                    embed.WithColor(255, 255, 0);
                    embed.WithTimestamp(DateTimeOffset.UtcNow);
                    embed.AddField($"👑 Role Update (Role {roleString}):",

                        $"User: **{before.Username} {before.Id}**\n" +
                        $"Server: **{before.Guild.Name}**\n" +
                        $"Role ({roleString}): **{role}**");
                    if (audit[0].Action == ActionType.MemberRoleUpdated)
                        embed.AddField("WHO:", $"{audit[0].User.Mention}\n");
                    embed.WithThumbnailUrl($"{after.GetAvatarUrl()}");


                    if (guild.ServerActivityLog == 1)
                    {
                        await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                            .SendMessageAsync("", false, embed.Build());
                    }
                }

            }
            catch
            {
                // ignored
            }

        }

        public static async Task Client_GuildMemberUpdated(SocketGuildUser before, SocketGuildUser after)
        {
            GuildMemberUpdated(before, after);
            await Task.CompletedTask;
        }

        public static async Task MessageUpdated(Cacheable<IMessage, ulong> messageBefore,
            SocketMessage messageAfter, ISocketMessageChannel arg3)
        {
            try
            {
                var before = (messageBefore.HasValue ? messageBefore.Value : null) as IUserMessage;
                if (arg3 is IGuildChannel currentIGuildChannel)
                {
                    var guild = GlobalGuildAccounts.GetGuildAccount(currentIGuildChannel.Guild.Id);
                    if (messageAfter.Author.IsBot)
                        return;

                    var after = messageAfter as IUserMessage;

                    if (messageAfter.Content == null)
                    {
                        return;
                    }

                    if (before == null)
                        return;


                    if (before.Content == after?.Content)
                        return;


                    var embed = new EmbedBuilder();
                    embed.WithColor(Color.Green);
                    embed.WithFooter($"MessId: {messageBefore.Id}");
                    embed.WithThumbnailUrl($"{messageBefore.Value.Author.GetAvatarUrl()}");
                    embed.WithTimestamp(DateTimeOffset.UtcNow);
                    embed.WithTitle($"📝 Updated Message");
                    embed.WithDescription($"Where: <#{before.Channel.Id}>" +
                                          $"\nMess Author: **{after?.Author}**\n");




                    if (messageBefore.Value.Content.Length > 1000)
                    {
                        var string1 = messageBefore.Value.Content.Substring(0, 1000);

                        embed.AddField("Before:", $"{string1}");

                        if (messageBefore.Value.Content.Length <= 2000)
                        {

                            var string2 =
                                messageBefore.Value.Content.Substring(1000, messageBefore.Value.Content.Length - 1000);
                            embed.AddField("Before: Continued", $"...{string2}");

                        }
                    }
                    else if (messageBefore.Value.Content.Length != 0)
                    {
                        embed.AddField("Before:", $"{messageBefore.Value.Content}");
                    }


                    if (messageAfter.Content.Length > 1000)
                    {
                        var string1 = messageAfter.Content.Substring(0, 1000);

                        embed.AddField("After:", $"{string1}");

                        if (messageAfter.Content.Length <= 2000)
                        {

                            var string2 =
                                messageAfter.Content.Substring(1000, messageAfter.Content.Length - 1000);
                            embed.AddField("After: Continued", $"...{string2}");

                        }
                    }
                    else if (messageAfter.Content.Length != 0)
                    {
                        embed.AddField("After:", $"{messageAfter.Content}");
                    }
                if (messageBefore.Value.Attachments.Any())
                {

                    var temp = messageBefore.Value.Attachments.FirstOrDefault()?.Url;
                    var check2 = $"{temp?.Substring(temp.Length - 8, 8)}";
                    var output = check2.Substring(check2.IndexOf('.') + 1);

               


                    if (messageBefore.Value.Attachments.Count == 1)
                    {
                        if (output == "png" || output == "jpg" || output == "gif")
                        {
                            embed.WithImageUrl(
                                $"attachment://{Path.GetFileName($"Attachments/{currentIGuildChannel.GuildId}/{messageBefore.Id}.{output}")}");
                            if (guild.ServerActivityLog == 1)
                            {

                                await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                                    .SendFileAsync(
                                        $"Attachments/{currentIGuildChannel.GuildId}/{messageBefore.Id}.{output}",
                                        "",
                                        embed: embed.Build());
                            }
                        }
                        else
                        {
                            if (guild.ServerActivityLog == 1)
                            {

                                await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                                    .SendMessageAsync("", false, embed.Build());
                                await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                                    .SendFileAsync(
                                        $@"Attachments/{currentIGuildChannel.GuildId}/{messageBefore.Id}.{output}",
                                        $"");
                            }
                        }
                    }
                    else
                    {
                        var sent = 0;
                        for (var i = 0; i < messageBefore.Value.Attachments.Count; i++)
                        {
                            var tempMulty = messageBefore.Value.Attachments.ToList();
                            var checkMulty = $"{tempMulty[i].Url.Substring(tempMulty[i].Url.Length - 8, 8)}";
                            var outputMylty = checkMulty.Substring(checkMulty.IndexOf('.') + 1);

                            if (i == 0 && (outputMylty == "png" || outputMylty == "jpg" || outputMylty == "gif"))
                            {
                                sent = 1;
                                embed.WithImageUrl(
                                    $"attachment://{Path.GetFileName($"Attachments/{currentIGuildChannel.GuildId}/{messageBefore.Id}-{i + 1}.{outputMylty}")}");

                                if (guild.ServerActivityLog == 1)
                                {

                                    await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                                        .SendFileAsync(
                                            $"Attachments/{currentIGuildChannel.GuildId}/{messageBefore.Id}-{i + 1}.{outputMylty}",
                                            "",
                                            embed: embed.Build());
                                }
                            }
                            else
                            {
                                if (guild.ServerActivityLog == 1)
                                {
                                    if (sent != 1)
                                        await Global.Client.GetGuild(guild.Id)
                                            .GetTextChannel(guild.LogChannelId)
                                            .SendMessageAsync("", false, embed.Build());
                                    await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                                        .SendFileAsync(
                                            $@"Attachments/{currentIGuildChannel.GuildId}/{messageBefore.Id}-{i + 1}.{
                                                    outputMylty
                                                }",
                                            "");
                                }

                                sent = 1;
                            }
                        }
                    }
                }    

                    if (guild.ServerActivityLog == 1)
                    {

                        await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                            .SendMessageAsync("", false, embed.Build());
                    }
                }
            }
            catch
            {
                //  Console.WriteLine("Cath messupd");
            }

        }


        public static async Task Client_MessageUpdated(Cacheable<IMessage, ulong> messageBefore,
            SocketMessage messageAfter, ISocketMessageChannel arg3)
        {
            MessageUpdated(messageBefore, messageAfter, arg3);
            await Task.CompletedTask;

        }

        // This Function, downloads all the attachments it saw, saves it as "Mess_ID" and if anyone edits message or delete it, the bot will pull the file from the drive with that name
        // to post it as well. multiple files handled as well.

       public static async Task MessageReceivedDownloadAttachment(SocketMessage arg)
      {
          try
          {
              if (arg.Attachments.Count == 1)
              {

                  var ll = arg.Channel as IGuildChannel;

                  Directory.CreateDirectory($@"Attachments");
                  Directory.CreateDirectory($@"Attachments/{ll?.GuildId}");

                  var temp = arg.Attachments.FirstOrDefault()?.Url;
                  if (!arg.Attachments.Any())
                      return;
                  var check = $"{temp?.Substring(temp.Length - 8, 8)}";
                  var output = check.Substring(check.IndexOf('.') + 1);

                  if (output == "png" || output == "jpg" || output == "gif")
                  {
                      using (var client = new WebClient())
                      {
                          client.DownloadFileAsync(new Uri(arg.Attachments.FirstOrDefault()?.Url),
                              $@"Attachments/{ll?.GuildId}/{arg.Id}.{output}");
                      }
                  }
                  else
                  {
                      // Console.WriteLine(output);
                      using (var client = new WebClient())
                      {
                          client.DownloadFileAsync(new Uri(arg.Attachments.FirstOrDefault()?.Url),
                              $@"Attachments/{ll?.GuildId}/{arg.Id}.{output}");
                      }
                  }
              }
              else
              {
                  for (var i = 0; i < arg.Attachments.Count; i++)
                  {
                      var ll = arg.Channel as IGuildChannel;

                      Directory.CreateDirectory($@"Attachments");
                      Directory.CreateDirectory($@"Attachments/{ll?.GuildId}");

                      var temp = arg.Attachments.ToList();



                      var check = $"{temp[i].Url.Substring(temp[i].Url.Length - 8, 8)}";
                      var output = check.Substring(check.IndexOf('.') + 1);

                      if (output == "png" || output == "jpg" || output == "gif")
                      {
                          using (var client = new WebClient())
                          {
                              client.DownloadFileAsync(new Uri(temp[i].Url),
                                  $@"Attachments/{ll?.GuildId}/{arg.Id}-{i + 1}.{output}");
                          }
                      }
                      else
                      {
                          // Console.WriteLine(output);
                          using (var client = new WebClient())
                          {
                              client.DownloadFileAsync(new Uri(temp[i].Url),
                                  $@"Attachments/{ll?.GuildId}/{arg.Id}-{i + 1}.{output}");
                          }
                      }
                  }
              }

              await Task.CompletedTask;
          }
          catch
          {
              //
          }
      }
     

       public static async Task Client_MessageReceived(SocketMessage arg)
      {

          if (arg.Author.Id == Global.Client.CurrentUser.Id)
              return;

           MessageReceivedDownloadAttachment(arg);
          await Task.CompletedTask;
      }
       

        public static async Task DeleteLogg(Cacheable<IMessage, ulong> messageBefore,
            ISocketMessageChannel arg3)
        {
            try
            {
                if (messageBefore.Value.Author.IsBot)
                    return;
                if (messageBefore.Value.Channel is ITextChannel kek)
                {
                    var guild = GlobalGuildAccounts.GetGuildAccount(kek.Guild.Id);

                    var log = await kek.Guild.GetAuditLogAsync(1);
                    var audit = log.ToList();

                    var name = $"{messageBefore.Value.Author.Mention}";
                    var check = audit[0].Data as MessageDeleteAuditLogData;

                    if (check?.ChannelId == messageBefore.Value.Channel.Id &&
                        audit[0].Action == ActionType.MessageDeleted)
                        name = $"{audit[0].User.Mention}";

                    var embedDel = new EmbedBuilder();

                    embedDel.WithFooter($"MessId: {messageBefore.Id}");
                    embedDel.WithTimestamp(DateTimeOffset.UtcNow);
                    embedDel.WithThumbnailUrl($"{messageBefore.Value.Author.GetAvatarUrl()}");

                    embedDel.WithColor(Color.Red);
                    embedDel.WithTitle($"🗑 Deleted Message");
                    embedDel.WithDescription($"Where: <#{messageBefore.Value.Channel.Id}>\n" +
                                             $"WHO: **{name}** (not always correct)\n" +
                                             $"Mess Author: **{messageBefore.Value.Author}**\n");


                    if (messageBefore.Value.Content.Length > 1000)
                    {
                        var string1 = messageBefore.Value.Content.Substring(0, 1000);

                        embedDel.AddField("Content1", $"{string1}");

                        if (messageBefore.Value.Content.Length <= 2000)
                        {

                            var string2 =
                                messageBefore.Value.Content.Substring(1000, messageBefore.Value.Content.Length - 1000);
                            embedDel.AddField("Continued", $"...{string2}");

                        }
                    }
                    else if (messageBefore.Value.Content.Length != 0)
                    {
                        embedDel.AddField("Content", $"{messageBefore.Value.Content}");
                    }

                    if (messageBefore.Value.Attachments.Any())
                    {

                        var temp = messageBefore.Value.Attachments.FirstOrDefault()?.Url;
                        var check2 = $"{temp?.Substring(temp.Length - 8, 8)}";
                        var output = check2.Substring(check2.IndexOf('.') + 1);
                       
                        var currentIGuildChannel = arg3 as IGuildChannel;


                        if (messageBefore.Value.Attachments.Count == 1)
                        {
                            if (output == "png" || output == "jpg" || output == "gif")
                            {
                                embedDel.WithImageUrl(
                                    $"attachment://{Path.GetFileName($"Attachments/{currentIGuildChannel?.GuildId}/{messageBefore.Id}.{output}")}");


                               
                             
                                if (guild.ServerActivityLog == 1)
                                {

                                    await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                                        .SendFileAsync(
                                            $"Attachments/{currentIGuildChannel?.GuildId}/{messageBefore.Id}.{output}",
                                            "",
                                            embed: embedDel.Build());
                                }
                            }
                            else
                            {
 
                                if (guild.ServerActivityLog == 1)
                                {

                                    await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                                        .SendMessageAsync("", false, embedDel.Build());
                                    await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                                        .SendFileAsync(
                                            $@"Attachments/{currentIGuildChannel?.GuildId}/{messageBefore.Id}.{output}",
                                            $"");
                                }
                            }
                        }
                        else
                        {
                            var sent = 0;
                            for (var i = 0; i < messageBefore.Value.Attachments.Count; i++)
                            {
                                var tempMulty = messageBefore.Value.Attachments.ToList();
                                var checkMulty = $"{tempMulty[i].Url.Substring(tempMulty[i].Url.Length - 8, 8)}";
                                var outputMylty = checkMulty.Substring(checkMulty.IndexOf('.') + 1);

                                if (i == 0 && (outputMylty == "png" || outputMylty == "jpg" || outputMylty == "gif"))
                                {
                                    sent = 1;
                                    embedDel.WithImageUrl(
                                        $"attachment://{Path.GetFileName($"Attachments/{currentIGuildChannel?.GuildId}/{messageBefore.Id}-{i + 1}.{outputMylty}")}");

                           
                                    if (guild.ServerActivityLog == 1)
                                    {

                                        await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                                            .SendFileAsync(
                                                $"Attachments/{currentIGuildChannel?.GuildId}/{messageBefore.Id}-{i + 1}.{outputMylty}",
                                                "",
                                                embed: embedDel.Build());
                                    }
                                }
                                else
                                {

                                    if (guild.ServerActivityLog == 1)
                                    {
                                        if (sent != 1)
                                            await Global.Client.GetGuild(guild.Id)
                                                .GetTextChannel(guild.LogChannelId)
                                                .SendMessageAsync("", false, embedDel.Build());
                                        await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                                            .SendFileAsync(
                                                $@"Attachments/{currentIGuildChannel?.GuildId}/{
                                                        messageBefore.Id
                                                    }-{i + 1}.{outputMylty}",
                                                $"");
                                    }

                                    sent = 1;

                                }
                            }
                        }
                    }

                        if (guild.ServerActivityLog == 1)
                        {

                            await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                                .SendMessageAsync("", false, embedDel.Build());
                        }
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

        }

        public static async Task Client_MessageDeleted(Cacheable<IMessage, ulong> messageBefore,
            ISocketMessageChannel arg3)
        {
            DeleteLogg(messageBefore, arg3);
            await Task.CompletedTask;
        }


        public static async Task RoleDeleted(SocketRole arg)
        {
            try
            {

                var log = await arg.Guild.GetAuditLogsAsync(1).FlattenAsync();
                var audit = log.ToList();
                var check = audit[0].Data as RoleDeleteAuditLogData;
                var name = "erorr";

                if (check?.RoleId == arg.Id)
                {
                    name = audit[0].User.Mention;
                }

                var embed = new EmbedBuilder();
                embed.WithColor(240, 51, 255);
                embed.AddField("⚰️ Role Deleted", $"WHO: {name}\n" +

                                                  $"Name: {arg.Name} ({arg.Guild})\n" +
                                                  $"Color: {arg.Color}\n" +
                                                  $"ID: {arg.Id}\n");
                embed.WithTimestamp(DateTimeOffset.UtcNow);

                embed.WithThumbnailUrl($"{audit[0].User.GetAvatarUrl()}");


                var guild = GlobalGuildAccounts.GetGuildAccount(arg.Guild.Id);

                if (guild.ServerActivityLog == 1)
                {
                    await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("", false, embed.Build());
                }
            }
            catch
            {
                //
            }

        }

        public static async Task Client_RoleDeleted(SocketRole arg)
        {
            RoleDeleted(arg);
            await Task.CompletedTask;
        }

        // Does not work fully correctly, PM me if you can fix it
        public static async Task RoleUpdated(SocketRole arg1, SocketRole arg2)
        {
            try
            {
                var before = arg1;
                var after = arg2;
                if (after == null)
                    return;
                if (before == after)
                    return;


                var roleString = "nothing";
                var list1 = before.Permissions.ToList();
                var list2 = after.Permissions.ToList();
                var role = "\n";

                if (list1.Count > list2.Count)
                {
                    roleString = "Removed";
                    var differenceQuery = list1.Except(list2);
                    var socketRoles = differenceQuery as GuildPermission[] ?? differenceQuery.ToArray();
                    for (var i = 0; i < socketRoles.Count(); i++)
                        role += $"{socketRoles[i]}\n";
                }
                else if (list1.Count < list2.Count)
                {
                    roleString = "Added";
                    var differenceQuery = list2.Except(list1);
                    var socketRoles = differenceQuery as GuildPermission[] ?? differenceQuery.ToArray();
                    for (var i = 0; i < socketRoles.Count(); i++)
                        role += $"{socketRoles[i]}\n";
                }

                var extra = "";
                if (before.Name != after.Name)
                {
                    extra += $"__**Before:**__\n" +
                             $"Name: **{before}**\n";
                    if (before.Color.ToString() != after.Color.ToString())
                    {
                        extra += $"Color: {before.Color}\n";
                    }

                    extra += $"__**After:**__\n" +
                             $"Name: **{after}**\n";
                    if (before.Color.ToString() != after.Color.ToString())
                    {
                        extra += $"Color: {after.Color}\n";
                    }

                }
                else if (before.Color.ToString() != after.Color.ToString())
                {
                    extra += $"__**Before:**__\n";
                    extra += $"Color: {before.Color}\n";
                    extra += $"__**After:**__\n";
                    extra += $"Color: {after.Color}\n";

                }

                var log = await before.Guild.GetAuditLogsAsync(1).FlattenAsync();
                var audit = log.ToList();
                var check = audit[0].Data as RoleUpdateAuditLogData;
                var name = "error";
                if (check?.After.Name == arg2.Name)
                {
                    name = audit[0].User.Mention;
                }

                var guild = GlobalGuildAccounts.GetGuildAccount(before.Guild.Id);
                var embed = new EmbedBuilder();
                embed.WithColor(57, 51, 255);
                embed.AddField($"🛠️ Role Updated({roleString})", $"Role: {after.Mention}\n" +
                                                                  $"WHO: {name}\n" +
                                                                  $"ID: {before.Id}\n" +
                                                                  $"Guild: {before.Guild.Name}\n" +
                                                                  $"{extra}" +
                                                                  $"Permission ({roleString}): **{role}**");
                embed.WithTimestamp(DateTimeOffset.UtcNow);
                embed.WithThumbnailUrl($"{audit[0].User.GetAvatarUrl()}");


                var s = role.Replace("\n", "");
                if (s.Length < 1 && extra.Length < 1)
                    return;





                if (guild.ServerActivityLog == 1)
                {
                    await Global.Client.GetGuild(guild.Id).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("", false, embed.Build());
                }
            }
            catch
            {
                //
            }
        }

        public static async Task Client_RoleUpdated(SocketRole arg1, SocketRole arg2)
        {
            RoleUpdated(arg1, arg2);
            await Task.CompletedTask;

        }

        public static async Task Client_UserJoined_ForRoleOnJoin(SocketGuildUser arg)
        {
            var guid = GlobalGuildAccounts.GetGuildAccount(arg.Guild.Id);

            if (guid.RoleOnJoin == null)
                return;

            var roleToGive = arg.Guild.Roles
                .SingleOrDefault(x => x.Name.ToString() == $"{guid.RoleOnJoin}");

            await arg.AddRoleAsync(roleToGive);
        }
    }
}