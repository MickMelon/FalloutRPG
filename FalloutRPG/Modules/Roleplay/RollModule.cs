using Discord.Commands;
using FalloutRPG.Addons;
using FalloutRPG.Constants;
using FalloutRPG.Models;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using System;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Ratelimit(Globals.RATELIMIT_TIMES, Globals.RATELIMIT_SECONDS, Measure.Seconds)]
    public class RollModule : ModuleBase<SocketCommandContext>
    {
        private readonly SkillsService _skillsService;
        private readonly SpecialService _specialService;
        private readonly RollService _rollService;
        private readonly HelpService _helpService;

        public RollModule(SkillsService skillsService,
            SpecialService specialService,
            RollService rollService,
            HelpService helpService)
        {
            _skillsService = skillsService;
            _specialService = specialService;
            _rollService = rollService;
            _helpService = helpService;
        }

        [Command("roll")]
        [Alias("r", "help")]
        public async Task ShowRollHelpAsync()
        {
            await _helpService.ShowRollHelpAsync(Context);
        }

        protected async Task RollAsync(Character character, Enum attribute, bool useEffects)
        {
            string result = "";

            if (attribute is Globals.SkillType skill)
            {
                if (!_skillsService.AreSkillsSet(character))
                {
                    await ReplyAsync(String.Format(Messages.ERR_SKILLS_NOT_FOUND, Context.User.Mention));
                    return;
                }

                result = _rollService.RollAttribute(character, skill, useEffects);
            }
            if (attribute is Globals.SpecialType special)
            {
                if (!_specialService.IsSpecialSet(character))
                {
                    await ReplyAsync(String.Format(Messages.ERR_SPECIAL_NOT_FOUND, Context.User.Mention));
                    return;
                }

                result = _rollService.RollAttribute(character, special, useEffects);
            }

            await ReplyAsync($"{result} ({Context.User.Mention})");
        }

        public class RollPlayerModule : RollModule
        {
            private readonly CharacterService _characterService;

            public RollPlayerModule(
                CharacterService characterService,
                SkillsService skillsService, 
                SpecialService specialService, 
                RollService rollService, 
                HelpService helpService) : base(skillsService, specialService, rollService, helpService)
            {
                _characterService = characterService;
            }

            [Command("roll")]
            [Alias("r")]
            public async Task RollSkill(Globals.SkillType skillToRoll) =>
            await RollPlayerAsync(skillToRoll, false);

            [Command("roll")]
            [Alias("r")]
            public async Task RollSpecial(Globals.SpecialType specialToRoll) =>
                await RollPlayerAsync(specialToRoll, false);

            [Command("broll")]
            [Alias("br")]
            public async Task RollSkillBuffed(Globals.SkillType skillToRoll) =>
                await RollPlayerAsync(skillToRoll, true);

            [Command("broll")]
            [Alias("br")]
            public async Task RollSpecialBuffed(Globals.SpecialType specialToRoll) =>
                await RollPlayerAsync(specialToRoll, true);

            private async Task RollPlayerAsync(Enum attribute, bool useEffects)
            {
                var character = await _characterService.GetCharacterAsync(Context.User.Id);

                if (character == null)
                {
                    await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, Context.User.Mention));
                    return;
                }

                await RollAsync(character, attribute, useEffects);
            }
        }

        [Group("npc")]
        public class RollNpcModule : RollModule
        {
            private readonly NpcService _npcService;

            public RollNpcModule(
                NpcService npcService,
                SkillsService skillsService,
                SpecialService specialService,
                RollService rollService,
                HelpService helpService) : base(skillsService, specialService, rollService, helpService)
            {
                _npcService = npcService;
            }

            [Command("roll")]
            [Alias("r")]
            public async Task RollSkill(string name, Globals.SkillType skillToRoll) =>
                await RollNpcAsync(name, skillToRoll, false);

            [Command("roll")]
            [Alias("r")]
            public async Task RollSpecial(string name, Globals.SpecialType specialToRoll) =>
                await RollNpcAsync(name, specialToRoll, false);

            [Command("broll")]
            [Alias("br")]
            public async Task RollSkillBuffed(string name, Globals.SkillType skillToRoll) =>
                await RollNpcAsync(name, skillToRoll, false);

            [Command("broll")]
            [Alias("br")]
            public async Task RollSpecialBuffed(string name, Globals.SpecialType specialToRoll) =>
                await RollNpcAsync(name, specialToRoll, false);

            private async Task RollNpcAsync(string name, Enum attribute, bool useEffects)
            {
                var character = _npcService.FindNpc(name);

                if (character == null)
                {
                    await ReplyAsync(String.Format(Messages.ERR_NPC_CHAR_NOT_FOUND, Context.User.Mention));
                    return;
                }

                await RollAsync(character, attribute, useEffects);
            }
        }
    }
}
