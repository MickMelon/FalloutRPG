﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FalloutRPG.Models
{
    public class NpcPreset : BaseModel
    {
        public string Name { get; set; }

        public bool Enabled { get; set; }

        // Initial Skills and S.P.E.C.I.A.L.
        public int Strength { get; set; }
        public int Perception { get; set; }
        public int Endurance { get; set; }
        public int Charisma { get; set; }
        public int Intelligence { get; set; }
        public int Agility { get; set; }
        public int Luck { get; set; }

        [NotMapped]
        public Special Special
        {
            get => new Special
            {
                Strength = Strength,
                Perception = Perception,
                Endurance = Endurance,
                Charisma = Charisma,
                Intelligence = Intelligence,
                Agility = Agility,
                Luck = Luck
            };
            private set { }
        }

        public int Barter { get; set; }
        public int EnergyWeapons { get; set; }
        public int Explosives { get; set; }
        public int Guns { get; set; }
        public int Lockpick { get; set; }
        public int Medicine { get; set; }
        public int MeleeWeapons { get; set; }
        public int Repair { get; set; }
        public int Science { get; set; }
        public int Sneak { get; set; }
        public int Speech { get; set; }
        public int Survival { get; set; }
        public int Unarmed { get; set; }

        [NotMapped]
        public SkillSheet Skills
        {
            get => new SkillSheet
            {
                Barter = Barter,
                EnergyWeapons = EnergyWeapons,
                Explosives = Explosives,
                Guns = Guns,
                Lockpick = Lockpick,
                Medicine = Medicine,
                MeleeWeapons = MeleeWeapons,
                Repair = Repair,
                Science = Science,
                Sneak = Sneak,
                Speech = Speech,
                Survival = Survival,
                Unarmed = Unarmed,
            };
            private set { }
        }
    }
}
