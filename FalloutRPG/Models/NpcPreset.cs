using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FalloutRPG.Models
{
    public class NpcPreset : BaseModel
    {
        public string Name { get; set; }
        //public int BaseLevel { get; set; }
        //public int MinLevel { get; set; }
        //public int MaxLevel { get; set; }

        public bool Enabled { get; set; }

        // Initial Skills and S.P.E.C.I.A.L.
        public int Strength { get; set; }
        public int Perception { get; set; }
        public int Endurance { get; set; }
        public int Charisma { get; set; }
        public int Intelligence { get; set; }
        public int Agility { get; set; }
        public int Luck { get; set; }

        // On NPC creation, +/- these amounts randomly
        public int StrengthRange { get; set; }
        public int PerceptionRange { get; set; }
        public int EnduranceRange { get; set; }
        public int CharismaRange { get; set; }
        public int IntelligenceRange { get; set; }
        public int AgilityRange { get; set; }
        public int LuckRange { get; set; }

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

        public int BarterRange { get; set; }
        public int EnergyWeaponsRange { get; set; }
        public int ExplosivesRange { get; set; }
        public int GunsRange { get; set; }
        public int LockpickRange { get; set; }
        public int MedicineRange { get; set; }
        public int MeleeWeaponsRange { get; set; }
        public int RepairRange { get; set; }
        public int ScienceRange { get; set; }
        public int SneakRange { get; set; }
        public int SpeechRange { get; set; }
        public int SurvivalRange { get; set; }
        public int UnarmedRange { get; set; }

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
