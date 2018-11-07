using FalloutRPG.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FalloutRPG.Models
{
    public class ItemWeapon : Item
    {
        public ItemWeapon()
        {
            Ammo = new List<ItemAmmo>();
        }

        public int Damage { get; set; }
        public Globals.SkillType Skill { get; set; }
        public int SkillMinimum { get; set; }
        public int StrengthMinimum { get; set; }

        public virtual ICollection<ItemAmmo> Ammo { get; set; }
        [NotMapped]
        public int AmmoRemaining { get; set; }
        public int AmmoCapacity { get; set; }
        public int AmmoOnAttack { get; set; }
    }
}
