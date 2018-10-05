using System.Collections.Generic;

namespace FalloutRPG.Models
{
    public class Effect : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public IList<EffectSpecial> SpecialEffects { get; set; }
        public IList<EffectSkill> SkillEffects { get; set; }
    }
}
