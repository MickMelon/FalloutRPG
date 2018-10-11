using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Services.Roleplay
{
    public class EffectsService
    {
        //private readonly CharacterService _characterService;
        //private readonly SkillsService _skillsService;
        //private readonly SpecialService _specialService;

        //private readonly IRepository<Effect> _effectsRepository;

        // I now realize that in order for this to work, we're also gonna need a RemoveEffect method
        // now the problem, is that there isn't a Character.Effects list or anything, so do we just apply the
        // effect in reverse to remove it? Probably should go ahead and add a NonMapped effect list...

        public double GetArmorClass(Character receiver)
        {
            throw new NotImplementedException();
        }

        //public void ApplyEffect(Character character, Effect effect)
        //{
        //    foreach (var specialEffect in effect.SpecialEffects)
        //        _specialService.SetSpecial(character,
        //            specialEffect.SpecialAttribute,
        //            _specialService.GetSpecial(character, specialEffect.SpecialAttribute) + specialEffect.EffectValue);

        //    //foreach (var skillEffect in effect.SkillEffects)
        //        //_skillsService.SetSkill()
        //}
    }
}
