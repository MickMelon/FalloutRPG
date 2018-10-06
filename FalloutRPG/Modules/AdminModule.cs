using Discord;
using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using System;
using System.Threading.Tasks;

namespace FalloutRPG.Modules
{
    [Group("admin")]
    [Alias("adm")]
    [RequireUserPermission(GuildPermission.BanMembers, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        private readonly CharacterService _charService;
        private readonly SkillsService _skillsService;
        private readonly SpecialService _specialService;
        private readonly HelpService _helpService;
        private readonly ItemService _itemService;

        public AdminModule(CharacterService charService,
            SkillsService skillsService,
            SpecialService specialService,
            HelpService helpService,
            ItemService itemService)
        {
            _charService = charService;
            _skillsService = skillsService;
            _specialService = specialService;
            _helpService = helpService;
            _itemService = itemService;
        }

        [Command]
        public async Task ShowAdminHelpAsync()
        {
            await _helpService.ShowAdminHelpAsync(Context);
        }

        [Command("givemoney")]
        public async Task GiveMoneyAsync(IUser user, int money)
        {
            var character = await _charService.GetPlayerCharacterAsync(user.Id);
            if (character == null) return;

            character.Money += money;

            await _charService.SaveCharacterAsync(character);
            await ReplyAsync(string.Format(Messages.ADM_GAVE_MONEY, Context.User.Mention));
        }

        [Command("giveskillpoints")]
        [RequireOwner]
        public async Task GiveSkillPointsAsync(IUser user, int points)
        {
            var character = await _charService.GetPlayerCharacterAsync(user.Id);
            if (character == null) return;

            character.SkillPoints += points;

            await _charService.SaveCharacterAsync(character);
            await ReplyAsync(string.Format(Messages.ADM_GAVE_SKILL_POINTS, Context.User.Mention));
        }

        [Command("giveitem")]
        public async Task AddItem(IUser user, string itemName)
        {
            var item = await _itemService.GetItemAsync(itemName);
            var character = await _charService.GetCharacterAsync(user.Id);

            if (item == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_ITEM_NOT_FOUND, Context.User.Mention));
                return;
            }
            if (character == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, user.Mention));
                return;
            }

            character.Inventory.Add(item);
            await _charService.SaveCharacterAsync(character);

            await ReplyAsync(String.Format(Messages.ITEM_GIVE_SUCCESS, item.Name, character.Name, Context.User.Mention));
        }

        [Command("changename")]
        public async Task ChangeCharacterNameAsync(IUser user, [Remainder]string name)
        {
            var character = await _charService.GetPlayerCharacterAsync(user.Id);
            if (character == null) return;

            if (!StringHelper.IsOnlyLetters(name))
                return;

            if (name.Length > 24 || name.Length < 2)
                return;

            character.Name = StringHelper.ToTitleCase(name);

            await _charService.SaveCharacterAsync(character);
            await ReplyAsync(string.Format(Messages.ADM_CHANGED_NAME, Context.User.Mention));
        }
        
        [Command("reset")]
        public async Task ResetCharacterAsync(IUser user)
        {
            var character = await _charService.GetPlayerCharacterAsync(user.Id);
            if (character == null) return;

            await _charService.ResetCharacterAsync(character);
            await ReplyAsync(string.Format(Messages.ADM_RESET, Context.User.Mention));
        }

        [Command("delete")]
        [RequireOwner]
        public async Task DeleteCharacterAsync(IUser user)
        {
            var character = await _charService.GetPlayerCharacterAsync(user.Id);
            if (character == null) return;

            await _charService.DeleteCharacterAsync(character);
            await ReplyAsync(string.Format(Messages.ADM_DELETE, Context.User.Mention));
        }
    }
}
