using FalloutRPG.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FalloutRPG.Models
{
    public class NpcPreset : BaseModel
    {
        public NpcPreset()
        {
            InitialInventory = new List<Item>();
        }

        public string Name { get; set; }

        public bool Enabled { get; set; }

        public virtual Special Special { get; set; }

        public Globals.SkillType Tag1 { get; set; }
        public Globals.SkillType Tag2 { get; set; }
        public Globals.SkillType Tag3 { get; set; }

        public virtual ICollection<Item> InitialInventory { get; set; }
    }
}
