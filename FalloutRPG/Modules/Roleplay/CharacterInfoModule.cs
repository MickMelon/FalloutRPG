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
    public class CharacterInfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly CharacterService _charService;

        public CharacterInfoModule(CharacterService charService)
        {
            _charService = charService;
        }

        [Command("changename")]
        [Ratelimit(1, Globals.RATELIMIT_SECONDS, Measure.Seconds)]
        public async Task ChangeCharacterNameAsync([Remainder]string name)
        {
            var character = await _charService.GetPlayerCharacterAsync(Context.User.Id);
            
            if (character == null) return;

            if (!StringHelper.IsOnlyLetters(name))
                return;

            if (name.Length > 24 || name.Length < 2)
                return;

            var fixedName = StringHelper.ToTitleCase(name);

            if (await _charService.HasDuplicateName(Context.User.Id, fixedName))
                return;

            character.Name = fixedName;

            await _charService.SaveCharacterAsync(character);
            await ReplyAsync(string.Format(Messages.CHAR_CHANGED_NAME, Context.User.Mention));
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
            public async Task ShowCharacterStoryAsync(IUser targetUser = null)
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

                if (character.Story == null || character.Story.Equals(""))
                {
                    await ReplyAsync(string.Format(Messages.ERR_STORY_NOT_FOUND, userInfo.Mention));
                    return;
                }

                var embed = EmbedHelper.BuildBasicEmbed("Command: $character story",
                    $"**Name:** {character.Name}\n" +
                    $"**Story:** {character.Story}");

                await ReplyAsync(userInfo.Mention, embed: embed);
            }

            [Command("update")]
            [Alias("set")]
            public async Task UpdateCharacterStoryAsync([Remainder]string story)
            {
                var userInfo = Context.User;
                var character = await _charService.GetPlayerCharacterAsync(userInfo.Id);

                if (character == null)
                {
                    await ReplyAsync(string.Format(Messages.ERR_CHAR_NOT_FOUND, userInfo.Mention));
                    return;
                }

                character.Story = story;

                await _charService.SaveCharacterAsync(character);
                await ReplyAsync(string.Format(Messages.CHAR_STORY_SUCCESS, userInfo.Mention));
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
            public async Task ShowCharacterDescriptionAsync(IUser targetUser = null)
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

                if (character.Description == null || character.Description.Equals(""))
                {
                    await ReplyAsync(string.Format(Messages.ERR_DESC_NOT_FOUND, userInfo.Mention));
                    return;
                }

                var embed = EmbedHelper.BuildBasicEmbed("Command: $character description",
                    $"**Name:** {character.Name}\n" +
                    $"**Description:** {character.Description}");

                await ReplyAsync(userInfo.Mention, embed: embed);
            }

            [Command("update")]
            [Alias("set")]
            public async Task UpdateCharacterDescriptionAsync([Remainder]string description)
            {
                var userInfo = Context.User;
                var character = await _charService.GetPlayerCharacterAsync(userInfo.Id);

                if (character == null)
                {
                    await ReplyAsync(string.Format(Messages.ERR_CHAR_NOT_FOUND, userInfo.Mention));
                    return;
                }

                character.Description = description;

                await _charService.SaveCharacterAsync(character);
                await ReplyAsync(string.Format(Messages.CHAR_DESC_SUCCESS, userInfo.Mention));
            }
        }

        [Command("inventory")]
        [Alias("inv", "items", "item")]
        public async Task ShowCharacterInventoryAsync()
        {
            var userInfo = Context.User;
            var character = await _charService.GetPlayerCharacterAsync(userInfo.Id);

            if (character == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, userInfo.Mention));
                return;
            }

            var inv = character.Inventory;

            StringBuilder sb = new StringBuilder("**Weapons:**\n");

            foreach (var item in inv.OfType<ItemWeapon>())
                sb.Append($"__*{item.Name}*__:\n" +
                    $"Damage: {item.Damage}\n" +
                    $"{item.Skill.ToString()} Skill: {item.SkillMinimum}\n" +
                    $"**Ammo Type:** {String.Join(", ", item.Ammo)}\n" +
                    $"Ammo Capacity: {item.AmmoCapacity}\n" +
                    $"Ammo Usage: {item.AmmoOnAttack}/Attack\n\n");

            sb.Append("**Apparel:**\n");
            foreach (var item in inv.OfType<ItemApparel>())
                sb.Append($"__*{item.Name}*__: DT {item.DamageThreshold}\n");

            sb.Append("**Consumables:**\n");
            foreach (var item in inv.OfType<ItemConsumable>().ToHashSet())
                sb.Append($"__*{item.Name}*__ x{inv.Count(x => x.Equals(item))}\n");

            sb.Append("**Miscellaneous:**\n");
            foreach (var item in inv.OfType<ItemMisc>())
                sb.Append($"__*{item.Name}*__\n");

            sb.Append("**Ammunition:**\n");
            foreach (var item in inv.OfType<ItemAmmo>().ToHashSet())
            {
                sb.Append($"__*{item.Name}:*__ x{inv.Count(x => x.Equals(item))}\n");
                if (item.DTMultiplier != 1)
                    sb.Append($"DT Multiplier: {item.DTMultiplier}\n");
                if (item.DTReduction != 0)
                    sb.Append($"DT Reduction: {item.DTReduction}\n");
            }
            
            await ReplyAsync(userInfo.Mention, embed: EmbedHelper.BuildBasicEmbed($"{character.Name}'s Inventory:", sb.ToString()));
        }
    }
}
