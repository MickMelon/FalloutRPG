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
        private const int MAX_CHARACTERS = 5;

        private readonly IRepository<Character> _charRepository;
        private readonly IRepository<SkillSheet> _skillRepository;
        private readonly IRepository<Special> _specialRepository;

        private readonly PlayerService _playerService;

        public CharacterService(
            IRepository<Character> charRepository,
            IRepository<SkillSheet> skillRepository,
            IRepository<Special> specialRepository,
            PlayerService playerService)
        {
            _charRepository = charRepository;
            _skillRepository = skillRepository;
            _specialRepository = specialRepository;

            _playerService = playerService;
        }

        /// <summary>
        /// Gets the active character from the repository by Discord ID.
        /// </summary>
        public async Task<PlayerCharacter> GetPlayerCharacterAsync(ulong discordId) =>
            await _charRepository.Query.OfType<PlayerCharacter>().Where(c => c.Player.DiscordId == discordId && c.Active == true).Include(c => c.Special).Include(c => c.Skills).FirstOrDefaultAsync();
        public async Task<PlayerCharacter> GetPlayerCharacterAsync(Player player) =>
            await GetPlayerCharacterAsync(player.DiscordId);

        /// <summary>
        /// Gets all characters from the repository by Discord ID.
        /// </summary>
        /// <param name="discordId"></param>
        /// <returns></returns>
        public async Task<List<PlayerCharacter>> GetAllPlayerCharactersAsync(ulong discordId) =>
            await _charRepository.Query.OfType<PlayerCharacter>().Where(c => c.Player.DiscordId == discordId).Include(c => c.Special).Include(c => c.Skills).ToListAsync();
        public async Task<List<PlayerCharacter>> GetAllPlayerCharactersAsync(Player player) =>
            await GetAllPlayerCharactersAsync(player.DiscordId);

        /// <summary>
        /// Creates a new character.
        /// </summary>
        public async Task<PlayerCharacter> CreatePlayerCharacterAsync(Player player, string name)
        {
            if (!StringHelper.IsOnlyLetters(name))
                throw new Exception(Exceptions.CHAR_NAMES_NOT_LETTERS);

            if (name.Length > 24 || name.Length < 2)
                throw new Exception(Exceptions.CHAR_NAMES_LENGTH);

            var characters = await GetAllPlayerCharactersAsync(player.DiscordId);

            if (characters.Count > 0)
            {
                if (HasDuplicateName(characters, name))
                    throw new Exception(Exceptions.CHAR_NAMES_NOT_UNIQUE);

                if (characters.Count >= MAX_CHARACTERS)
                    throw new Exception(Exceptions.CHAR_TOO_MANY);
            }

            name = StringHelper.ToTitleCase(name);
            var character = new PlayerCharacter(player)
            {
                //DiscordId = discordId,
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
                }
            };

            if (characters.Count == 0)
                character.Active = true;

            await _charRepository.AddAsync(character);
            return character;
        }

        public async Task<PlayerCharacter> CreatePlayerCharacterAsync(ulong discordId, string name)
        {
            var player = await _playerService.GetPlayerAsync(discordId);

            return await CreatePlayerCharacterAsync(player, name);
        }

        /// <summary>
        /// Gets the top 10 characters with the most experience.
        /// </summary>
        public async Task<List<Character>> GetHighScoresAsync() =>
            await _charRepository.Query.Take(10).OrderByDescending(x => x.Experience).ToListAsync();

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
        public async Task ResetCharacterAsync(PlayerCharacter character)
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

        public async Task<bool> HasDuplicateName(ulong discordId, string name) =>
            HasDuplicateName(await GetAllPlayerCharactersAsync(discordId), name);

        public bool HasDuplicateName(List<PlayerCharacter> characters, string name)
        {
            if (characters == null) return false;

            foreach (var character in characters)
                if (character.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }
    }
}

