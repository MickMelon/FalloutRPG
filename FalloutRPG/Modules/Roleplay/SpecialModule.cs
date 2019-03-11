using Discord;
using Discord.Commands;
using FalloutRPG.Addons;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using FalloutRPG.Models;
using FalloutRPG.Services.Roleplay;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("character")]
    [Alias("char")]
    [Ratelimit(Globals.RATELIMIT_TIMES, Globals.RATELIMIT_SECONDS, Measure.Seconds)]
    public class SpecialModule : ModuleBase<SocketCommandContext>
    {
        [Group("special")]
        [Alias("spec", "sp")]
        public class CharacterSpecialModule : ModuleBase<SocketCommandContext>
        {
            private readonly CharacterService _charService;
            private readonly EffectsService _effectsService;
            private readonly SpecialService _specService;

            public CharacterSpecialModule(CharacterService charService, EffectsService effectsService, SpecialService specService)
            {
                _charService = charService;
                _effectsService = effectsService;
                _specService = specService;
            }

            [Command]
            [Alias("show", "view")]
            public async Task<RuntimeResult> ShowSpecialAsync(IUser targetUser = null) =>
                await ShowSpecialAsync(targetUser, false);

            [Command("buffed")]
            [Alias("bshow", "bview")]
            public async Task<RuntimeResult> ShowSpecialBuffedAsync(IUser targetUser = null) =>
                await ShowSpecialAsync(targetUser, true);

            private async Task<RuntimeResult> ShowSpecialAsync(IUser targetUser = null, bool useEffects = false)
            {
                var userInfo = Context.User;
                var character = targetUser == null
                    ? await _charService.GetCharacterAsync(userInfo.Id)
                    : await _charService.GetCharacterAsync(targetUser.Id);

                if (character == null) return CharacterResult.CharacterNotFound();

                var stats = character.Statistics;
                if (useEffects)
                    stats = _effectsService.GetEffectiveStatistics(character);

                StringBuilder message = new StringBuilder($"**Name:** {character.Name}\n");

                foreach (var special in stats.Where(x => x.Statistic is Special).OrderBy(x => x.Statistic.Id))
                    message.Append($"**{special.Statistic.Name}:** {special.Value}\n");

                if (!_specService.IsSpecialSet(character))
                    message.Append($"*You have {character.SpecialPoints} S.P.E.C.I.A.L. points left to spend!*");

                var embed = EmbedHelper.BuildBasicEmbed("Command: $special", message.ToString());

                await ReplyAsync(userInfo.Mention, embed: embed);
                return GenericResult.FromSilentSuccess();
            }

            [Command("spend")]
            [Alias("put", "upgrade")]
            public async Task<RuntimeResult> UpgradeSpecialAsync(Special special)
            {
                var userInfo = Context.User;
                var character = await _charService.GetCharacterAsync(userInfo.Id);

                if (character == null) return CharacterResult.CharacterNotFound();
                if (!_specService.IsSpecialSet(character)) return StatisticResult.SpecialNotSet();

                return _specService.UpgradeSpecial(character, special);
            }

            [Command("set")]
            public async Task<RuntimeResult> SetSpecialAsync(Special special, int amount)
            {
                var userInfo = Context.User;
                var character = await _charService.GetCharacterAsync(userInfo.Id);

                if (character == null) return CharacterResult.CharacterNotFound();
                if (character.Level > 1 && _specService.IsSpecialSet(character)) return StatisticResult.SpecialAlreadySet();

                return await _specService.SetInitialSpecialAsync(character, special, amount);
            }
        }
    }
}
