using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Helpers;
using FalloutRPG.Models;
using FalloutRPG.Models.Effects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FalloutRPG.Services.Roleplay
{
    public class CharacterService
    {
        private const int MAX_CHARACTERS = 5;

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
        /// Gets the active character from the repository by Discord ID.
        /// </summary>
        public async Task<Character> GetCharacterAsync(ulong discordId) =>
            await _charRepository.Query.Where(c => c.DiscordId == discordId && c.Active == true)
            .Include(c => c.Special)
            .Include(c => c.Skills)
            .Include(c => c.EffectCharacters)
                .ThenInclude(x => x.Effect)
                    .ThenInclude(x => x.SkillAdditions)
            .Include(c => c.EffectCharacters)
                .ThenInclude(x => x.Effect)
                    .ThenInclude(x => x.SpecialAdditions)
            .FirstOrDefaultAsync();

        /// <summary>
        /// Gets all characters from the repository by Discord ID.
        /// </summary>
        /// <param name="discordId"></param>
        /// <returns></returns>
        public async Task<List<Character>> GetAllCharactersAsync(ulong discordId) =>
            await _charRepository.Query.Where(c => c.DiscordId == discordId)
            .Include(c => c.Special)
            .Include(c => c.Skills)
            .Include(c => c.EffectCharacters)
                .ThenInclude(x => x.Effect)
                    .ThenInclude(x => x.SkillAdditions)
            .Include(c => c.EffectCharacters)
                .ThenInclude(x => x.Effect)
                    .ThenInclude(x => x.SpecialAdditions)
            .ToListAsync();

        /// <summary>
        /// Creates a new character.
        /// </summary>
        public async Task<Character> CreateCharacterAsync(ulong discordId, string name)
        {
            if (!StringHelper.IsOnlyLetters(name))
                throw new Exception(Exceptions.CHAR_NAMES_NOT_LETTERS);

            if (name.Length > 24 || name.Length < 2)
                throw new Exception(Exceptions.CHAR_NAMES_LENGTH);

            var characters = await GetAllCharactersAsync(discordId);

            if (characters.Count > 0)
            {
                if (HasDuplicateName(characters, name))
                    throw new Exception(Exceptions.CHAR_NAMES_NOT_UNIQUE);

                if (characters.Count >= MAX_CHARACTERS)
                    throw new Exception(Exceptions.CHAR_TOO_MANY);
            }

            name = StringHelper.ToTitleCase(name);
            var character = new Character()
            {
                DiscordId = discordId,
                Active = false,
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
                },
                EffectCharacters = new List<EffectCharacter>()
            };

            if (characters.Count == 0)
                character.Active = true;

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
            return await _charRepository.Query.CountAsync();
        }

        public async Task<bool> HasDuplicateName(ulong discordId, string name) =>
            HasDuplicateName(await GetAllCharactersAsync(discordId), name);

        private bool HasDuplicateName(List<Character> characters, string name)
        {
            if (characters == null) return false;

            foreach (var character in characters)
                if (character.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }
    }
}

