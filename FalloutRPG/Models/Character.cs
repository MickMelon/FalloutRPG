using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FalloutRPG.Models
{
    public abstract class Character : BaseModel
    {
        public string Name { get; set; }

        public Campaign Campaign { get; set; }

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

        public Special Special { get; set; }
        public SkillSheet Skills { get; set; }

        public long Money { get; set; }

        public List<Item> Inventory { get; set; }
    }
}
