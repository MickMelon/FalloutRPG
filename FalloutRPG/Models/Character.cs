using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FalloutRPG.Models
{
    public abstract class Character : BaseModel
    {
        public string Name { get; set; }

        public virtual Campaign Campaign { get; set; }

        public int Experience { get; set; }
        [NotMapped]
        public int Level
        {
            get
            {
                if (Experience == 0) return 1;
                return Convert.ToInt32((Math.Sqrt(Experience + 125) / (10 * Math.Sqrt(5))));
            }
        }

        public virtual Special Special { get; set; }
        public virtual SkillSheet Skills { get; set; }

        public int HitPoints { get; set; }
        [NotMapped]
        public int HitPointsLimit
        {
            get
            {
                if (Special == null || Special.Endurance < 1)
                    return -1;

                return 95 + (Special.Endurance * 20) + (Level * 5);
            }
        }

        public int ArmorClass { get; set; }

        public long Money { get; set; }

        public virtual List<Effect> Effects { get; set; }

        public virtual List<Item> Inventory { get; set; }
    }
}
