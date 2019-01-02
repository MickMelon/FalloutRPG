using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FalloutRPG.Services.Roleplay
{
    public class SkillsService
    {
        public static int TAG_MIN = 1;
        public static int TAG_MAX = 6;
        public static int TAG_MAX_QUANTITY = 2;
        public static int TAG_POINTS = 36;

        public static int MAX_SKILL_LEVEL = 12;

        private readonly CharacterService _charService;
        private readonly ExperienceService _expService;
        private readonly SpecialService _specService;
        private readonly StatisticsService _statService;

        private readonly IConfiguration _config;
        
        public static IReadOnlyCollection<Skill> Skills { get => StatisticsService.Statistics.OfType<Skill>().ToList().AsReadOnly(); }
        private IReadOnlyDictionary<int, int> _skillPrices;

        public SkillsService(
            CharacterService charService,
            ExperienceService expService,
            SpecialService specService,
            StatisticsService statService,
            IConfiguration config)
        {
            _charService = charService;
            _expService = expService;
            _specService = specService;
            _statService = statService;

            _config = config;

            LoadSkillConfig();
        }

        void LoadSkillConfig()
        {
            try
            {
                var temp = new Dictionary<int, int>();

                foreach (var item in _config.GetSection("roleplay:skill-prices").GetChildren())
                    temp.Add(Int32.Parse(item.Key), Int32.Parse(item.Value));

                _skillPrices = temp;

                TAG_POINTS = _config.GetValue<int>("roleplay:chargen:skill-points");
                TAG_MAX = _config.GetValue<int>("roleplay:chargen:skill-level-limit");
                TAG_MAX_QUANTITY = _config.GetValue<int>("roleplay:chargen:skills-at-limit");

                MAX_SKILL_LEVEL = _config.GetValue<int>("roleplay:skill-max");
            }
            catch (Exception)
            {
                Console.WriteLine("Skill prices improperly configured, check Config.json.");
                throw;
            }
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

        public async Task TagSkills(Character character, Skill tag1, Skill tag2, Skill tag3)
        {
            if (character == null) throw new ArgumentNullException("character");

            if (!_specService.IsSpecialSet(character))
                throw new Exception(Exceptions.CHAR_SPECIAL_NOT_FOUND);

            if (!AreUniqueTags(tag1, tag2, tag3))
                throw new ArgumentException(Exceptions.CHAR_TAGS_NOT_UNIQUE);

            _statService.InitializeStatistics(character.Statistics);
            InitializeSkills(character);

            _statService.SetStatistic(character, tag1, _statService.GetStatistic(character, tag1) + 15);
            _statService.SetStatistic(character, tag2, _statService.GetStatistic(character, tag2) + 15);
            _statService.SetStatistic(character, tag3, _statService.GetStatistic(character, tag3) + 15);

            await _charService.SaveCharacterAsync(character);
        }

        private bool IsTagInRange(IList<StatisticValue> skills, int points)
        {
            if (points < TAG_MIN || points > TAG_MAX)
                return false;

            // Unique MUSH rules :/
            if (skills.Where(sk => sk.Value == TAG_MAX).Count() > TAG_MAX_QUANTITY)
                return false;

            if (points == TAG_MAX && skills.Where(sk => sk.Value == TAG_MAX).Count() >= TAG_MAX_QUANTITY)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if character's skills are tagged.
        /// </summary>
        public bool AreSkillsSet(IList<StatisticValue> skillSheet)
        {
            if (skillSheet == null)
                return false;

            if (_expService.UseNewVegasRules)
            {
                if (skillSheet.Count <= 0) return false;
                // This works because all SPECIAL and Skills should be set by now
                return skillSheet.All(x => x.Value > 0);
            }

            // Character has used all tag points
            if (skillSheet.Where(x => x.Statistic is Skill).Sum(x => x.Value) >= TAG_POINTS)
                return true;

            return false;
        }

        /// <summary>
        /// Checks if character's skills are tagged.
        /// </summary>
        public bool AreSkillsSet(Character character) =>
            AreSkillsSet(character?.Skills);

        /// <summary>
        /// Puts one extra point in a specified skill.
        /// </summary>
        public RuntimeResult UpgradeSkill(Character character, Skill skill)
        {
            if (character == null) throw new ArgumentNullException("character");

            var skillVal = _statService.GetStatistic(character, skill);

            if (skillVal + 1 > MAX_SKILL_LEVEL)
                return GenericResult.FromError(Exceptions.CHAR_SKILL_POINTS_GOES_OVER_MAX);

            int price = CalculatePrice(_statService.GetStatistic(character, skill), character.Level);

            if (price > character.ExperiencePoints)
                return GenericResult.FromError(String.Format(Messages.ERR_STAT_NOT_ENOUGH_POINTS, price));

            if (price < 0)
                return GenericResult.FromError(Messages.ERR_STAT_PRICE_NOT_SET);

            _statService.SetStatistic(character, skill, skillVal + 1);
            character.ExperiencePoints -= price;

            return GenericResult.FromSuccess(Messages.SKILLS_SPEND_POINTS_SUCCESS);
        }

        /// <summary>
        /// Checks if all the tags are unique.
        /// </summary>
        private bool AreUniqueTags(Skill tag1, Skill tag2, Skill tag3)
        {
            return !(tag1.Equals(tag2) || tag1.Equals(tag3) || tag2.Equals(tag3));
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
            double multiplier = _expService.GetPriceMultiplier(charLevel);

            if (_skillPrices.TryGetValue(skillLevel, out int basePrice))
            {
                return (int)(_skillPrices[skillLevel] * multiplier);
            }

            return -1;
        }

        public void InitializeSkills(Character character)
        {
            foreach (var skill in character.Skills)
            {
                var specialValue = _statService.GetStatistic(character, ((Skill)skill.Statistic).Special);
                var luck = _statService.GetStatistic(character, Globals.StatisticFlag.Luck);

                skill.Value = 2 + (2 * specialValue) + (luck / 2);
            }
        }
    }
}
