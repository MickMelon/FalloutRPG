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
    public class NpcService
    {
        private readonly IRepository<NpcPreset> _presetRepository;

        private readonly SkillsService _skillsService;
        private readonly SpecialService _specialService;
        private readonly RollService _rollService;

        private readonly List<Character> Npcs;
        
        public NpcService(SkillsService skillsService, SpecialService specialService, RollService rollService, IRepository<NpcPreset> presetRepository)
        {
            _skillsService = skillsService;
            _specialService = specialService;
            _rollService = rollService;

            _presetRepository = presetRepository;

            Npcs = new List<Character>();
        }

        public async Task CreateNpc(string npcType, string firstName)
        {
            if (Npcs.Find(x => x.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase)) != null)
                throw new Exception(Exceptions.NPC_CHAR_EXISTS);

            NpcPreset preset = await GetNpcPreset(npcType);

            if (preset == null)
                throw new Exception(Exceptions.NPC_INVALID_TYPE);
            if (preset.Enabled == false)
                throw new Exception(Exceptions.NPC_INVALID_TYPE_DISABLED);

            Character character = new Character { FirstName = firstName, Special = preset.Special, Skills = preset.Skills };

            Npcs.Add(character);
        }

        public Character FindNpc(string name) => Npcs.Find(x => x.FirstName.Equals(name, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Returns an NpcPreset of the given name if it exists, case-insensitively.
        /// </summary>
        /// <param name="typeString">The name of the NPC preset to find.</param>
        /// <returns>An NpcPreset with the given name in the database if it exists.</returns>
        public async Task<NpcPreset> GetNpcPreset(string typeString) => await _presetRepository.Query.Where(x => x.Name.Equals(typeString, StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync();

        public async Task SaveNpcPreset(NpcPreset npcPreset)
        {
            if (npcPreset == null)
                throw new ArgumentNullException("npcPreset");

            await _presetRepository.SaveAsync(npcPreset);
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

        public void InitializeNpcPresetSkills(NpcPreset preset)
        {
            if (preset == null)
                throw new ArgumentNullException("preset");

            Special presetSpecial = preset.Special;

            Character temp = new Character { Special = presetSpecial };
            _skillsService.InitializeSkills(temp);

            // Loop through NpcPreset's properties and set them accordingly to the Skills we just made in temp
            foreach (var prop in typeof(NpcPreset).GetProperties())
            {
                string potentialSkillName = prop.Name;

                // passing this check, name is definitely a skill
                if (Globals.SKILL_NAMES.Contains(potentialSkillName)) 
                    // set the property value in NpcPreset to the one in temp.Skills
                    prop.SetValue(preset, typeof(SkillSheet).GetProperty(potentialSkillName).GetValue(temp.Skills)); 
            }
        }

        public async Task<bool> EditNpcPresetEnable(string name, bool enabled)
        {
            NpcPreset preset = await GetNpcPreset(name);

            if (preset == null)
                return false;

            preset.Enabled = enabled;

            await SaveNpcPreset(preset);
            return true;
        }

        public async Task<bool> EditNpcPreset(string name, string attribName, int value)
        {
            // Check to make sure the given attribName matches a Skill or S.P.E.C.I.A.L. attribute, otherwise we might be modifying an important property
            if (Globals.SKILL_NAMES.Contains(attribName, StringComparer.OrdinalIgnoreCase) || Globals.SPECIAL_NAMES.Contains(attribName, StringComparer.OrdinalIgnoreCase))
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

        public string RollNpcSkill(string firstName, string skill)
        {
            var character = Npcs.Find(x => x.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase));

            if (character == null)
            {
                return String.Format(Messages.ERR_NPC_CHAR_NOT_FOUND, firstName);
            }
            else if (character.Skills == null)
            {
                throw new Exception(Exceptions.NPC_NULL_SKILLS);
            }

            int skillAmount = (int)typeof(SkillSheet).GetProperty(skill).GetValue(character.Skills);

            if (skillAmount == 0)
                return String.Format(Messages.NPC_CANT_USE_SKILL, character.FirstName);

            return _rollService.GetSkillRollResult(skill, character) + " " + Messages.NPC_SUFFIX;
        }

        public string RollNpcSpecial(string firstName, string special)
        {
            var character = Npcs.Find(x => x.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase));

            if (character == null)
            {
                return String.Format(Messages.ERR_NPC_CHAR_NOT_FOUND, firstName);
            }
            else if (character.Special == null)
            {
                throw new Exception(Exceptions.NPC_NULL_SPECIAL);
            }

            int specialAmt = (int)typeof(Special).GetProperty(special).GetValue(character.Skills);

            if (specialAmt == 0)
                return String.Format(Messages.NPC_CANT_USE_SPECIAL, character.FirstName);

            return _rollService.GetSpecialRollResult(special, character) + " " + Messages.NPC_SUFFIX;
        }
    }
}
