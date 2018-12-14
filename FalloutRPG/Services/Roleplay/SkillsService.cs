using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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

        private const int TAG_ADDITION = 15;

        public const int MAX_SKILL_LEVEL = 200;

        private readonly CharacterService _charService;
        private readonly SpecialService _specService;
        private readonly StatisticsService _statService;
        
        public IReadOnlyCollection<Skill> Skills { get => (ReadOnlyCollection<Skill>)_statService.Statistics.OfType<Skill>(); }

        public SkillsService(
            CharacterService charService, 
            SpecialService specService,
            StatisticsService statService)
        {
            _charService = charService;
            _specService = specService;
            _statService = statService;
        }

        /// <summary>
        /// Set character's tag skills.
        /// </summary>
        public async Task SetTagSkills(Character character, Skill tag1, Skill tag2, Skill tag3)
        {
            if (character == null) throw new ArgumentNullException("character");

            if (!_specService.IsSpecialSet(character))
                throw new Exception(Exceptions.CHAR_SPECIAL_NOT_FOUND);

            if (!AreUniqueTags(tag1, tag2, tag3))
                throw new ArgumentException(Exceptions.CHAR_TAGS_NOT_UNIQUE);

            InitializeSkills(character);

            SetTagSkill(character, tag1);
            SetTagSkill(character, tag2);
            SetTagSkill(character, tag3);

            await _charService.SaveCharacterAsync(character);
        }

        /// <summary>
        /// Returns the value of the specified character's given skill.
        /// </summary>
        /// <returns>Returns 0 if character or skills are null.</returns>
        public int GetSkill(IList<StatisticValue> skillSheet, Skill skill)
        {
            var match = skillSheet.FirstOrDefault(x => x.Statistic.Equals(skill));

            if (match == null)
                return -1;

            return match.Value;
        }

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
        public void PutPointsInSkill(Character character, Skill skill, int points)
        {
            if (character == null) throw new ArgumentNullException("character");

            if (points < 1) return;

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

        /// <summary>
        /// Checks if all the tags are unique.
        /// </summary>
        private bool AreUniqueTags(Skill tag1, Skill tag2, Skill tag3)
        {
            if (tag1.Equals(tag2) ||
                tag1.Equals(tag3) ||
                tag2.Equals(tag3))
                return false;

            return true;
        }

        /// <summary>
        /// Sets a character's tag skill.
        /// </summary>
        public void SetTagSkill(Character character, Skill tag)
        {
            _statService.SetStatistic(character, tag, _statService.GetStatistic(character, tag) + TAG_ADDITION);
        }

        /// <summary>
        /// Initializes a character's skills.
        /// </summary>
        public void InitializeSkills(Character character)
        {
            foreach (var skill in Skills)
            {
                character.Statistics.Add(
                    new StatisticValue
                    {
                        Statistic = skill,
                        Value = CalculateSkill(_statService.GetStatistic(character, skill.Special))
                    });
            }
        }

        /// <summary>
        /// Calculates a skill based on New Vegas formula.
        /// </summary>
        private int CalculateSkill(int stat, int luck = 0)
        {
            return (2 + (2 * stat) + (luck / 2));
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