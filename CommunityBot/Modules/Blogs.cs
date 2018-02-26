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

                await msg.Result.AddReactionAsync(new Emoji("➕"));

                var list = Global.MessagesIdToTrack ?? new Dictionary<ulong, string>();

                list.Add(msg.Result.Id, blog.Name);
            }
        }

        [Command("Subscribe")]
        public async Task Subscribe(string name)
        {
            var embed = BlogHandler.SubscribeToBlog(Context.User.Id, name);

            await Context.Channel.SendMessageAsync("", false, embed);
        }
    }
}
