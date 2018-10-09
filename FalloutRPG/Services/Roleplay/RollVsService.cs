using FalloutRPG.Constants;
using FalloutRPG.Models;
using System.Linq;

namespace FalloutRPG.Services.Roleplay
{
    public class RollVsService
    {
        private readonly ItemService _itemService;
        private readonly RollService _rollService;
        private readonly SkillsService _skillsService;

        public RollVsService(ItemService itemService, RollService rollService, SkillsService skillsService)
        {
            _itemService = itemService;
            _rollService = rollService;
            _skillsService = skillsService;
        }

        public bool AttackCharacter(Character sender, Character receiver)
        {
            var senderWeapon = _itemService.GetEquippedItems(sender).OfType<ItemWeapon>().Single();
            var victimDt = _itemService.GetDamageThreshold(receiver);
            var rollResult = _rollService.GetRollResult(sender, senderWeapon.Skill);

            // >= 0 is a success, less than 0 is a failure
            if (rollResult >= 0)
            {
                double dam = senderWeapon.Damage * 
                _itemService.GetDamageSkillMultiplier(senderWeapon, _skillsService.GetSkill(sender, senderWeapon.Skill));

                if (senderWeapon.Skill == Globals.SkillType.MeleeWeapons)
                    dam *= sender.Special.Strength * 0.5;
                if (senderWeapon.Skill == Globals.SkillType.Unarmed)
                    dam *= sender.Skills.Unarmed / 20 + 0.5;
                if (rollResult >= 95)
                {
                    // critical!
                }
                /*
                Second, Melee/Unarmed special attack multiplier, critical damage (if a critical roll succeeded) and armor reduction are added in, where:

                Damadjusted1=(Dam+isCrit×CritDmg×CritPerks)

                DTadjusted=max(0,DT×AmmoDTmult−AmmoDT) 

                Damadjusted2=max(Damadjusted1×0.2,Damadjusted1−DTadjusted) 

                isCrit = 1 if a critical hit, otherwise 0.
                CritDmg = Unmodified critical bonus damage, as seen on individual weapon pages.
                CritPerks = Perks that multiply critical damage (Better Criticals, Stealth Girl, etc.)
                DT = The Damage Threshold of the target. Note that DT cannot reduce damage (DR modified) below 20%.
                AmmoDT_mult = Ammo DT multiplier (such as the x3 of HP ammo).
                AmmoDT = Ammo DT reduction (such as the 15DT reduction of AP ammo). Note that it cannot reduce DT of the target below 0. 

            Finally, any other multiplicative modifiers are included, such as:

                Final damage=Damadjusted2×SA×LM×AM×DM×Perks×Chems 

                SA = Sneak Attack multiplier: 2 if ranged, 5 if Melee/Unarmed.
                LM = Location multiplier for ranged attacks. For example, most creatures with a head take 2x damage from headshots and ants take 0.5x damage to their legs.
                AM = Ammo type damage multiplier (such as the x1.75 of HP ammo).
                DM = Difficulty multiplier: 2 on very easy, 1 on normal difficulty, 0.5 on very hard.
                Perks = Damage multipliers from perks such as Lord Death, Bloody Mess, Living Anatomy, etc.
                Chems = Damage multipliers from Chems such as Psycho, Slasher, etc.  */
            }

            return false;
        }
    }
}