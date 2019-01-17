using Discord;
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
        private readonly CharacterService _charService;
        private readonly EffectsService _effectsService;
        private readonly HelpService _helpService;
        private readonly NpcService _npcService;
        private readonly RollService _rollService;
        private readonly SpecialService _specialService;
        private readonly StatisticsService _statService;

        public RollModule(
            CharacterService characterService,
            EffectsService effectsService,
            HelpService helpService,
            NpcService npcService,
            RollService rollService,
            SpecialService specialService,
            StatisticsService statService)
        {
            _charService = characterService;
            _effectsService = effectsService;
            _helpService = helpService;
            _npcService = npcService;
            _rollService = rollService;
            _specialService = specialService;
            _statService = statService;
        }

        [Command("roll"), Priority(2)]
        [Alias("r", "help")]
        public async Task ShowRollHelpAsync()
        {
            await _helpService.ShowRollHelpAsync(Context);
        }

        #region Old, Single Rolls
        [Command("roll")]
        [Alias("r")]
        public async Task<RuntimeResult> RollSelfStatAsync(Statistic statToRoll) =>
            await RollPlayerAsync(statToRoll, Context.User);

        [Command("roll"), Priority(0)]
        [Alias("r")]
        public async Task<RuntimeResult> RollOthersStatAsync(Statistic statToRoll, IUser user) =>
            await RollPlayerAsync(statToRoll, user);

        [Command("roll"), Priority(1)]
        [Alias("r")]
        public async Task<RuntimeResult> RollNpcStatAsync(Statistic statToRoll, string npcName) =>
            await Task.FromResult(RollNpcAsync(statToRoll, npcName));

        [Command("broll")]
        [Alias("br")]
        public async Task<RuntimeResult> RollSelfStatBuffedAsync(Statistic statToRoll) =>
            await RollPlayerAsync(statToRoll, Context.User, true);

        [Command("broll"), Priority(0)]
        [Alias("br")]
        public async Task<RuntimeResult> RollOthersStatBuffedAsync(Statistic statToRoll, IUser user) =>
            await RollPlayerAsync(statToRoll, user, true);

        [Command("broll"), Priority(1)]
        [Alias("br")]
        public async Task<RuntimeResult> RollNpcStatBuffedAsync(Statistic statToRoll, string npcName) =>
            await Task.FromResult(RollNpcAsync(statToRoll, npcName, true));
        
        #endregion

        #region Roll VS
        [Command("rollvs")]
        [Alias("rv")]
        public async Task<RuntimeResult> RollVsPlayerAsync(IUser user1, Statistic stat1, IUser user2, Statistic stat2) =>
            await RollVsPlayers(user1, user2, stat1, stat2);

        [Command("rollvs")]
        [Alias("rv")]
        public async Task<RuntimeResult> RollVsPlayerAsync(Statistic stat1, IUser user2, Statistic stat2) =>
            await RollVsPlayers(Context.User, user2, stat1, stat2);

        [Command("rollvs")]
        [Alias("rv")]
        public async Task<RuntimeResult> RollVsNpcAsync(string npcName, Statistic stat1, Statistic stat2) =>
            await RollVsPlayerAndNpc(Context.User, npcName, stat1, stat2);

        [Command("rollvs")]
        [Alias("rv")]
        public async Task<RuntimeResult> RollVsNpcAsync(IUser user, Statistic stat1, string npcName, Statistic stat2) =>
            await RollVsPlayerAndNpc(user, npcName, stat1, stat2);

        [Command("rollvs")]
        [Alias("rv")]
        public async Task<RuntimeResult> RollVsNpcAsync(string npcName, IUser user, Statistic stat1, Statistic stat2) =>
            await RollVsPlayerAndNpc(user, npcName, stat1, stat2);

        [Command("rollvs")]
        [Alias("rv")]
        public RuntimeResult RollVsNpcsAsync(string npcName1, Statistic stat1, string npcName2, Statistic stat2) =>
            RollVsTwoNpcsAsync(npcName1, npcName2, stat1, stat2);

        // useEffects
        [Command("brollvs")]
        [Alias("brv")]
        public async Task<RuntimeResult> RollVsPlayerAsyncBuffed(IUser user1, Statistic stat1, IUser user2, Statistic stat2) =>
            await RollVsPlayers(user1, user2, stat1, stat2, true);

        [Command("brollvs")]
        [Alias("brv")]
        public async Task<RuntimeResult> RollVsPlayerAsyncBuffed(Statistic stat1, IUser user2, Statistic stat2) =>
            await RollVsPlayers(Context.User, user2, stat1, stat2, true);

        [Command("brollvs")]
        [Alias("brv")]
        public async Task<RuntimeResult> RollVsNpcAsyncBuffed(string npcName, Statistic stat1, Statistic stat2) =>
            await RollVsPlayerAndNpc(Context.User, npcName, stat1, stat2, true);

        [Command("brollvs")]
        [Alias("brv")]
        public async Task<RuntimeResult> RollVsNpcAsyncBuffed(IUser user, Statistic stat1, string npcName, Statistic stat2) =>
            await RollVsPlayerAndNpc(user, npcName, stat1, stat2, true);

        [Command("brollvs")]
        [Alias("brv")]
        public async Task<RuntimeResult> RollVsNpcAsyncBuffed(string npcName, IUser user, Statistic stat1, Statistic stat2) =>
            await RollVsPlayerAndNpc(user, npcName, stat1, stat2, true);

        [Command("brollvs")]
        [Alias("brv")]
        public RuntimeResult RollVsNpcsAsyncBuffed(string npcName1, Statistic stat1, string npcName2, Statistic stat2) =>
            RollVsTwoNpcsAsync(npcName1, npcName2, stat1, stat2, true);
        #endregion

        private async Task<RuntimeResult> RollPlayerAsync(Statistic statToRoll, IUser user, bool useEffects = false)
        {
            var character = await _charService.GetCharacterAsync(user.Id);
            if (character == null) return CharacterResult.CharacterNotFound(Context.User.Mention);

            if (statToRoll is Skill sk && _statService.GetStatistic(character, statToRoll) < sk.MinimumValue)
                return StatisticResult.SkillNotHighEnough(Context.User.Mention);

            return _rollService.RollStatistic(character, statToRoll, useEffects);
        }

        private RuntimeResult RollNpcAsync(Statistic statToRoll, string npcName, bool useEffects = false)
        {
            var npc = _npcService.FindNpc(npcName);
            if (npc == null) return CharacterResult.NpcNotFound(Context.User.Mention);

            if (statToRoll is Skill sk && _statService.GetStatistic(npc, statToRoll) < sk.MinimumValue)
                return StatisticResult.SkillNotHighEnough(Context.User.Mention);

            _npcService.ResetNpcTimer(npc);

            return _rollService.RollStatistic(npc, statToRoll, useEffects);
        }

        private async Task<RuntimeResult> RollVsPlayers(IUser user1, IUser user2, Statistic stat1, Statistic stat2, bool useEffects = false)
        {
            var char1 = await _charService.GetCharacterAsync(user1.Id);
            var char2 = await _charService.GetCharacterAsync(user2.Id);

            if (char1 == null || char2 == null) return CharacterResult.CharacterNotFound(Context.User.Mention);

            if (stat1 is Skill sk1 && _statService.GetStatistic(char1, stat1) < sk1.MinimumValue)
                return StatisticResult.SkillNotHighEnough(user1.Mention);

            if (stat2 is Skill sk2 && _statService.GetStatistic(char2, stat2) < sk2.MinimumValue)
                return StatisticResult.SkillNotHighEnough(user2.Mention);

            return _rollService.RollVsStatistic(char1, char2, stat1, stat2, useEffects);
        }

        private async Task<RuntimeResult> RollVsPlayerAndNpc(IUser user, string npcName, Statistic stat1, Statistic stat2, bool useEffects = false)
        {
            var char1 = await _charService.GetCharacterAsync(user.Id);
            var npc2 = _npcService.FindNpc(npcName);

            if (char1 == null) return CharacterResult.CharacterNotFound(Context.User.Mention);
            if (npc2 == null) return CharacterResult.NpcNotFound(Context.User.Mention);

            if (stat1 is Skill sk1 && _statService.GetStatistic(char1, stat1) < sk1.MinimumValue)
                return StatisticResult.SkillNotHighEnough(Context.User.Mention);

            if (stat2 is Skill sk2 && _statService.GetStatistic(npc2, stat2) < sk2.MinimumValue)
                return StatisticResult.SkillNotHighEnough(Context.User.Mention);

            return _rollService.RollVsStatistic(char1, npc2, stat1, stat2, useEffects);
        }

        private RuntimeResult RollVsTwoNpcsAsync(string npcName1, string npcName2, Statistic stat1, Statistic stat2, bool useEffects = false)
        {
            var char1 = _npcService.FindNpc(npcName1);
            var char2 = _npcService.FindNpc(npcName2);

            if (char1 == null) return CharacterResult.NpcNotFound(Context.User.Mention);
            if (char2 == null) return CharacterResult.NpcNotFound(Context.User.Mention);

            if (stat1 is Skill sk && _statService.GetStatistic(char1, stat1) < sk.MinimumValue)
                return StatisticResult.SkillNotHighEnough(Context.User.Mention);

            if (stat2 is Skill sk2 && _statService.GetStatistic(char2, stat2) < sk2.MinimumValue)
                return StatisticResult.SkillNotHighEnough(Context.User.Mention);

            return _rollService.RollVsStatistic(char1, char2, stat1, stat2, useEffects);
        }
    }
}