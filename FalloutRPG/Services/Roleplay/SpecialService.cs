using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using FalloutRPG.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FalloutRPG.Services.Roleplay
{
    public class SpecialService
    {
        public const int SPECIAL_MIN = 1;
        private static int CONFIGURED_SPECIAL_POINTS = 29;
        public static int STARTING_SPECIAL_POINTS { get => CONFIGURED_SPECIAL_POINTS - Specials.Count * SPECIAL_MIN; }

        private readonly CharacterService _charService;
        private readonly ChargenOptions _chargenOptions;
        private readonly ExperienceService _expService;
        private readonly ProgressionOptions _progOptions;
        private readonly RoleplayOptions _roleplayOptions;
        private readonly StatisticsService _statService;

        public static IReadOnlyCollection<Special> Specials { get => StatisticsService.Statistics.OfType<Special>().ToList().AsReadOnly(); }
        private IReadOnlyDictionary<int, int> _specialPrices;

        public SpecialService(CharacterService charService,
            ChargenOptions chargenOptions,
            ExperienceService expService,
            ProgressionOptions progOptions,
            StatisticsService statService,
            RoleplayOptions roleplayOptions)
        {
            _chargenOptions = chargenOptions;
            _progOptions = progOptions;
            _roleplayOptions = roleplayOptions;

            _charService = charService;
            _expService = expService;
            _statService = statService;

            LoadSpecialConfig();
        }

        void LoadSpecialConfig()
        {
            try
            {
                var temp = new Dictionary<int, int>();

                foreach (var item in _progOptions.NewProgression.SpecialUpgradePrices)
                    temp.Add(Int32.Parse(item.Key), item.Value);

                _specialPrices = temp;
                CONFIGURED_SPECIAL_POINTS = _chargenOptions.SpecialPoints;
            }
            catch (Exception)
            {
                Console.WriteLine("Special settings improperly configured, check Config.json.");
                throw;
            }
        }

        /// <summary>
        /// Set character's special.
        /// </summary>
        public async Task<RuntimeResult> SetInitialSpecialAsync(Character character, Special special, int points)
        {
            if (character == null) throw new ArgumentNullException("character");

            if (!IsSpecialInRange(character.Special, points))
                return GenericResult.FromError(String.Format(Exceptions.CHAR_SPECIAL_NOT_IN_RANGE, SPECIAL_MIN, _chargenOptions.SpecialLevelMax, _chargenOptions.SpecialsAtLimit));

            // Refund special points used if overwriting the same skill
            character.SpecialPoints += _statService.GetStatistic(character, special);

            if (character.SpecialPoints - points < 0)
                return GenericResult.FromError(Exceptions.CHAR_NOT_ENOUGH_SKILL_POINTS);

            _statService.SetStatistic(character, special, points);
            character.SpecialPoints -= points;

            await _charService.SaveCharacterAsync(character);
            return GenericResult.FromSuccess(String.Format(Messages.SPECIAL_SET_SUCCESS));
        }

        /// <summary>
        /// Puts one extra point in a specified skill.
        /// </summary>
        public RuntimeResult UpgradeSpecial(Character character, Special special)
        {
            if (character == null) throw new ArgumentNullException("character");

            var specialVal = _statService.GetStatistic(character, special);

            if (specialVal + 1 > _roleplayOptions.SpecialMax)
                return GenericResult.FromError(Exceptions.CHAR_SKILL_POINTS_GOES_OVER_MAX);

            int price = CalculatePrice(_statService.GetStatistic(character, special), character.Level);

            if (price > character.ExperiencePoints)
                return GenericResult.FromError(String.Format(Messages.ERR_STAT_NOT_ENOUGH_POINTS, price));

            if (price < 0)
                return GenericResult.FromError(Messages.ERR_STAT_PRICE_NOT_SET);

            _statService.SetStatistic(character, special, specialVal + 1);
            character.ExperiencePoints -= price;

            return GenericResult.FromSuccess(Messages.SKILLS_SPEND_POINTS_SUCCESS);
        }

        private int CalculatePrice(int skillLevel, int charLevel)
        {
            double multiplier = _expService.GetPriceMultiplier(charLevel);

            if (_specialPrices.TryGetValue(skillLevel, out int basePrice))
            {
                return (int)(_specialPrices[skillLevel] * multiplier);
            }

            return -1;
        }

        private bool IsSpecialInRange(IList<StatisticValue> stats, int points)
        {
            var special = stats.Where(x => x.Statistic is Special);

            if (points < SPECIAL_MIN || points > _chargenOptions.SpecialLevelMax)
                return false;

            // Unique MUSH rules :/
            if (special.Where(sp => sp.Value == _chargenOptions.SpecialLevelMax).Count() > _chargenOptions.SpecialsAtLimit)
                return false;

            if (points == _chargenOptions.SpecialLevelMax &&
                special.Where(sp => sp.Value == _chargenOptions.SpecialLevelMax).Count() >= _chargenOptions.SpecialsAtLimit)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if the special name is valid.
        /// </summary>
        private bool IsValidSpecialName(string special)
        {
            return StatisticsService.Statistics.OfType<Special>().Select(spec => spec.AliasesArray).Any(aliases => aliases.Contains(special));
        }

        /// <summary>
        /// Checks if each number in SPECIAL is in valid range
        /// and ensures that the given list's Count matches the
        /// configured amount.
        /// </summary>
        private bool IsSpecialInRange(IList<StatisticValue> stats)
        {
            var special = stats.Where(x => x.Statistic is Special);

            if (special.Count() != Specials.Count) return false;
            if (special.Sum(x => x.Value) != STARTING_SPECIAL_POINTS) return false;

            foreach (var sp in special)
                if (sp.Value < SPECIAL_MIN || sp.Value > _roleplayOptions.SpecialMax)
                    return false;

            return true;
        }

        /// <summary>
        /// Checks if a character's special has been set.
        /// </summary>
        public bool IsSpecialSet(Character character)
        {
            if (character == null || character.Statistics == null) return false;
            if (character.SpecialPoints > 0) return false;
            if (character.Special.Sum(x => x.Value) < STARTING_SPECIAL_POINTS) return false;
            
            return true;
        }
    }
}
