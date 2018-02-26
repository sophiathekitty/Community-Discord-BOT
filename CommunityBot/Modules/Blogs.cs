using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityBot.Configuration;
using CommunityBot.Features.Blogs;
using CommunityBot.Handlers;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace CommunityBot.Modules
{
    [Group("Blog")]
    public class Blogs : ModuleBase<SocketCommandContext>
    {
        private static readonly string blogFile = "blogs.json";
        [Command("Create")]
        public async Task Create(string name)
        {
            await Context.Message.DeleteAsync();

            var blogs = DataStorage.RestoreObject<List<BlogItem>>(blogFile) ?? new List<BlogItem>();

            if (blogs.FirstOrDefault(k=>k.Name == name) == null)
            {
                var newBlog = new BlogItem
                {
                    BlogId = Guid.NewGuid(),
                    Author = Context.User.Id,
                    Name = name,
                    Subscribers = new List<ulong>()
                };

                blogs.Add(newBlog);

                DataStorage.StoreObject(blogs, blogFile, Formatting.Indented);
                
                var embed = EmbedHandler.CreateEmbed("Blog", $"Your blog {name} was created.", EmbedHandler.EmbedMessageType.Success);
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            else
            {
                var embed = EmbedHandler.CreateEmbed("Blog :x:", $"There is already a Blog with the name {name}", EmbedHandler.EmbedMessageType.Error);
                await Context.Channel.SendMessageAsync("", false, embed);
            }
        }

        [Command("Post")]
        public async Task Post(string name, [Remainder]string post)
        {
            await Context.Message.DeleteAsync();

            var blogs = DataStorage.RestoreObject<List<BlogItem>>(blogFile);

            var blog = blogs.FirstOrDefault(k => k.Name == name && k.Author == Context.User.Id);

            if (blog != null)
            {
                var subs = string.Empty;
                foreach (var subId in blog.Subscribers)
                {
                    var sub = Context.Guild.GetUser(subId);
                    
                    subs += $"{sub.Mention},";
                }

                if (string.IsNullOrEmpty(subs))
                {
                    subs = "No subs";
                }

                var embed = EmbedHandler.CreateBlogEmbed(blog.Name, post, subs, EmbedHandler.EmbedMessageType.Info, true);
                var msg = Context.Channel.SendMessageAsync("", false, embed);
                
                if (Global.MessagesIdToTrack == null)
                {
                    Global.MessagesIdToTrack = new Dictionary<ulong, string>();
                }

                Global.MessagesIdToTrack.Add(msg.Result.Id, blog.Name);

                await msg.Result.AddReactionAsync(new Emoji("➕"));
            }
        }

        [Command("Subscribe")]
        public async Task Subscribe(string name)
        {
            await Context.Message.DeleteAsync();

            var embed = BlogHandler.SubscribeToBlog(Context.User.Id, name);

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("Unsubscribe")]
        public async Task UnSubscribe(string name)
        {
            await Context.Message.DeleteAsync();

            var embed = BlogHandler.UnSubscribeToBlog(Context.User.Id, name);

            await Context.Channel.SendMessageAsync("", false, embed);
        }
    }
}
