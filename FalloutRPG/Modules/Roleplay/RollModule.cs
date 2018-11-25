using Discord.Commands;
using FalloutRPG.Addons;
using FalloutRPG.Constants;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using System;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("roll")]
    [Alias("r")]
    [Ratelimit(Globals.RATELIMIT_TIMES, Globals.RATELIMIT_SECONDS, Measure.Seconds)]
    public class RollModule : ModuleBase<SocketCommandContext>
    {
        private readonly CharacterService _characterService;
        private readonly RollService _rollService;
        private readonly SkillsService _skillsService;
        private readonly SpecialService _specialService;
        private readonly HelpService _helpService;

        public RollModule(CharacterService characterService,
            RollService rollService,
            SkillsService skillsService,
            SpecialService specialService,
            HelpService helpService)
        {
            _characterService = characterService;
            _rollService = rollService;
            _skillsService = skillsService;
            _specialService = specialService;
            _helpService = helpService;
        }

        [Command]
        [Alias("help")]
        public async Task ShowRollHelpAsync()
        {
            await _helpService.ShowRollHelpAsync(Context);
        }

        [Command]
        public async Task RollSkill(Globals.SkillType skill)
        {
            var character = await _characterService.GetCharacterAsync(Context.User.Id);

            if (character == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, Context.User.Mention));
                return;
            }

            if (!_skillsService.AreSkillsSet(character))
            {
                await ReplyAsync(String.Format(Messages.ERR_SKILLS_NOT_FOUND, Context.User.Mention));
                return;
            }

            await ReplyAsync(_rollService.GetRollMessage(character.Name, skill.ToString(), _rollService.GetRollResult(character, skill)));
        }

        [Command]
        public async Task RollSpecial(Globals.SpecialType special)
        {
            var character = await _characterService.GetCharacterAsync(Context.User.Id);

            if (character == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, Context.User.Mention));
                return;
            }

            if (!_specialService.IsSpecialSet(character))
            {
                await ReplyAsync(String.Format(Messages.ERR_SPECIAL_NOT_FOUND, Context.User.Mention));
                return;
            }

            await ReplyAsync(_rollService.GetRollMessage(character.Name, special.ToString(), _rollService.GetRollResult(character, special)));
        }
    }
}
