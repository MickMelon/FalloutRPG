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
        private const int DEFAULT_SPECIAL_POINTS = 40;
        public const int MAX_SPECIAL = 10;

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
        public async Task SetInitialSpecialAsync(Character character, int[] special)
        {
            if (character == null) throw new ArgumentNullException("character");

            if (!IsSpecialInRange(special))
                throw new ArgumentException(Exceptions.CHAR_SPECIAL_NOT_IN_RANGE);

            if (special.Sum() != DEFAULT_SPECIAL_POINTS)
                throw new ArgumentException(Exceptions.CHAR_SPECIAL_DOESNT_ADD_UP);

            InitializeSpecial(character, special);

            await _charService.SaveCharacterAsync(character);
        }

        /// <summary>
        /// Checks if the special name is valid.
        /// </summary>
        private bool IsValidSpecialName(string special)
        {
            return _statService.Statistics.OfType<Special>().Select(spec => spec.AliasesArray).Any(aliases => aliases.Contains(special));
        }

        /// <summary>
        /// Checks if each number in SPECIAL is between 1 and 10
        /// and ensures there are 7 elements in the input array.
        /// </summary>
        private bool IsSpecialInRange(int[] special)
        {
            if (special.Length != 7) return false;

            foreach (var sp in special)
                if (sp < 1 || sp > 10)
                    return false;

            return true;
        }

        /// <summary>
        /// Initializes character's special.
        /// </summary>
        private void InitializeSpecial(Character character, int[] special)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if a character's special has been set.
        /// </summary>
        public bool IsSpecialSet(Character character)
        {
            if (character == null || character.Statistics == null) return false;
            if (character.Special.Count() != Specials.Count) return false;
            if (character.Special.All(x => x.Value == 0)) return false;
            
            return true;
        }
    }
}
