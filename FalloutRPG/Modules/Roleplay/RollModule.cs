using Discord.Commands;
using FalloutRPG.Addons;
using FalloutRPG.Constants;
using FalloutRPG.Models;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using System;
using System.Linq;
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
    }

    public class RollPlayerModule : ModuleBase<SocketCommandContext>
    {
        private readonly CharacterService _characterService;
        private readonly EffectsService _effectsService;
        private readonly RollService _rollService;
        private readonly SpecialService _specialService;

        public RollPlayerModule(
            CharacterService characterService,
            EffectsService effectsService,
            RollService rollService,
            SpecialService specialService)
        {
            _characterService = characterService;
            _effectsService = effectsService;
            _rollService = rollService;
            _specialService = specialService;
        }

        [Command("roll")]
        [Alias("r")]
        public async Task<RuntimeResult> RollStatAsync(Statistic statToRoll) =>
            await RollPlayerAsync(statToRoll, false);

        [Command("broll")]
        [Alias("br")]
        public async Task<RuntimeResult> RollStatBuffedAsync(Statistic statToRoll) =>
            await RollPlayerAsync(statToRoll, true);

        private async Task<RuntimeResult> RollPlayerAsync(Statistic stat, bool useEffects)
        {
            var character = await _characterService.GetCharacterAsync(Context.User.Id);

            if (character == null) return CharacterResult.CharacterNotFound(Context.User.Mention);

            if (!_specialService.IsSpecialSet(character)) return StatisticResult.SpecialNotSet(Context.User.Mention);

            var stats = character.Statistics;
            if (useEffects) stats = _effectsService.GetEffectiveStatistics(character);

            string result = _rollService.RollStatistic(character, stat);
            return GenericResult.FromSuccess($"{result} ({Context.User.Mention})");
        }
    }

    [Group("npc")]
    public class RollNpcModule : ModuleBase<SocketCommandContext>
    {
        private readonly EffectsService _effectsService;
        private readonly NpcService _npcService;
        private readonly RollService _rollService;

        public RollNpcModule(
            EffectsService effectsService,
            NpcService npcService,
            RollService rollService)
        {
            _effectsService = effectsService;
            _npcService = npcService;
            _rollService = rollService;
        }

        [Command("roll")]
        [Alias("r")]
        public RuntimeResult RollSkill(string name, Statistic statToRoll) =>
            RollNpcAsync(name, statToRoll, false);

        [Command("broll")]
        [Alias("br")]
        public RuntimeResult RollSkillBuffed(string name, Statistic statToRoll) =>
            RollNpcAsync(name, statToRoll, true);

        private RuntimeResult RollNpcAsync(string name, Statistic stat, bool useEffects)
        {
            var character = _npcService.FindNpc(name);

            if (character == null) return CharacterResult.NpcNotFound(Context.User.Mention);

            var stats = character?.Statistics;
            if (useEffects) stats = _effectsService.GetEffectiveStatistics(character);

            var statValue = stats.FirstOrDefault(x => x.Statistic.Equals(stat));

            if (statValue == null)
                return GenericResult.FromError(String.Format(Messages.NPC_CANT_USE_STAT, character.Name));

            _npcService.ResetNpcTimer(character);

            string result = _rollService.RollStatistic(character, stat);
            return GenericResult.FromSuccess($"{result} ({Context.User.Mention}) {Messages.NPC_SUFFIX}");
        }
    }
}
