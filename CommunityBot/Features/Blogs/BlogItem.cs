using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityBot.Features.Blogs
{
    public class BlogItem
    {
        public Guid BlogId { get; set; }
        public ulong Author { get; set; }
        public List<ulong> Subscribers { get; set; }
        public string Name { get; set; }
    }
}
