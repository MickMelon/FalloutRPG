using FalloutRPG.Constants;
using FalloutRPG.Models;
using System;
using System.Linq;

namespace FalloutRPG.Services.Roleplay
{
    public class RollVsService
    {
        private readonly EffectsService _effectsService;
        private readonly ItemService _itemService;
        private readonly RollService _rollService;
        private readonly SkillsService _skillsService;

        public RollVsService(EffectsService effectsService, ItemService itemService, RollService rollService, SkillsService skillsService)
        {
            _effectsService = effectsService;
            _itemService = itemService;
            _rollService = rollService;
            _skillsService = skillsService;
        }

        public bool AttackCharacter(Character sender, Character receiver)
        {
            var senderEquipped = _itemService.GetEquippedItems(sender);
            var senderWeapon = senderEquipped.OfType<ItemWeapon>().Single();
            var senderAmmo = senderEquipped.OfType<ItemAmmo>().Single();
            double victimDt = _itemService.GetDamageThreshold(receiver);
            var rollResult = _rollService.GetRollResult(sender, senderWeapon.Skill) - receiver.ArmorClass;
            
            // >= 0 is a success, less than 0 is a failure
            // going to roll with the FO1 & 2 system with AC
            if (rollResult >= 0)
            {
                double dam = senderWeapon.Damage * 
                _itemService.GetDamageSkillMultiplier(senderWeapon, _skillsService.GetSkill(sender, senderWeapon.Skill));

                if (senderWeapon.Skill == Globals.SkillType.MeleeWeapons)
                    dam *= sender.Special.Strength * 0.5;
                if (senderWeapon.Skill == Globals.SkillType.Unarmed)
                    dam *= sender.Skills.Unarmed / 20 + 0.5;

                // critical!
                if (rollResult >= 95)
                    dam *= 2;
                // find adjusted DT due to armor piercing / opposite of that ammo
                victimDt = Math.Max(0, victimDt * senderAmmo.DTMultiplier - senderAmmo.DTReduction);

                // armor never reduces damage completely
                dam = Math.Max(dam * 0.2, dam - victimDt);

                // perks & more ammo calculations
                dam *= senderAmmo.DamageMultiplier /* times perks */;
            }

            return false;
        }
    }
}