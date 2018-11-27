using System;
using System.Collections.Generic;
using System.Text;

namespace FalloutRPG.Models.Effects
{
    public class EffectCharacter
    {
        public int EffectId { get; set; }
        public Effect Effect { get; set; }

        public int CharacterId { get; set; }
        public Character Character { get; set; }
    }
}
