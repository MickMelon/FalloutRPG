using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using Microsoft.EntityFrameworkCore;
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
        public const int DEFAULT_SPECIAL_POINTS = 29;

        public const int SPECIAL_MAX = 10;
        private const int SPECIAL_MAX_CHARGEN = 8;
        private const int SPECIAL_MAX_CHARGEN_QUANTITY = 2;
        private const int SPECIAL_MIN = 1;

        private readonly CharacterService _charService;
        private readonly StatisticsService _statService;

        public IReadOnlyCollection<Special> Specials { get => (ReadOnlyCollection<Special>)_statService.Statistics.OfType<Special>(); }

        public SpecialService(CharacterService charService, StatisticsService statService)
        {
            _charService = charService;
            _statService = statService;
        }

        /// <summary>
        /// Set character's special.
        /// </summary>
        public async Task SetInitialSpecialAsync(Character character, Special special, int points)
        {
            if (character == null) throw new ArgumentNullException("character");

            if (!IsSpecialInRange(character.Skills, points))
                throw new ArgumentException(Exceptions.CHAR_TAGS_OUT_OF_RANGE);

            // Refund special points used if overwriting the same skill
            character.SpecialPoints += _statService.GetStatistic(character, special);

            if (character.SpecialPoints - points < 0)
                throw new Exception(Exceptions.CHAR_NOT_ENOUGH_SKILL_POINTS);

            _statService.SetStatistic(character, special, points);
            character.SpecialPoints -= points;

            await _charService.SaveCharacterAsync(character);
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
            return _statService.Statistics.OfType<Special>().Select(spec => spec.AliasesArray).Any(aliases => aliases.Contains(special));
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
            if (special.Sum(x => x.Value) != DEFAULT_SPECIAL_POINTS) return false;

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
            if (character.Special.Sum(x => x.Value) < DEFAULT_SPECIAL_POINTS) return false;
            
            return true;
        }
    }
}
