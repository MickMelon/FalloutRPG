using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FalloutRPG.Services.Roleplay
{
    public class SkillsService
    {
        private const int DEFAULT_SKILL_POINTS = 10;

        public const int MIN_TAG = 0;
        public const int MAX_TAG = 6;
        public const int POINTS_TAG = 36;

        public const int MAX_SKILL_LEVEL = 12;

        private readonly CharacterService _charService;
        private readonly SpecialService _specService;
        private readonly StatisticsService _statService;

        private readonly IConfiguration _config;
        
        public IReadOnlyCollection<Skill> Skills { get => (ReadOnlyCollection<Skill>)_statService.Statistics.OfType<Skill>(); }
        private readonly IReadOnlyDictionary<int, int> _skillPrices;

        public SkillsService(
            CharacterService charService, 
            SpecialService specService,
            StatisticsService statService,
            IConfiguration config)
        {
            _charService = charService;
            _specService = specService;
            _statService = statService;

            _config = config;

            _skillPrices = JsonConvert.DeserializeObject<Dictionary<int, int>>(_config["roleplay:skill-prices"]);
        }

        /// <summary>
        /// Used during chargen to set initial (tag) skills
        /// </summary>
        /// <param name="character">The character to set tag skills</param>
        /// <param name="tag">The skill to tag</param>
        /// <param name="points">The value to set 'tag' equal to.</param>
        /// <returns>Remaining points.</returns>
        public async Task<int> TagSkill(Character character, Skill tag, int points)
        {
            if (character == null) throw new ArgumentNullException("character");

            if (!IsTagInRange(character.Skills, points))
                throw new ArgumentException(Exceptions.CHAR_TAGS_OUT_OF_RANGE);

            // Refund skill points used if overwriting the same skill
            character.TagPoints += _statService.GetStatistic(character, tag);

            if (character.TagPoints - points < 0)
                throw new Exception(Exceptions.CHAR_NOT_ENOUGH_SKILL_POINTS);

            _statService.SetStatistic(character, tag, points);
            character.TagPoints -= points;

            await _charService.SaveCharacterAsync(character);
            return character.TagPoints;
        }

        private bool IsTagInRange(IList<StatisticValue> skills, int points)
        {
            if (points < MIN_TAG || points > MAX_TAG)
                return false;

            // Unique MUSH rules :/
            if (skills.Where(sk => sk.Value == MAX_TAG).Count() > 2)
                return false;

            if (points == MAX_TAG && skills.Where(sk => sk.Value == MAX_TAG).Count() >= 2)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if character's skills are tagged.
        /// </summary>
        public bool AreSkillsTagged(IList<StatisticValue> skillSheet)
        {
            if (skillSheet == null)
                return false;
            if (skillSheet.Where(x => x.Statistic is Skill).Sum(x => x.Value) >= POINTS_TAG)
                return true;

            return false;
        }

        /// <summary>
        /// Checks if character's skills are tagged.
        /// </summary>
        public bool AreSkillsTagged(Character character) =>
            AreSkillsTagged(character?.Skills);

        /// <summary>
        /// Calculate skill points given on level up.
        /// </summary>
        /// <remarks>
        /// Uses the Fallout New Vegas formula. (10 + (INT / 2))
        /// </remarks>
        public int CalculateSkillPoints(int intelligence)
        {
            return DEFAULT_SKILL_POINTS + (intelligence / 2);
        }

        public int CalculateSkillPointsForLevel(int intelligence, int level)
        {
            return CalculateSkillPoints(intelligence) * (level - 1);
        }

        /// <summary>
        /// Puts an amount of points in a specified skill.
        /// </summary>
        public void UpgradeSkill(Character character, Skill skill)
        {
            if (character == null) throw new ArgumentNullException("character");

            int points = 0;
            if (points > character.ExperiencePoints)
                throw new Exception(Exceptions.CHAR_NOT_ENOUGH_SKILL_POINTS);

            var skillVal = _statService.GetStatistic(character, skill);

            if ((skillVal + points) > MAX_SKILL_LEVEL)
                throw new Exception(Exceptions.CHAR_SKILL_POINTS_GOES_OVER_MAX);

            _statService.SetStatistic(character, skill, skillVal + points);
            character.ExperiencePoints -= points;
        }

        /// <summary>
        /// Checks if the special name is valid.
        /// </summary>
        private bool IsValidSkillName(string skill)
        {
            return Skills.Select(sk => sk.AliasesArray).Any(aliases => aliases.Contains(skill));
        }

        private int CalculatePrice(int skillLevel, int charLevel)
        {
            double multiplier = 1.0;

            if (charLevel > 10)
                for (int i = 0; i < (charLevel - 5) / 5; i++)
                    multiplier += .5;

            return (int)(_skillPrices[skillLevel] * multiplier);
        }

        /// <summary>
        /// Checks if a character's special has been set.
        /// </summary>
        public bool AreSkillsSet(Character character)
        {
            if (character == null || character.Statistics == null) return false;
            if (character.Skills.Count() <= 0) return false;

            return true;
        }
    }
}
