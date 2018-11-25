using Discord.Commands;
using FalloutRPG.Addons;
using FalloutRPG.Constants;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using System;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Ratelimit(Globals.RATELIMIT_TIMES, Globals.RATELIMIT_SECONDS, Measure.Seconds)]
    public class RollModule : ModuleBase<SocketCommandContext>
    {
        private readonly CharacterService _characterService;
        private readonly EffectsService _effectsService;
        private readonly RollService _rollService;
        private readonly SkillsService _skillsService;
        private readonly SpecialService _specialService;
        private readonly HelpService _helpService;

        public RollModule(CharacterService characterService,
            EffectsService effectsService,
            RollService rollService,
            SkillsService skillsService,
            SpecialService specialService,
            HelpService helpService)
        {
            _characterService = characterService;
            _effectsService = effectsService;
            _rollService = rollService;
            _skillsService = skillsService;
            _specialService = specialService;
            _helpService = helpService;
        }

        [Command("roll")]
        [Alias("r", "help")]
        public async Task ShowRollHelpAsync()
        {
            await _helpService.ShowRollHelpAsync(Context);
        }

        [Command("roll")]
        [Alias("r")]
        public async Task RollSkill(Globals.SkillType skillToRoll) =>
            await RollSkill(skillToRoll, false);

        [Command("broll")]
        [Alias("br")]
        public async Task RollSkillBuffed(Globals.SkillType skillToRoll) =>
            await RollSkill(skillToRoll, true);

        private async Task RollSkill(Globals.SkillType skillToRoll, bool useEffects)
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

            if (useEffects)
            {
                var effectiveValue = _skillsService.GetSkill(_effectsService.GetEffectiveSkills(character), skillToRoll);
                var effectiveLuck = _effectsService.GetEffectiveSpecial(character).Luck;

                await ReplyAsync($"{Messages.MUSCLE_EMOJI}{_rollService.GetRollMessage(character.Name, skillToRoll.ToString(), _rollService.GetRollResult(effectiveValue, effectiveLuck, false))} ({Context.User.Mention})");
            }
            else
            {
                var skillValue = _skillsService.GetSkill(character, skillToRoll);

                await ReplyAsync($"{_rollService.GetRollMessage(character.Name, skillToRoll.ToString(), _rollService.GetRollResult(skillValue, character.Special.Luck, false))} ({Context.User.Mention})");
            }
        }

        [Command("roll")]
        [Alias("r")]
        public async Task RollSpecial(Globals.SpecialType specialToRoll) =>
            await RollSpecial(specialToRoll, false);

        [Command("broll")]
        [Alias("br")]
        public async Task RollSpecialBuffed(Globals.SpecialType specialToRoll) =>
            await RollSpecial(specialToRoll, true);

        private async Task RollSpecial(Globals.SpecialType specialToRoll, bool useEffects)
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

            if (useEffects)
            {
                var newSpecial = _effectsService.GetEffectiveSpecial(character);

                var effectiveValue = _specialService.GetSpecial(newSpecial, specialToRoll);
                var effectiveLuck = newSpecial.Luck;

                await ReplyAsync($"{Messages.MUSCLE_EMOJI}{_rollService.GetRollMessage(character.Name, specialToRoll.ToString(), _rollService.GetRollResult(effectiveValue, effectiveLuck, true))} {Context.User.Mention}");
            }
            else
            {
                var specialValue = _specialService.GetSpecial(character, specialToRoll);

                await ReplyAsync($"{_rollService.GetRollMessage(character.Name, specialToRoll.ToString(), _rollService.GetRollResult(specialValue, character.Special.Luck, true))} {Context.User.Mention}");
            }
        }
    }
}
