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
        private readonly RollService _rollService;
        private readonly HelpService _helpService;

        public RollModule(
            RollService rollService,
            HelpService helpService)
        {
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
                result = _rollService.RollAttribute(character, skill, useEffects);
            }
            if (attribute is Globals.SpecialType special)
            {
                result = _rollService.RollAttribute(character, special, useEffects);
            }

            if (useEffects)
                await ReplyAsync($"{Messages.MUSCLE_EMOJI}{result} ({Context.User.Mention})");
            else
                await ReplyAsync($"{result} ({Context.User.Mention})");
        }

        public class RollPlayerModule : RollModule
        {
            private readonly CharacterService _characterService;
            private readonly SkillsService _skillsService;
            private readonly SpecialService _specialService;

            public RollPlayerModule(
                CharacterService characterService,
                SkillsService skillsService, 
                SpecialService specialService, 
                RollService rollService, 
                HelpService helpService) : base(rollService, helpService)
            {
                _characterService = characterService;
                _skillsService = skillsService;
                _specialService = specialService;
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

                if (!_specialService.IsSpecialSet(character))
                {
                    await ReplyAsync(String.Format(Messages.ERR_SPECIAL_NOT_FOUND, Context.User.Mention));
                    return;
                }

                if (!_skillsService.AreSkillsSet(character))
                {
                    await ReplyAsync(String.Format(Messages.ERR_SKILLS_NOT_FOUND, Context.User.Mention));
                    return;
                }

                await RollAsync(character, attribute, useEffects);
            }
        }

        [Group("npc")]
        public class RollNpcModule : RollModule
        {
            private readonly SkillsService _skillsService;
            private readonly SpecialService _specialService;
            private readonly NpcService _npcService;

            public RollNpcModule(
                NpcService npcService,
                SkillsService skillsService,
                SpecialService specialService,
                RollService rollService,
                HelpService helpService) : base(rollService, helpService)
            {
                _skillsService = skillsService;
                _specialService = specialService;
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
                await RollNpcAsync(name, skillToRoll, true);

            [Command("broll")]
            [Alias("br")]
            public async Task RollSpecialBuffed(string name, Globals.SpecialType specialToRoll) =>
                await RollNpcAsync(name, specialToRoll, true);

            private async Task RollNpcAsync(string name, Enum attribute, bool useEffects)
            {
                var character = _npcService.FindNpc(name);

                if (character == null)
                {
                    await ReplyAsync(String.Format(Messages.ERR_NPC_CHAR_NOT_FOUND, Context.User.Mention));
                    return;
                }

                if (attribute is Globals.SkillType skill)
                {
                    if (_skillsService.GetSkill(character, skill) <= 0)
                    {
                        await ReplyAsync(String.Format(Messages.NPC_CANT_USE_SKILL, character.Name));
                        return;
                    }
                }

                if (attribute is Globals.SpecialType special)
                {
                    if (_specialService.GetSpecial(character, special) <= 0)
                    {
                        await ReplyAsync(String.Format(Messages.NPC_CANT_USE_SPECIAL, character.Name));
                        return;
                    }
                }

                _npcService.ResetNpcTimer(character);
                await RollAsync(character, attribute, useEffects);
            }
        }
    }
}
