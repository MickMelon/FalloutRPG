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

        public ICollection<Player> Players { get; set; }
        public ICollection<Character> Characters { get; set; }
        public ICollection<Player> Moderators { get; set; }

        public Player Owner { get; private set; }
        public ulong RoleId { get; private set; }
        public ulong TextChannelId { get; private set; }
    }
}
