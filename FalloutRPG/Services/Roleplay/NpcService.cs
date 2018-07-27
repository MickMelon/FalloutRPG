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
        private readonly SkillsService _skillsService;
        private readonly SpecialService _specialService;
        private readonly RollService _rollService;
        private readonly NpcPresetService _presetService;

        private readonly List<Character> Npcs;
        
        public NpcService(SkillsService skillsService, SpecialService specialService, RollService rollService, NpcPresetService presetService, IRepository<NpcPreset> presetRepository)
        {
            _skillsService = skillsService;
            _specialService = specialService;
            _rollService = rollService;
            _presetService = presetService;

            Npcs = new List<Character>();
        }

        // plan is to let the players create NPCs with whatever level they desire, and scale Skills according to the level
        public async Task CreateNpc(string npcType, string name)
        {
            if (Npcs.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) != null)
                throw new Exception(Exceptions.NPC_CHAR_EXISTS);

            NpcPreset preset = await _presetService.GetNpcPreset(npcType);

            if (preset == null)
                throw new Exception(Exceptions.NPC_INVALID_TYPE);
            if (preset.Enabled == false)
                throw new Exception(Exceptions.NPC_INVALID_TYPE_DISABLED);

            Character character = new Character { Name = name, Special = preset.Special, Skills = preset.Skills };

            Npcs.Add(character);
        }

        public Character FindNpc(string name) => Npcs.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public string RollNpcSkill(string name, string skill)
        {
            var character = Npcs.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (character == null)
            {
                return String.Format(Messages.ERR_NPC_CHAR_NOT_FOUND, name);
            }
            else if (character.Skills == null)
            {
                throw new Exception(Exceptions.NPC_NULL_SKILLS);
            }

            int skillAmount = (int)typeof(SkillSheet).GetProperty(skill).GetValue(character.Skills);

            if (skillAmount == 0)
                return String.Format(Messages.NPC_CANT_USE_SKILL, character.Name);

            return _rollService.GetRollMessage(character.Name, skill, _rollService.GetRollResult(skill, character)) + " " + Messages.NPC_SUFFIX;
        }

        public string RollNpcSpecial(string name, string special)
        {
            var character = Npcs.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (character == null)
            {
                return String.Format(Messages.ERR_NPC_CHAR_NOT_FOUND, name);
            }
            else if (character.Special == null)
            {
                throw new Exception(Exceptions.NPC_NULL_SPECIAL);
            }

            int specialAmt = (int)typeof(Special).GetProperty(special).GetValue(character.Skills);

            if (specialAmt == 0)
                return String.Format(Messages.NPC_CANT_USE_SPECIAL, character.Name);

            return _rollService.GetRollMessage(character.Name, special, _rollService.GetRollResult(special, character)) + " " + Messages.NPC_SUFFIX;
        }
    }
}
