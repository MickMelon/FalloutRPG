using Discord;
using Discord.Commands;
using FalloutRPG.Addons;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using FalloutRPG.Services.Roleplay;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("character")]
    [Alias("char")]
    public class CharacterInfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly CharacterService _charService;

        public CharacterInfoModule(CharacterService charService)
        {
            _charService = charService;
        }

        [Command("changename")]
        [Ratelimit(1, Globals.RATELIMIT_SECONDS, Measure.Seconds)]
        public async Task<RuntimeResult> ChangeCharacterNameAsync([Remainder]string name)
        {
            var character = await _charService.GetCharacterAsync(Context.User.Id);
            
            if (character == null) return CharacterResult.CharacterNotFound();

            if (!StringHelper.IsOnlyLetters(name)) return GenericResult.FromError("Name contained non-alphabetic characters.");

            if (name.Length > 24 || name.Length < 2) return GenericResult.FromError("Name was too long or short.");

            var fixedName = StringHelper.ToTitleCase(name);

            if (await _charService.HasDuplicateName(Context.User.Id, fixedName)) return GenericResult.FromError("Name is a duplicate.");

            character.Name = fixedName;

            await _charService.SaveCharacterAsync(character);
            return GenericResult.FromSuccess(Messages.CHAR_CHANGED_NAME);
        }

        [Group("story")]
        public class CharacterStoryModule : ModuleBase<SocketCommandContext>
        {
            private readonly CharacterService _charService;

            public CharacterStoryModule(CharacterService service)
            {
                _charService = service;
            }

            [Command]
            [Alias("show")]
            [Ratelimit(1, Globals.RATELIMIT_SECONDS, Measure.Seconds)]
            public async Task<RuntimeResult> ShowCharacterStoryAsync(IUser targetUser = null)
            {
                var userInfo = Context.User;
                var character = targetUser == null
                    ? await _charService.GetCharacterAsync(userInfo.Id)
                    : await _charService.GetCharacterAsync(targetUser.Id);

                if (character == null) return CharacterResult.CharacterNotFound();

                if (character.Story == null || character.Story.Equals("")) return GenericResult.FromError(Messages.ERR_STORY_NOT_FOUND);

                var embed = EmbedHelper.BuildBasicEmbed("Command: $character story",
                    $"**Name:** {character.Name}\n" +
                    $"**Story:** {character.Story}");

                await ReplyAsync(userInfo.Mention, embed: embed);
                return GenericResult.FromSilentSuccess();
            }

            [Command("update")]
            [Alias("set")]
            public async Task<RuntimeResult> UpdateCharacterStoryAsync([Remainder]string story)
            {
                var userInfo = Context.User;
                var character = await _charService.GetCharacterAsync(userInfo.Id);

                if (character == null) return CharacterResult.CharacterNotFound();

                character.Story = story;

                await _charService.SaveCharacterAsync(character);
                return GenericResult.FromSuccess(Messages.CHAR_STORY_SUCCESS);
            }
        }

        [Group("description")]
        [Alias("desc")]
        public class CharacterDescriptionModule : ModuleBase<SocketCommandContext>
        {
            private readonly CharacterService _charService;

            public CharacterDescriptionModule(CharacterService service)
            {
                _charService = service;
            }

            [Command]
            [Alias("show")]
            public async Task<RuntimeResult> ShowCharacterDescriptionAsync(IUser targetUser = null)
            {
                var userInfo = Context.User;
                var character = targetUser == null
                    ? await _charService.GetCharacterAsync(userInfo.Id)
                    : await _charService.GetCharacterAsync(targetUser.Id);

                if (character == null) return CharacterResult.CharacterNotFound();

                if (character.Description == null || character.Description.Equals(""))
                    return GenericResult.FromError(Messages.ERR_DESC_NOT_FOUND);

                var embed = EmbedHelper.BuildBasicEmbed("Command: $character description",
                    $"**Name:** {character.Name}\n" +
                    $"**Description:** {character.Description}");

                await ReplyAsync(userInfo.Mention, embed: embed);
                return GenericResult.FromSilentSuccess();
            }

            [Command("update")]
            [Alias("set")]
            public async Task<RuntimeResult> UpdateCharacterDescriptionAsync([Remainder]string description)
            {
                var userInfo = Context.User;
                var character = await _charService.GetCharacterAsync(userInfo.Id);

                if (character == null) return CharacterResult.CharacterNotFound();

                character.Description = description;

                await _charService.SaveCharacterAsync(character);
                return GenericResult.FromSuccess(Messages.CHAR_DESC_SUCCESS);
            }
        }
    }
}
