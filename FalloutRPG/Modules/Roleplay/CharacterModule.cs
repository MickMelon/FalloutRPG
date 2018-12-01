using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using FalloutRPG.Addons;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using System;
using System.Text;
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
                ? await _charService.GetPlayerCharacterAsync(userInfo.Id)
                : await _charService.GetPlayerCharacterAsync(targetUser.Id);

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
                $"**Inventory:** ($char inventory)\n" +
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
                await _charService.CreatePlayerCharacterAsync(userInfo.Id, name, Context.Channel.Id);
                await ReplyAsync(string.Format(Messages.CHAR_CREATED_SUCCESS, userInfo.Mention));
            }
            catch (Exception e)
            {
                await ReplyAsync($"{Messages.FAILURE_EMOJI} {e.Message} ({userInfo.Mention})");
                return;
            }
        }

        [Command("activate")]
        [Alias("active")]
        public async Task ActivateCharacterAsync([Remainder]string name)
        {
            var chars = await _charService.GetAllPlayerCharactersAsync(Context.User.Id);

            if (chars == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, Context.User.Mention));
                return;
            }

            //var charMatch = chars.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            var charMatch = chars.Find(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

            if (charMatch == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, Context.User.Mention));
                return;
            }

            if (charMatch.Active)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_ALREADY_ACTIVE, charMatch.Name, Context.User.Mention));
                return;
            }

            foreach (var character in chars.FindAll(x => x.Active))
            {
                character.Active = false;
                await _charService.SaveCharacterAsync(character);
            }

            charMatch.Active = true;
            await _charService.SaveCharacterAsync(charMatch);

            await ReplyAsync(String.Format(Messages.CHAR_ACTIVATED, charMatch.Name, Context.User.Mention));
        }

        [Command("list")]
        public async Task ListCharactersAsync()
        {
            var characters = await _charService.GetAllPlayerCharactersAsync(Context.User.Id);

            if (characters == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, Context.User.Mention));
                return;
            }

            var message = new StringBuilder();

            for (var i = 0; i < characters.Count; i++)
            {
                message.Append($"{i + 1}: {characters[i].Name}\n");
            }

            var embed = EmbedHelper.BuildBasicEmbed("Command: $character list", message.ToString());

            await ReplyAsync(Context.User.Mention, embed: embed);
        }

        [Command("remove")]
        [Alias("delete")]
        public async Task RemoveCharacterAsync([Remainder]string name)
        {
            var chars = await _charService.GetAllPlayerCharactersAsync(Context.User.Id);

            if (chars == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, Context.User.Mention));
                return;
            }

            var charMatch = chars.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (charMatch == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, Context.User.Mention));
                return;
            }

            if (charMatch.Active)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_CANT_REMOVE_ACTIVE, charMatch.Name, Context.User.Mention));
                return;
            }

            await ReplyAsync(String.Format(Messages.CHAR_REMOVE_CONFIRM, charMatch.Name, charMatch.Level, Context.User.Mention));
            var response = await NextMessageAsync();
            if (response != null && response.Content.Equals(charMatch.Name, StringComparison.OrdinalIgnoreCase))
            {
                await _charService.DeleteCharacterAsync(charMatch);
                await ReplyAsync(String.Format(Messages.CHAR_REMOVE_SUCCESS, charMatch.Name, Context.User.Mention));
            }
            else
            {
                await ReplyAsync(String.Format(Messages.CHAR_NOT_REMOVED, charMatch.Name, Context.User.Mention));
            }
        }
    }
}
