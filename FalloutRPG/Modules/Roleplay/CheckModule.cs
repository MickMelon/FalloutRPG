using Discord;
using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Services.Roleplay;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("check")]
    public class CheckModule : ModuleBase<SocketCommandContext>
    {
        private readonly CharacterService _charService;
        private readonly SkillsService _skillsService;
        private readonly SpecialService _specialService;

        public CheckModule(CharacterService charService, SkillsService skillsService, SpecialService specialService)
        {
            _charService = charService;
            _skillsService = skillsService;
            _specialService = specialService;
        }

        private string GetCheckMessage(string charName, string attribName, int attribValue, int minimum)
        {
            if (attribValue < minimum)
            {
                return $"[{attribName} {attribValue}/{minimum}] Check **failed** for {charName}! ({Context.User.Mention})";
            }
            else
            {
                return $"[{attribName} {minimum}] Check **passed** for {charName}! ({Context.User.Mention})";
            }
        }

        [Command]
        public async Task CheckSkill(IUser user, Globals.SkillType skill, int minimum)
        {
            var character = await _charService.GetCharacterAsync(user.Id);

            if (character == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, user.Username));
                return;
            }
            if (!_skillsService.AreSkillsSet(character))
            {
                await ReplyAsync(String.Format(Messages.ERR_SKILLS_NOT_FOUND, user.Username));
                return;
            }

            int skillValue = _skillsService.GetSkill(character, skill);

            await ReplyAsync(GetCheckMessage(character.Name, skill.ToString(), skillValue, minimum));
        }

        [Command]
        public async Task CheckSpecial(IUser user, Globals.SpecialType special, int minimum)
        {
            var character = await _charService.GetCharacterAsync(user.Id);

            if (character == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, user.Username));
                return;
            }
            if (!_specialService.IsSpecialSet(character))
            {
                await ReplyAsync(String.Format(Messages.ERR_SPECIAL_NOT_FOUND, user.Username));
                return;
            }

            int specialValue = _specialService.GetSpecial(character, special);

            await ReplyAsync(GetCheckMessage(character.Name, special.ToString(), specialValue, minimum));
        }
    }
}
