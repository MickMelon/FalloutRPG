﻿using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
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
        private static int CONFIGURED_SPECIAL_POINTS = 29;
        public static int STARTING_SPECIAL_POINTS { get => CONFIGURED_SPECIAL_POINTS - Specials.Count * SPECIAL_MIN; }

        public static int SPECIAL_MAX = 10;
        public static int SPECIAL_MAX_CHARGEN = 8;
        public static int SPECIAL_MAX_CHARGEN_QUANTITY = 2;
        public static int SPECIAL_MIN = 1;

        private readonly CharacterService _charService;
        private readonly StatisticsService _statService;
        private readonly IConfiguration _config;

        public static IReadOnlyCollection<Special> Specials { get => StatisticsService.Statistics.OfType<Special>().ToList().AsReadOnly(); }
        private IReadOnlyDictionary<int, int> _specialPrices;

        public SpecialService(CharacterService charService,
            IConfiguration config,
            StatisticsService statService)
        {
            _charService = charService;
            _statService = statService;
            _config = config;

            LoadSpecialConfig();
        }

        void LoadSpecialConfig()
        {
            try
            {
                var temp = new Dictionary<int, int>();

                foreach (var item in _config.GetSection("roleplay:skill-prices").GetChildren())
                    temp.Add(Int32.Parse(item.Key), Int32.Parse(item.Value));

                _specialPrices = temp;

                SPECIAL_MAX = _config.GetValue<int>("roleplay:special-max");

                CONFIGURED_SPECIAL_POINTS = _config.GetValue<int>("roleplay:chargen:special-points");

                SPECIAL_MAX_CHARGEN = _config.GetValue<int>("roleplay:chargen:special-level-limit");
                SPECIAL_MAX_CHARGEN_QUANTITY = _config.GetValue<int>("roleplay:chargen:specials-at-limit");
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
        public async Task SetInitialSpecialAsync(Character character, Special special, int points)
        {
            if (character == null) throw new ArgumentNullException("character");

            if (!IsSpecialInRange(character.Special, points))
                throw new ArgumentException(Exceptions.CHAR_SPECIAL_NOT_IN_RANGE);

            // Refund special points used if overwriting the same skill
            character.SpecialPoints += _statService.GetStatistic(character, special);

            if (character.SpecialPoints - points < 0)
                throw new Exception(Exceptions.CHAR_NOT_ENOUGH_SKILL_POINTS);

            _statService.SetStatistic(character, special, points);
            character.SpecialPoints -= points;

            await _charService.SaveCharacterAsync(character);
        }

        /// <summary>
        /// Puts one extra point in a specified skill.
        /// </summary>
        public RuntimeResult UpgradeSpecial(Character character, Special special)
        {
            if (character == null) throw new ArgumentNullException("character");

            var specialVal = _statService.GetStatistic(character, special);

            if (specialVal + 1 > SPECIAL_MAX)
                return GenericResult.FromError(Exceptions.CHAR_SKILL_POINTS_GOES_OVER_MAX);

            int price = CalculatePrice(_statService.GetStatistic(character, special), character.Level);

            if (price > character.ExperiencePoints)
                return GenericResult.FromError(String.Format(Messages.ERR_SKILLS_NOT_ENOUGH_POINTS, price));

            _statService.SetStatistic(character, special, specialVal + 1);
            character.ExperiencePoints -= price;

            return GenericResult.FromSuccess(Messages.SKILLS_SPEND_POINTS_SUCCESS);
        }

        private int CalculatePrice(int skillLevel, int charLevel)
        {
            double multiplier = 1.0;

            if (charLevel >= 10)
                for (int i = 0; i < (charLevel - 5) / 5; i++)
                    multiplier += .5;

            return (int)(_specialPrices[skillLevel] * multiplier);
        }

        private bool IsSpecialInRange(IList<StatisticValue> stats, int points)
        {
            var special = stats.Where(x => x.Statistic is Special);

            if (points < SPECIAL_MIN || points > SPECIAL_MAX_CHARGEN)
                return false;

            // Unique MUSH rules :/
            if (special.Where(sp => sp.Value == SPECIAL_MAX_CHARGEN).Count() > SPECIAL_MAX_CHARGEN_QUANTITY)
                return false;

            if (points == SPECIAL_MAX_CHARGEN && special.Where(sp => sp.Value == SPECIAL_MAX_CHARGEN).Count() >= SPECIAL_MAX_CHARGEN_QUANTITY)
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
                if (sp.Value < SPECIAL_MIN || sp.Value > SPECIAL_MAX)
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
