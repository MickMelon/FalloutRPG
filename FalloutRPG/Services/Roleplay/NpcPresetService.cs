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

        public NpcPresetService(SkillsService skillsService, IRepository<NpcPreset> presetRepository)
        {
            _presetRepository = presetRepository;
            _skillsService = skillsService;
        }

        public async Task<bool> CreateNpcPreset(string name)
        {
            if (await GetNpcPreset(name) != null)
                return false;

            NpcPreset preset = new NpcPreset { Name = name };
            await _presetRepository.AddAsync(preset);
            return true;
        }

        public async Task<bool> CreateNpcPreset(string name, int str, int per, int end, int cha, int @int, int agi, int luc, bool initializeSkills = false)
        {
            // NPC preset with name exists
            if (await GetNpcPreset(name) != null)
                return false;

            NpcPreset preset = new NpcPreset
            {
                Name = name,
                Strength = str,
                Perception = per,
                Endurance = end,
                Charisma = cha,
                Intelligence = @int,
                Agility = agi,
                Luck = luc
            };

            if (initializeSkills)
                InitializeNpcPresetSkills(preset);

            await _presetRepository.AddAsync(preset);
            return true;
        }

        public async Task<bool> EditNpcPreset(string name, string attribName, object value)
        {
            // Check to make sure the given attribName matches a Skill or S.P.E.C.I.A.L. attribute (or the enabled property), otherwise we might be modifying an important property
            if (Globals.SKILL_NAMES.Contains(attribName, StringComparer.OrdinalIgnoreCase) || Globals.SPECIAL_NAMES.Contains(attribName, StringComparer.OrdinalIgnoreCase) ||
                attribName.Equals("Enabled", StringComparison.OrdinalIgnoreCase) || attribName.Contains("Range"))
            {
                NpcPreset preset = await GetNpcPreset(name);

                if (preset == null)
                    return false;

                typeof(NpcPreset).GetProperty(attribName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).SetValue(preset, value);

                await SaveNpcPreset(preset);
                return true;
            }
            return false;
        }

        public async Task SaveNpcPreset(NpcPreset npcPreset)
        {
            if (npcPreset == null)
                throw new ArgumentNullException("npcPreset");

            await _presetRepository.SaveAsync(npcPreset);
        }

        /// <summary>
        /// Returns an NpcPreset of the given name if it exists in the database, case-insensitively.
        /// </summary>
        /// <param name="typeString">The name of the NPC preset to find.</param>
        /// <returns>An NpcPreset with the given name in the database if it exists.</returns>
        public async Task<NpcPreset> GetNpcPreset(string typeString) => await _presetRepository.Query.Where(x => x.Name.Equals(typeString, StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync();

        public void InitializeNpcPresetSkills(NpcPreset preset)
        {
            if (preset == null)
                throw new ArgumentNullException("preset");

            Character temp = new Character { Special = preset.Special };
            _skillsService.InitializeSkills(temp);

            // Loop through NpcPreset's properties and set them accordingly to the Skills we just made in temp
            foreach (var presetSkill in typeof(NpcPreset).GetProperties())
                if (Globals.SKILL_NAMES.Contains(presetSkill.Name))
                    // set the property value in NpcPreset to the one in temp.Skills
                    presetSkill.SetValue(preset, typeof(SkillSheet).GetProperty(presetSkill.Name).GetValue(temp.Skills));
        }
    }
}
