﻿using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Services.Roleplay
{
    public class NpcPresetService
    {
        private readonly IRepository<NpcPreset> _presetRepository;

        private readonly SkillsService _skillsService;

        public NpcPresetService(SkillsService skillsService, IRepository<NpcPreset> presetRepository)
        {
            _presetRepository = presetRepository;
            _skillsService = skillsService;
        }

        public async Task<bool> CreateNpcPresetAsync(string name)
        {
            return await CreateNpcPresetAsync(name, 0, 0, 0, 0, 0, 0, 0);
        }

        public async Task<bool> CreateNpcPresetAsync(string name, int str, int per, int end, int cha, int @int, int agi, int luc)
        {
            // NPC preset with name exists
            if (await GetNpcPreset(name) != null)
                return false;

            NpcPreset preset = new NpcPreset
            {
                Name = name,
                Special = new Models.Statistic { Strength = str, Perception = per, Endurance = end, Charisma = cha, Intelligence = @int, Agility = agi, Luck = luc }
            };

            await _presetRepository.AddAsync(preset);
            return true;
        }

        /// <summary>
        /// Returns an NpcPreset of the given name if it exists in the database, case-insensitively.
        /// </summary>
        /// <param name="typeString">The name of the NPC preset to find.</param>
        /// <returns>An NpcPreset with the given name in the database if it exists.</returns>
        public async Task<NpcPreset> GetNpcPreset(string typeString) =>
            await _presetRepository.Query.Where(x => x.Name.Equals(typeString, StringComparison.OrdinalIgnoreCase))
            .Include(x => x.Special)
            .FirstOrDefaultAsync();

        public async Task SaveNpcPreset(NpcPreset npcPreset)
        {
            if (npcPreset == null)
                throw new ArgumentNullException("npcPreset");

            await _presetRepository.SaveAsync(npcPreset);
        }
    }
}
