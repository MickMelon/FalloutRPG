using System;
using System.Collections.Generic;

namespace FalloutRPG.Constants
{
    public class Globals
    {
        public enum StatisticFlag
        {
            None = 0,

            Strength,
            Perception,
            Endurance,
            Charisma,
            Intelligence,
            Agility,
            Luck,

            Barter,
            EnergyWeapons,
            Explosives,
            Guns,
            Lockpick,
            Medicine,
            MeleeWeapons,
            Repair,
            Science,
            Sneak,
            Speech,
            Survival,
            Unarmed
        }

        public const int RATELIMIT_SECONDS = 2;
        public const int RATELIMIT_TIMES = 3;
    }
}
