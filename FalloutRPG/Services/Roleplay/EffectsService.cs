using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using FalloutRPG.Models.Effects;
using Microsoft.EntityFrameworkCore;
using System;
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
            await _effectsRepository.AddAsync(new Effect { Name = name, OwnerId = ownerId, StatisticEffects = new List<StatisticValue>() });

        public async Task SaveEffectAsync(Effect effect) =>
            await _effectsRepository.SaveAsync(effect);

        public async Task DeleteEffectAsync(Effect effect) =>
            await _effectsRepository.DeleteAsync(effect);

        public async Task<Effect> GetEffectAsync(string name) =>
            await _effectsRepository.Query.Where(x => x.Name.Equals(name))
            .Include(x => x.StatisticEffects)
            .FirstOrDefaultAsync();

        public async Task<bool> IsDuplicateName(string name) =>
            await _effectsRepository.Query.AnyAsync(x => x.Name.Equals(name));

        public async Task<List<Effect>> GetAllOwnedEffectsAsync(ulong ownerId) =>
            await _effectsRepository.Query.Where(x => x.OwnerId.Equals(ownerId))
            .Include(x => x.StatisticEffects)
            .ToListAsync();

        public async Task<int> CountEffectsAsync(ulong ownerId) =>
            await _effectsRepository.Query.CountAsync(x => x.OwnerId.Equals(ownerId));

        public IList<StatisticValue> GetEffectiveStatistics(Character character)
        {
            var newStats = _skillsService.CloneSkills(character.Skills);

            // Loop through all applied effects, then loop through every StatisticValue in the effect,
            // then actually apply them
            foreach (var effect in character.EffectCharacters.Select(x => x.Effect))
            {
                foreach (var statEffect in effect.StatisticEffects)
                {
                    foreach (var stat in newStats)
                    {
                        if (stat.Value + statEffect.Value >= 1) 
                        {
                            stat.Value += statEffect.Value;
                        }
                        else
                        {
                            stat.Value = 1;
                        }
                    }
                }
            }

            return newStats;
        }

        public string GetEffectInfo(Effect effect)
        {
            if (effect?.StatisticEffects == null || effect?.StatisticEffects.Count <= 0) return String.Empty;

            StringBuilder sb = new StringBuilder();

            sb.Append($"__{effect.Name}__:\n");

            var specEffects = effect.StatisticEffects.Where(x => x.Statistic.StatisticType == Globals.StatisticType.Special);
            if (specEffects.Count() > 0)
            {
                sb.Append("**S.P.E.C.I.A.L.:** ");

                foreach (var entry in specEffects)
                {
                    if (entry.Value > 0)
                        sb.Append($"{entry.Statistic.ToString()}: +{entry.Value} ");
                    else
                        sb.Append($"{entry.Statistic.ToString()}: {entry.Value} ");
                }
            }

            var skillEffects = effect.StatisticEffects.Where(x => x.Statistic.StatisticType == Globals.StatisticType.Skill);
            if (skillEffects.Count() > 0)
            {
                sb.Append("\n**Skills:** ");

                foreach (var entry in skillEffects)
                {
                    if (entry.Value > 0)
                        sb.Append($"{entry.Statistic.ToString()}: +{entry.Value} ");
                    else
                        sb.Append($"{entry.Statistic.ToString()}: {entry.Value} ");
                }
            }

            return sb.ToString();
        }

        public string GetCharacterEffects(Character character)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var effect in character.EffectCharacters.Select(x => x.Effect))
                sb.Append($"{GetEffectInfo(effect)}\n**------**\n");

            return sb.ToString();
        }
    }
}
