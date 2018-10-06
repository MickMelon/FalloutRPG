using FalloutRPG.Constants;
using System.ComponentModel.DataAnnotations.Schema;

namespace FalloutRPG.Models
{
    public class ItemWeapon : Item
    {
        public int Damage { get; set; }
        public Globals.SkillType Skill { get; set; }
        public int SkillMinimum { get; set; }

        public ItemAmmo Ammo { get; set; }
        [NotMapped]
        public int AmmoRemaining { get; set; }
        public int AmmoCapacity { get; set; }
        public int AmmoOnAttack { get; set; }
    }
}
