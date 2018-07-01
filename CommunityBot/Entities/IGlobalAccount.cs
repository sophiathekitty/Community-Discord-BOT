using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityBot.Entities
{
    public interface IGlobalAccount : IEquatable<IGlobalAccount>
    {
        ulong Id { get; }
        Dictionary<string, string> Tags { get; }
    }
}
