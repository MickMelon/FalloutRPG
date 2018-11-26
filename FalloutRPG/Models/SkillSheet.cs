namespace FalloutRPG.Models
{
    public class SkillSheet : BaseModel
    {
        public int CharacterId { get; set; }
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
    }
}
