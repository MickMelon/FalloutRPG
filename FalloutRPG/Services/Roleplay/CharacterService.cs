using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Helpers;
using FalloutRPG.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FalloutRPG.Services.Roleplay
{
    public class CharacterService
    {
        private readonly IRepository<Character> _charRepository;
        private readonly IRepository<SkillSheet> _skillRepository;
        private readonly IRepository<Special> _specialRepository;

        public CharacterService(
            IRepository<Character> charRepository,
            IRepository<SkillSheet> skillRepository,
            IRepository<Special> specialRepository)
        {
            _charRepository = charRepository;
            _skillRepository = skillRepository;
            _specialRepository = specialRepository;
        }

        /// <summary>
        /// Gets a character from the repository by Discord ID.
        /// </summary>
        public async Task<Character> GetCharacterAsync(ulong discordId)
        {
            var character = await _charRepository.Query.Where(x => x.DiscordId == discordId).FirstOrDefaultAsync();
            if (character == null) return null;

            character.Special = await _specialRepository.Query.Where(x => x.CharacterId == character.Id).FirstOrDefaultAsync();
            character.Skills = await _skillRepository.Query.Where(x => x.CharacterId == character.Id).FirstOrDefaultAsync();

            return character;
        }

        /// <summary>
        /// Creates a new character.
        /// </summary>
        public async Task<Character> CreateCharacterAsync(ulong discordId, string name)
        {
            if (await GetCharacterAsync(discordId) != null)
                throw new Exception(Exceptions.CHAR_DISCORDID_EXISTS);

            if (!StringHelper.IsOnlyLetters(name))
                throw new Exception(Exceptions.CHAR_NAMES_NOT_LETTERS);

            if (name.Length > 24 || name.Length < 2)
                throw new Exception(Exceptions.CHAR_NAMES_LENGTH);

            name = StringHelper.ToTitleCase(name);

            var character = new Character()
            {
                DiscordId = discordId,
                Name = name,
                Description = "",
                Story = "",
                Experience = 0,
                SkillPoints = 0,
                Money = 1000,
                Special = new Special()
                {
                    Strength = 0,
                    Perception = 0,
                    Endurance = 0,
                    Charisma = 0,
                    Intelligence = 0,
                    Agility = 0,
                    Luck = 0
                },
                Skills = new SkillSheet()
                {
                    Barter = 0,
                    EnergyWeapons = 0,
                    Explosives = 0,
                    Guns = 0,
                    Lockpick = 0,
                    Medicine = 0,
                    MeleeWeapons = 0,
                    Repair = 0,
                    Science = 0,
                    Sneak = 0,
                    Speech = 0,
                    Survival = 0,
                    Unarmed = 0
                }
            };

            await _charRepository.AddAsync(character);
            return character;
        }

        /// <summary>
        /// Gets the top 10 characters with the most experience.
        /// </summary>
        public async Task<List<Character>> GetHighScoresAsync()
        {
            return await _charRepository.Query.Take(10).OrderByDescending(x => x.Experience).ToListAsync();
        }

        /// <summary>
        /// Deletes a character.
        /// </summary>
        public async Task DeleteCharacterAsync(Character character)
        {
            if (character == null) throw new ArgumentNullException("character");
            await _charRepository.DeleteAsync(character);
        }

        /// <summary>
        /// Saves a character.
        /// </summary>
        public async Task SaveCharacterAsync(Character character)
        {
            if (character == null) throw new ArgumentNullException("character");
            await _charRepository.SaveAsync(character);
        }

        /// <summary>
        /// Removes a character's skills and SPECIAL and marks them
        /// as reset so they can claim skill points back.
        /// </summary>
        public async Task ResetCharacterAsync(Character character)
        {
            await _skillRepository.DeleteAsync(character.Skills);
            await _specialRepository.DeleteAsync(character.Special);
            character.IsReset = true;
            await SaveCharacterAsync(character);
        }
        
        /// <summary>
        /// Get the total number of characters in the database.
        /// </summary>
        public async Task<int> GetTotalCharactersAsync()
        {
            var characters = await _charRepository.FetchAllAsync();
            return characters.Count;
        }
    }
}

