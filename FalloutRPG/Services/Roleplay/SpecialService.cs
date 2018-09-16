using FalloutRPG.Constants;
using FalloutRPG.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FalloutRPG.Services.Roleplay
{
    public class SpecialService
    {
        private const int DEFAULT_SPECIAL_POINTS = 40;
        private const int MAX_SPECIAL = 10;

        private readonly CharacterService _charService;

        public SpecialService(CharacterService charService)
        {
            _charService = charService;
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
            foreach (var name in Globals.SPECIAL_NAMES)
                if (special.Equals(name, StringComparison.InvariantCultureIgnoreCase) ||
                    special.Equals(name.Substring(0, 3), StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }

        /// <summary>
        /// Returns the value of the specified character's given special.
        /// </summary>
        /// <returns>Returns 0 if character or special values are null.</returns>
        public int GetSpecial(Character character, Globals.SpecialType special)
        {
            if (character == null || !IsSpecialSet(character))
                return 0;

            return (int)typeof(Special).GetProperty(special.ToString()).GetValue(character.Special);
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
            character.Special = new Special()
            {
                Strength = special[0],
                Perception = special[1],
                Endurance = special[2],
                Charisma = special[3],
                Intelligence = special[4],
                Agility = special[5],
                Luck = special[6]
             };
        }

        /// <summary>
        /// Checks if a character's special has been set.
        /// </summary>
        public bool IsSpecialSet(Character character)
        {
            if (character == null || character.Special == null) return false;

            var properties = character.Special.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (prop.Name.Equals("CharacterId") || prop.Name.Equals("Id"))
                    continue;

                var value = Convert.ToInt32(prop.GetValue(character.Special));
                if (value == 0) return false;
            }

            return true;
        }
    }
}
