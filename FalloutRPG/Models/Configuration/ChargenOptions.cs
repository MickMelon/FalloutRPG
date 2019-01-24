using System;
using System.Collections.Generic;
using System.Text;

namespace FalloutRPG.Models.Configuration
{
    public class ChargenOptions
    {
        public int SkillPoints { get; set; }
        public int SkillLevelMax { get; set; }
        public int SkillsAtMax { get; set; }

        public int SpecialPoints { get; set; }
        public int SpecialLevelMax { get; set; }
        public int SpecialsAtMax { get; set; }
    }
}
