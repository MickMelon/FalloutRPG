using System;
using System.Collections.Generic;

namespace FalloutRPG.Constants
{
    public class Globals
    {
        public enum SkillType
        {
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

        public enum SpecialType
        {
            Strength,
            Perception,
            Endurance,
            Charisma,
            Intelligence,
            Agility,
            Luck
        }

        public enum ItemType
        {
            Ammo,
            Apparel,
            Consumable,
            Misc,
            Weapon
        }

        public static readonly Dictionary<string, SkillType> SKILL_ALIASES = new Dictionary<string, SkillType>(StringComparer.OrdinalIgnoreCase)
        {
            { "Barter", SkillType.Barter },

            { "Energy", SkillType.EnergyWeapons },
            { "Energy Weapons", SkillType.EnergyWeapons },
            { "Energy Weapon", SkillType.EnergyWeapons },
            { "EnergyWeapon", SkillType.EnergyWeapons },
            { "EnergyWeapons", SkillType.EnergyWeapons },

            { "Explosives", SkillType.Explosives },

            { "Guns", SkillType.Guns },

            { "Lockpick", SkillType.Lockpick },

            { "Medicine", SkillType.Medicine },
            { "Medic", SkillType.Medicine },
            { "Doctor", SkillType.Medicine },

            { "Melee", SkillType.MeleeWeapons },
            { "MeleeWeapon", SkillType.MeleeWeapons },
            { "MeleeWeapons", SkillType.MeleeWeapons },
            { "Melee Weapons", SkillType.MeleeWeapons },

            { "Repair", SkillType.Repair },

            { "Science", SkillType.Science },

            { "Sneak", SkillType.Sneak },

            { "Speech", SkillType.Speech },

            { "Survival", SkillType.Survival },

            { "Unarmed", SkillType.Unarmed },
        };

        public static readonly Dictionary<string, SpecialType> SPECIAL_ALIASES = new Dictionary<string, SpecialType>(StringComparer.OrdinalIgnoreCase)
        {
            { "Strength", SpecialType.Strength },
            { "STR", SpecialType.Strength },

            { "Perception", SpecialType.Perception },
            { "PER", SpecialType.Perception },

            { "Endurance", SpecialType.Endurance },
            { "END", SpecialType.Endurance },

            { "Charisma", SpecialType.Charisma },
            { "CHA", SpecialType.Charisma },

            { "Intelligence", SpecialType.Intelligence },
            { "INT", SpecialType.Intelligence },

            { "Agility", SpecialType.Agility },
            { "AGI", SpecialType.Agility },
            { "AGL", SpecialType.Agility },

            { "Luck", SpecialType.Luck },
            { "LCK", SpecialType.Luck },
            { "LUC", SpecialType.Luck },
        };

        public static string[] SKILL_NAMES = new string[]
        {
            "Barter",
            "EnergyWeapons",
            "Explosives",
            "Guns",
            "Lockpick",
            "Medicine",
            "MeleeWeapons",
            "Repair",
            "Science",
            "Sneak",
            "Speech",
            "Survival",
            "Unarmed"
        };

        public static string[] SPECIAL_NAMES = new string[]
        {
            "Strength",
            "Perception",
            "Endurance",
            "Charisma",
            "Intelligence",
            "Agility",
            "Luck"
        };

        public const int RATELIMIT_SECONDS = 2;
        public const int RATELIMIT_TIMES = 3;
    }
}
