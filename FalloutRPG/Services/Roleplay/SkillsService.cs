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
        private readonly SpecialService _specService;
        private readonly StatisticsService _statService;

        private readonly IConfiguration _config;
        
        public static IReadOnlyCollection<Skill> Skills { get => StatisticsService.Statistics.OfType<Skill>().ToList().AsReadOnly(); }
        private IReadOnlyDictionary<int, int> _skillPrices;

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

            LoadSkilLConfig();
        }

        void LoadSkilLConfig()
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
        public bool AreSkillsTagged(IList<StatisticValue> skillSheet)
        {
            if (skillSheet == null)
                return false;
            if (skillSheet.Where(x => x.Statistic is Skill).Sum(x => x.Value) >= TAG_POINTS)
                return true;

            return false;
        }

        /// <summary>
        /// Checks if character's skills are tagged.
        /// </summary>
        public bool AreSkillsTagged(Character character) =>
            AreSkillsTagged(character?.Skills);

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
                return GenericResult.FromError(String.Format(Messages.ERR_SKILLS_NOT_ENOUGH_POINTS, price));

            _statService.SetStatistic(character, skill, skillVal + 1);
            character.ExperiencePoints -= price;

            return GenericResult.FromSuccess(Messages.SKILLS_SPEND_POINTS_SUCCESS);
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

            if (charLevel >= 10)
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
            if (character.TagPoints > 0) return false;
            if (character.Skills.Sum(x => x.Value) < TAG_POINTS) return false;

            return true;
        }
    }
}
