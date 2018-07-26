using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FalloutRPG.Addons;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using FalloutRPG.Services.Roleplay;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    public class RoleplayModule : ModuleBase<SocketCommandContext>
    {
        private readonly CharacterService _charService;
        private readonly ExperienceService _expService;

        public RoleplayModule(
            CharacterService charService,
            ExperienceService expService)
        {
            _charService = charService;
            _expService = expService;
        }

        [Command("pay")]
        public async Task PayAsync(IUser user, int amount)
        {
            if (amount < 1) return;

            var sourceChar = await _charService.GetCharacterAsync(Context.User.Id);
            if (sourceChar == null) return;

            var targetChar = await _charService.GetCharacterAsync(user.Id);
            if (targetChar == null)
            {
                await ReplyAsync(string.Format(Messages.ERR_CHAR_NOT_FOUND, Context.User.Mention));
                return;
            }

            if (amount > sourceChar.Money)
            {
                await ReplyAsync(string.Format(Messages.ERR_NOT_ENOUGH_CAPS, Context.User.Mention));
                return;
            }

            sourceChar.Money -= amount;
            targetChar.Money += amount;

            await _charService.SaveCharacterAsync(sourceChar);
            await _charService.SaveCharacterAsync(targetChar);

            await ReplyAsync(string.Format(Messages.PAY_SUCCESS, user.Mention, amount, Context.User.Mention));
        }

        [Command("highscores")]
        [Ratelimit(1, Globals.RATELIMIT_SECONDS, Measure.Seconds)]
        [Alias("hiscores", "high", "hi", "highscore", "hiscore")]
        public async Task ShowHighScoresAsync()
        {
            var userInfo = Context.User;
            var charList = await _charService.GetHighScoresAsync();
            var strBuilder = new StringBuilder();

            /*for (var i = 0; i < charList.Count; i++)
            {
                var level = _expService.CalculateLevelForExperience(charList[i].Experience);

                strBuilder.Append(
                    $"**{i + 1}:** {charList[i].FirstName} {charList[i].LastName}" +
                    $" | Level: {level}" +
                    $" | Experience: {charList[i].Experience}\n");
            }

            var embed = EmbedHelper.BuildBasicEmbed("Command: $highscores", strBuilder.ToString());*/

            var fieldTitlesList = new List<string>();
            var fieldContentsList = new List<string>();

            for (var i = 0; i < charList.Count; i++)
            {
                var level = _expService.CalculateLevelForExperience(charList[i].Experience);
                fieldTitlesList.Add($"{i + 1}: {charList[i].Name}");
                fieldContentsList.Add($"Level {level} / {charList[i].Experience} XP");
            }

            var embed = EmbedHelper.BuildBasicEmbedWithFields("Command: $hiscores", string.Empty, fieldTitlesList.ToArray(), fieldContentsList.ToArray());

            await ReplyAsync(userInfo.Mention, embed: embed);
        }
    }
}
