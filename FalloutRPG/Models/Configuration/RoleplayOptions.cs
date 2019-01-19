using System;
using System.Collections.Generic;
using System.Text;

namespace FalloutRPG.Models.Configuration
{
    public class RoleplayOptions
    {
        public int CharacterMax { get; set; }

        public int SkillMax { get; set; }
        public int SpecialMax { get; set; }

        public bool UsePercentage { get; set; }

        public bool UseOldProbability { get; set; }
    }
}
