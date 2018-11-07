using System.Collections.Generic;

namespace FalloutRPG.Models
{
    public class Effect : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int ArmorClassAddition { get; set; }

        public virtual Special SpecialLocks { get; set; }
        public virtual SkillSheet SkillLocks { get; set; }

        public virtual IList<EffectSpecial> SpecialAdditions { get; set; }
        public virtual IList<EffectSkill> SkillAdditions { get; set; }
    }
}
