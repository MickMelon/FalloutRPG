using System;
using System.Collections.Generic;
using System.Text;

namespace FalloutRPG.Models
{
    public class Campaign : BaseModel
    {
        private Campaign() { }

        public Campaign(string name, Player owner, ulong roleId, ulong textChannelId)
        {
            Name = name;
            Owner = owner;
            RoleId = roleId;
            TextChannelId = textChannelId;
            Players = new List<Player>();
            Moderators = new List<Player>();
        }

        public string Name { get; private set; }

        public virtual ICollection<Player> Players { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
        public virtual ICollection<Player> Moderators { get; set; }

        public virtual Player Owner { get; private set; }
        public ulong RoleId { get; private set; }
        public ulong TextChannelId { get; private set; }
    }
}
