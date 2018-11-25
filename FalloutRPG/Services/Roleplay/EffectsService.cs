using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using FalloutRPG.Models.Effects;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Services.Roleplay
{
    public class EffectsService
    {
        private readonly CharacterService _characterService;
        private readonly IRepository<Effect> _effectsRepository;
        private readonly SkillsService _skillsService;
        private readonly SpecialService _specialService;

        public EffectsService(
            CharacterService characterService,
            IRepository<Effect> effectsRepository,
            SkillsService skillsService,
            SpecialService specialService)
        {
            _characterService = characterService;
            _effectsRepository = effectsRepository;
            _skillsService = skillsService;
            _specialService = specialService;
        }

        public async Task CreateEffectAsync(string name, ulong ownerId) =>
            await _effectsRepository.AddAsync(new Effect { Name = name, OwnerId = ownerId, SkillAdditions = new List<EffectSkill>(), SpecialAdditions = new List<EffectSpecial>() });

        public async Task SaveEffectAsync(Effect effect) =>
            await _effectsRepository.SaveAsync(effect);

        public async Task DeleteEffectAsync(Effect effect) =>
            await _effectsRepository.DeleteAsync(effect);

        public async Task<Effect> GetEffectAsync(string name) =>
            await _effectsRepository.Query.Where(x => x.Name.Equals(name))
            .Include(x => x.SkillAdditions)
            .Include(x => x.SpecialAdditions)
            .FirstOrDefaultAsync();

        public async Task<bool> IsDuplicateName(string name) =>
            await _effectsRepository.Query.AnyAsync(x => x.Name.Equals(name));

        public async Task<List<Effect>> GetAllOwnedEffectsAsync(ulong ownerId) =>
            await _effectsRepository.Query.Where(x => x.OwnerId.Equals(ownerId))
            .Include(x => x.SkillAdditions)
            .Include(x => x.SpecialAdditions)
            .ToListAsync();

        public async Task<int> CountEffectsAsync(ulong ownerId) =>
            await _effectsRepository.Query.CountAsync(x => x.OwnerId.Equals(ownerId));

        public SkillSheet GetEffectiveSkills(Character character)
        {
            var newSkills = _skillsService.CloneSkills(character.Skills);

            foreach (var effect in character.Effects)
            {
                foreach (var skillEffect in effect.SkillAdditions)
                {
                    var newValue = _skillsService.GetSkill(newSkills, skillEffect.Skill) + skillEffect.EffectValue;
                    if (newValue < 1) newValue = 1;
                    _skillsService.SetSkill(newSkills, skillEffect.Skill, newValue);
                }
            }

            return newSkills;
        }

        public Special GetEffectiveSpecial(Character character)
        {
            var newSpecial = _specialService.CloneSpecial(character.Special);

            foreach (var effect in character.Effects)
                foreach (var specialEffect in effect.SpecialAdditions)
                {
                    var newValue = _specialService.GetSpecial(newSpecial, specialEffect.SpecialAttribute) + specialEffect.EffectValue;
                    if (newValue < 1) newValue = 1;
                    _specialService.SetSpecial(newSpecial, specialEffect.SpecialAttribute, newValue);
                }

            return newSpecial;
        }

        public string GetEffectInfo(Effect effect)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"__{effect.Name}__:\n");

            if (effect.SpecialAdditions != null && effect.SpecialAdditions.Count > 0)
            {
                sb.Append("**S.P.E.C.I.A.L.:** ");
                foreach (var entry in effect.SpecialAdditions)
                {
                    if (entry.EffectValue > 0)
                        sb.Append($"{entry.SpecialAttribute.ToString()}: +{entry.EffectValue} ");
                    else
                        sb.Append($"{entry.SpecialAttribute.ToString()}: {entry.EffectValue} ");
                }
            }

            if (effect.SkillAdditions != null && effect.SkillAdditions.Count > 0)
            {
                sb.Append("\n**Skills:** ");
                foreach (var entry in effect.SkillAdditions)
                {
                    if (entry.EffectValue > 0)
                        sb.Append($"{entry.Skill.ToString()}: +{entry.EffectValue} ");
                    else
                        sb.Append($"{entry.Skill.ToString()}: {entry.EffectValue} ");
                }
            }

            return sb.ToString();
        }

        public string GetCharacterEffects(Character character)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var effect in character.Effects)
                sb.Append($"{GetEffectInfo(effect)}\n**------**\n");

            return sb.ToString();
        }
    }
}
