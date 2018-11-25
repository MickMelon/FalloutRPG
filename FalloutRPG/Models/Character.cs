using FalloutRPG.Models.Effects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FalloutRPG.Models
{
    public class Character : BaseModel
    {
        public ulong DiscordId { get; set; }
        public bool Active { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public string Story { get; set; }

        public int Experience { get; set; }
        public int Level
        {
            get
            {
                if (Experience == 0) return 1;
                return Convert.ToInt32((Math.Sqrt(Experience + 125) / (10 * Math.Sqrt(5))));
            }

            private set { }
        }

        public float SkillPoints { get; set; }
        public bool IsReset { get; set; }

        public Special Special { get; set; }
        public SkillSheet Skills { get; set; }

        public virtual IList<Effect> Effects { get; set; }

        public long Money { get; set; }
    }
}
