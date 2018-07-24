using FalloutRPG.Constants;
using FalloutRPG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalloutRPG.Services.Roleplay
{
    public class NpcService
    {
        private readonly SkillsService _skillsService;
        private readonly RollService _rollService;

        private readonly List<Character> Npcs;
        
        public NpcService(SkillsService skillsService, RollService rollService)
        {
            _skillsService = skillsService;
            _rollService = rollService;

            Npcs = new List<Character>();
        }

        public void CreateNpc(string npcType, string firstName)
        {
            if (Npcs.Find(x => x.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase)) != null)
                throw new Exception(Exceptions.NPC_CHAR_EXISTS);

            if(!IsValidNpcType(npcType))
                throw new Exception(Exceptions.NPC_INVALID_TYPE);

            Character character = new Character
            {
                FirstName = firstName,
                Special = GetPresetSpecial(npcType),
            };

            character.Skills = GetPresetSkillSheet(npcType);

            Npcs.Add(character);
        }

        public Character FindNpc(string name) => Npcs.Find(x => x.FirstName.Equals(name, StringComparison.OrdinalIgnoreCase));

        public Special GetPresetSpecial(string presetName) => throw new NotImplementedException();
        public SkillSheet GetPresetSkillSheet(string presetName) => throw new NotImplementedException();

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
                return $"{Messages.FAILURE_EMOJI} {firstName} can't use this skill!" + " (\uD83D\uDCBBNPC)";

            return _rollService.GetSkillRollResult(skill, character) + " (\uD83D\uDCBBNPC)";
        }

        public string RollNpcSpecial(string firstName, string special)
        {
            var character = Npcs.Find(x => x.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase));

            if (character == null)
            {
                return Messages.FAILURE_EMOJI + "NPC was not found with given name.";
            }
            else if (character.Skills == null)
            {
                throw new Exception(Exceptions.NPC_NULL_SPECIAL);
            }

            return _rollService.GetSpecialRollResult(special, character) + " (\uD83D\uDCBBNPC)";
        }

        public bool IsValidNpcType(string typeString)
        {
            throw new NotImplementedException();
        }
    }
}
