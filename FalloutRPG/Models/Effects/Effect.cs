﻿using System.Collections.Generic;

namespace FalloutRPG.Models.Effects
{
    public class Effect : BaseModel
    {
        public string Name { get; set; }

        public ulong OwnerId { get; set; }

        public virtual IList<StatisticValue> StatisticEffects { get; set; }

        public virtual IList<EffectCharacter> EffectCharacters { get; set; }
    }
}
