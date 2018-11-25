using Discord;
using Discord.Commands;
using FalloutRPG.Addons;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using FalloutRPG.Services.Roleplay;
using System;
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
            public async Task ShowSpecialAsync(IUser targetUser = null) =>
                await ShowSpecialAsync(targetUser);

            [Command("buffed")]
            [Alias("bshow", "bview")]
            public async Task ShowSpecialBuffedAsync(IUser targetUser = null) =>
                await ShowSpecialAsync(targetUser, true);

            private async Task ShowSpecialAsync(IUser targetUser = null, bool useEffects = false)
            {
                var userInfo = Context.User;
                var character = targetUser == null
                    ? await _charService.GetCharacterAsync(userInfo.Id)
                    : await _charService.GetCharacterAsync(targetUser.Id);

                if (character == null)
                {
                    await ReplyAsync(
                        string.Format(Messages.ERR_CHAR_NOT_FOUND, userInfo.Mention));
                    return;
                }

                if (!_specService.IsSpecialSet(character))
                {
                    await ReplyAsync(
                        string.Format(Messages.ERR_SPECIAL_NOT_FOUND, userInfo.Mention));
                    return;
                }

                var special = character.Special;
                if (useEffects)
                    special = _effectsService.GetEffectiveSpecial(character);

                var embed = EmbedHelper.BuildBasicEmbed("Command: $special",
                    $"**Name:** {character.Name}\n" +
                    $"**STR:** {special.Strength}\n" +
                    $"**PER:** {special.Perception}\n" +
                    $"**END:** {special.Endurance}\n" +
                    $"**CHA:** {special.Charisma}\n" +
                    $"**INT:** {special.Intelligence}\n" +
                    $"**AGI:** {special.Agility}\n" +
                    $"**LUC:** {special.Luck}");

                await ReplyAsync(userInfo.Mention, embed: embed);
            }

            [Command("set")]
            public async Task SetSpecialAsync(int str, int per, int end, int cha, int inte, int agi, int luc)
            {
                var userInfo = Context.User;
                var character = await _charService.GetCharacterAsync(userInfo.Id);
                var special = new int[] { str, per, end, cha, inte, agi, luc };

                if (character == null)
                {
                    await ReplyAsync(string.Format(Messages.ERR_CHAR_NOT_FOUND, userInfo.Mention));
                    return;
                }

                if (_specService.IsSpecialSet(character))
                {
                    await ReplyAsync(string.Format(Messages.ERR_SPECIAL_ALREADY_SET, userInfo.Mention));
                    return;
                }

                try
                {
                    await _specService.SetInitialSpecialAsync(character, special);
                    await ReplyAsync(string.Format(Messages.SPECIAL_SET_SUCCESS, userInfo.Mention));
                }
                catch (Exception e)
                {
                    await ReplyAsync($"{Messages.FAILURE_EMOJI} {e.Message} ({userInfo.Mention})");
                }
            }
        }
    }
}
