using System;
using System.Collections.Generic;
using System.Text;

namespace FalloutRPG.Models
{
    public class PlayerCharacter : Character
    {
        private PlayerCharacter() { }

        public PlayerCharacter(Player player)
        {
            Player = player;
        }

        public virtual Player Player { get; set; }

        public bool Active { get; set; }

        public string Description { get; set; }
        public string Story { get; set; }

        public float SkillPoints { get; set; }
        public bool IsReset { get; set; }
    }
}
