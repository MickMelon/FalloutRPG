using FalloutRPG.Constants;
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
        private readonly StatisticsService _statsService;

        public NpcPresetService(SkillsService skillsService, StatisticsService statsService, IRepository<NpcPreset> presetRepository)
        {
            _presetRepository = presetRepository;
            _skillsService = skillsService;
            _statsService = statsService;
        }

        public async Task<bool> CreateNpcPresetAsync(string name)
        {
            // NPC preset with name exists
            if (await GetNpcPreset(name) != null)
                throw new Exception(Exceptions.NPC_PRESET_EXISTS);

            NpcPreset preset = new NpcPreset
            {
                Name = name,
                Statistics = new List<StatisticValue>()
            };

            _statsService.InitializeStatistics(preset.Statistics);

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
            .Include(x => x.Statistics)
            .FirstOrDefaultAsync();

        public async Task SaveNpcPreset(NpcPreset npcPreset)
        {
            if (npcPreset == null)
                throw new ArgumentNullException("npcPreset");

            await _presetRepository.SaveAsync(npcPreset);
        }
    }
}
