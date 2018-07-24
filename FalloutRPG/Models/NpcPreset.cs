using System;

namespace FalloutRPG.Models
{
    public class NpcPreset : BaseModel
    {
        public string Name { get; set; }

        public Special InitialSpecial { get; set; }
        public SkillSheet InitialSkills { get; set; }
    }
}
