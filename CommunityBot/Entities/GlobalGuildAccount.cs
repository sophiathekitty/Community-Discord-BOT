using System.Collections.Generic;
using CommunityBot.Features.RoleAssignment;

namespace CommunityBot.Entities
{
    public class GlobalGuildAccount : IGlobalAccount
    {
        public GlobalGuildAccount(ulong id)
        {
            Id = id;
        }
        public ulong Id { get; }

        public ulong AnnouncementChannelId { get; set; }

        public List<string> Prefixes { get; set; } = new List<string>();

        public List<string> WelcomeMessages { get; set; } = new List<string> { };

        public List<string> LeaveMessages { get; set; } = new List<string>();

        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();

        public Modules.ServerBots.GuildData BotData { get; set; }

        public RoleByPhraseSettings RoleByPhraseSettings { get; set; } = new RoleByPhraseSettings();

        /* Add more values to store */

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Equals(obj as IGlobalAccount);
        }

        // implementation for IEquatable
        public bool Equals(IGlobalAccount other)
        {
            return Id == other.Id;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return unchecked((int)Id);
        }
    }
}
