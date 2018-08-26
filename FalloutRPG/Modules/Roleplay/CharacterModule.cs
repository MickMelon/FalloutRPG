using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using FalloutRPG.Addons;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using System;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("character")]
    [Alias("char")]
    [Ratelimit(Globals.RATELIMIT_TIMES, Globals.RATELIMIT_SECONDS, Measure.Seconds)]
    public class CharacterModule : InteractiveBase<SocketCommandContext>
    {
        private readonly CharacterService _charService;
        private readonly ExperienceService _expService;
        private readonly HelpService _helpService;

        public CharacterModule(
            CharacterService charService,
            ExperienceService expService,
            HelpService helpService)
        {
            _charService = charService;
            _expService = expService;
            _helpService = helpService;
        }

        [Command]
        [Alias("show", "display", "stats")]
        public async Task ShowCharacterAsync(IUser targetUser = null)
        {
            var userInfo = Context.User;
            var character = targetUser == null
                ? await _charService.GetCharacterAsync(userInfo.Id)
                : await _charService.GetCharacterAsync(targetUser.Id);

            if (character == null)
            {
                await ReplyAsync(string.Format(Messages.ERR_CHAR_NOT_FOUND, userInfo.Mention));
                return;
            }

            var level = _expService.CalculateLevelForExperience(character.Experience);
            var expToNextLevel = _expService.CalculateRemainingExperienceToNextLevel(character.Experience);

            var description = string.IsNullOrEmpty(character.Description) ? "No description." : character.Description;
            var story = string.IsNullOrEmpty(character.Story) ? "No story." : character.Story;

            var embed = EmbedHelper.BuildBasicEmbed($"{character.Name}",
                $"**Description:** {description}\n" +
                $"**Story:** ($char story)\n" +
                $"**Level:** {level}\n" +
                $"**Experience:** {character.Experience}\n" +
                $"**To Next Level:** {expToNextLevel}\n" +
                $"**Caps:** {character.Money}");

            await ReplyAsync(userInfo.Mention, embed: embed);
        }

        [Command("help")]
        [Alias("help")]
        public async Task ShowCharacterHelpAsync()
        {
            await _helpService.ShowCharacterHelpAsync(Context);
        }

        [Command("create")]
        [Alias("new")]
        public async Task CreateCharacterAsync([Remainder]string name)
        {
            var userInfo = Context.User;

            try
            {
                await _charService.CreateCharacterAsync(userInfo.Id, name);
                await ReplyAsync(string.Format(Messages.CHAR_CREATED_SUCCESS, userInfo.Mention));
            }
            catch (Exception e)
            {
                await ReplyAsync($"{Messages.FAILURE_EMOJI} {e.Message} ({userInfo.Mention})");
                return;
            }
        }
    }
}
