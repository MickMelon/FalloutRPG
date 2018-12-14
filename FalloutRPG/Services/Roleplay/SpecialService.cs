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

        private readonly IRepository<Statistic> _statRepo;

        public ReadOnlyCollection<Special> Specials { get; private set; }

        public SpecialService(CharacterService charService, IRepository<Statistic> statRepo)
        {
            _charService = charService;

            _statRepo = statRepo;

            Specials = new ReadOnlyCollection<Special>(_statRepo.Query.OfType<Special>().ToList());
        }

        public async Task<RuntimeResult> AddSpecialAsync(string name)
        {
            if (_statRepo.Query.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return StatisticResult.StatisticAlreadyExists();

            var newSpecial = new Special
            {
                Name = name,
                Aliases = name + "/"
            };

            await _statRepo.AddAsync(newSpecial);
            await ReloadSpecialsAsync();            

            return GenericResult.FromSuccess(Messages.SPECIAL_ADDED);
        }

        public async Task<RuntimeResult> DeleteSpecialAsync(Skill skill)
        {
            await _statRepo.DeleteAsync(skill);
            await ReloadSpecialsAsync();

            return GenericResult.FromSuccess(Messages.SKILLS_REMOVED);
        }

        public async Task ReloadSpecialsAsync() =>
            Specials = new ReadOnlyCollection<Special>(await _statRepo.Query.OfType<Special>().ToListAsync());

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
            return Specials.Select(spec => spec.AliasesArray).Any(aliases => aliases.Contains(special));
        }

        /// <summary>
        /// Returns the value of the specified special.
        /// </summary>
        /// <returns>Returns -1 if special values are null.</returns>
        public int GetSpecial(IList<StatisticValue> specialSheet, Special special)
        {
            var match = specialSheet.FirstOrDefault(x => x.Statistic.Equals(special));

            if (match == null)
                return -1;

            return match.Value;
        }

        /// <summary>
        /// Returns the value of the specified character's given special.
        /// </summary>
        /// <returns>Returns -1 if character or special values are null.</returns>
        public int GetSpecial(Character character, Special special) =>
            GetSpecial(character?.Statistics, special);

        /// <summary>
        /// Sets the value of the specified character's given special.
        /// </summary>
        /// <returns>Returns false if special is null.</returns>
        public bool SetSpecial(IList<StatisticValue> specialSheet, Special special, int newValue)
        {
            var match = specialSheet.FirstOrDefault(x => x.Statistic.Equals(special));

            if (match == null)
                return false;

            match.Value = newValue;
            return true;
        }

        /// <summary>
        /// Sets the value of the specified character's given special.
        /// </summary>
        /// <returns>Returns false if character or special are null.</returns>
        public bool SetSpecial(Character character, Special special, int newValue) =>
            SetSpecial(character?.Statistics, special, newValue);

        public IList<StatisticValue> CloneSpecial(List<StatisticValue> special)
        {
            var copy = new List<StatisticValue>();

            foreach (var spec in special)
                copy.Add(new StatisticValue { Statistic = spec.Statistic, Value = spec.Value });

            return copy;
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
