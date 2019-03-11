using Discord;
using Discord.Commands;
using FalloutRPG.Addons;
using FalloutRPG.Constants;
using FalloutRPG.Models;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly Random _random;
        private readonly RollService _rollService;
        private readonly SpecialService _specialService;

        public RollModule(
            CharacterService characterService,
            EffectsService effectsService,
            HelpService helpService,
            NpcService npcService,
            RollService rollService,
            SpecialService specialService,
            Random random)
        {
            _charService = characterService;
            _effectsService = effectsService;
            _helpService = helpService;
            _npcService = npcService;
            _rollService = rollService;
            _specialService = specialService;

            _random = random;
        }

        [Command("roll")]
        [Alias("r", "help")]
        public async Task ShowRollHelpAsync()
        {
            await _helpService.ShowRollHelpAsync(Context);
        }

        [Command("rolld")]
        [Alias("rolldice", "rd")]
        public async Task<RuntimeResult> RollDiceAsync(int dieCount, int sides, int bonus = 0)
        {
            if (dieCount > 20)
                return GenericResult.FromError(Messages.ROLL_DICE_TOO_MANY);

            if (sides > 100)
                return GenericResult.FromError(Messages.ROLL_DICE_TOO_MANY_SIDES);

            int[] dice = new int[dieCount];

            int sum = 0;

            for (int die = 0; die < dieCount; die++)
            {
                int dieResult = _random.Next(1, sides + 1);
                dice[die] = dieResult;
                sum += dieResult;
            }

            StringBuilder sb = new StringBuilder();
            for (int die = 0; die < dice.Length; die++)
                sb.Append($"[{dice[die]}] + ");

            sb.Append($"{bonus} = {dice.Sum() + bonus}");

            return await Task.FromResult(GenericResult.FromSuccess(String.Format(Messages.ROLL_DICE, sb.ToString())));
        }

        [Command("rollm")]
        [Alias("rm")]
        public async Task<RuntimeResult> RollManualAsync(Statistic stat, int value)
        {
            if (value > 5000)
                return GenericResult.FromError("Too high.");

            var mockCharacter = new Character { Name = Context.User.Username };
            mockCharacter.Statistics = new List<StatisticValue> { new StatisticValue { Statistic = stat, Value = value } };
            return await Task.FromResult(_rollService.RollStatistic(mockCharacter, stat));
        }

        #region Old, Single Rolls
        [Command("roll")]
        [Alias("r")]
        public async Task<RuntimeResult> RollSelfStatAsync(Statistic statToRoll) =>
                await RollPlayerAsync(statToRoll, Context.User);

        [Command("roll")]
        [Alias("r")]
        public async Task<RuntimeResult> RollOthersStatAsync(Statistic statToRoll, IUser user) =>
            await RollPlayerAsync(statToRoll, user);

        [Command("rollnpc")]
        [Alias("rolln", "rn")]
        public async Task<RuntimeResult> RollNpcStatAsync(Statistic statToRoll, string npcName) =>
            await Task.FromResult(RollNpcAsync(statToRoll, npcName));

        [Command("broll")]
        [Alias("br")]
        public async Task<RuntimeResult> RollSelfStatBuffedAsync(Statistic statToRoll) =>
            await RollPlayerAsync(statToRoll, Context.User, true);

        [Command("broll")]
        [Alias("br")]
        public async Task<RuntimeResult> RollOthersStatBuffedAsync(Statistic statToRoll, IUser user) =>
            await RollPlayerAsync(statToRoll, user, true);

        [Command("brollnpc")]
        [Alias("brolln", "brn")]
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

        [Command("rollvsnpc")]
        [Alias("rollvsn", "rvn")]
        public async Task<RuntimeResult> RollVsNpcAsync(Statistic stat1, string npcName, Statistic stat2) =>
            await RollVsPlayerAndNpc(Context.User, npcName, stat1, stat2);

        [Command("rollvs")]
        [Alias("rv")]
        public async Task<RuntimeResult> RollVsNpcAsync(IUser user, Statistic stat1, string npcName, Statistic stat2) =>
            await RollVsPlayerAndNpc(user, npcName, stat1, stat2);

        [Command("rollvsnpc")]
        [Alias("rollvsn", "rvn")]
        public async Task<RuntimeResult> RollVsNpcAsync(string npcName, Statistic stat1, IUser user, Statistic stat2) =>
            await RollVsPlayerAndNpc(user, npcName, stat1, stat2);

        [Command("rollvsnpc")]
        [Alias("rollvsn", "rvn")]
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

        [Command("brollvsnpc")]
        [Alias("brollvsn", "brvn")]
        public async Task<RuntimeResult> RollVsNpcAsyncBuffed(Statistic stat1, string npcName, Statistic stat2) =>
            await RollVsPlayerAndNpc(Context.User, npcName, stat1, stat2, true);

        [Command("brollvsnpc")]
        [Alias("brollvsn", "brvn")]
        public async Task<RuntimeResult> RollVsNpcAsyncBuffed(IUser user, Statistic stat1, string npcName, Statistic stat2) =>
            await RollVsPlayerAndNpc(user, npcName, stat1, stat2, true);

        [Command("brollvsnpc")]
        [Alias("brollvsn", "brvn")]
        public async Task<RuntimeResult> RollVsNpcAsyncBuffed(string npcName, Statistic stat1, IUser user, Statistic stat2) =>
            await RollVsPlayerAndNpc(user, npcName, stat1, stat2, true);

        [Command("brollvsnpc")]
        [Alias("brollvsn", "brvn")]
        public RuntimeResult RollVsNpcsAsyncBuffed(string npcName1, Statistic stat1, string npcName2, Statistic stat2) =>
            RollVsTwoNpcsAsync(npcName1, npcName2, stat1, stat2, true);
        #endregion

        private async Task<RuntimeResult> RollPlayerAsync(Statistic statToRoll, IUser user, bool useEffects = false)
        {
            var character = await _charService.GetCharacterAsync(user.Id);
            if (character == null) return CharacterResult.CharacterNotFound();

            return _rollService.RollStatistic(character, statToRoll, useEffects);
        }

        private RuntimeResult RollNpcAsync(Statistic statToRoll, string npcName, bool useEffects = false)
        {
            var npc = _npcService.FindNpc(npcName);
            if (npc == null) return CharacterResult.NpcNotFound();

            _npcService.ResetNpcTimer(npc);

            return _rollService.RollStatistic(npc, statToRoll, useEffects);
        }

        private async Task<RuntimeResult> RollVsPlayers(IUser user1, IUser user2, Statistic stat1, Statistic stat2, bool useEffects = false)
        {
            var char1 = await _charService.GetCharacterAsync(user1.Id);
            var char2 = await _charService.GetCharacterAsync(user2.Id);

            if (char1 == null || char2 == null) return CharacterResult.CharacterNotFound();

            return _rollService.RollVsStatistic(char1, char2, stat1, stat2, useEffects);
        }

        private async Task<RuntimeResult> RollVsPlayerAndNpc(IUser user, string npcName, Statistic stat1, Statistic stat2, bool useEffects = false)
        {
            var char1 = await _charService.GetCharacterAsync(user.Id);
            var char2 = _npcService.FindNpc(npcName);

            if (char1 == null) return CharacterResult.CharacterNotFound();
            if (char2 == null) return CharacterResult.NpcNotFound();

            return _rollService.RollVsStatistic(char1, char2, stat1, stat2, useEffects);
        }

        private RuntimeResult RollVsTwoNpcsAsync(string npcName1, string npcName2, Statistic stat1, Statistic stat2, bool useEffects = false)
        {
            var char1 = _npcService.FindNpc(npcName1);
            var char2 = _npcService.FindNpc(npcName2);

            if (char1 == null) return CharacterResult.NpcNotFound();
            if (char2 == null) return CharacterResult.NpcNotFound();

            return _rollService.RollVsStatistic(char1, char2, stat1, stat2, useEffects);
        }
    }
}