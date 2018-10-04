using Discord;
using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Services;
using FalloutRPG.Services.Roleplay;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("camp")]
    public class CampaignCheatModule : ModuleBase<SocketCommandContext>
    {
        private readonly CampaignCheatService _cheatService;

        public CampaignCheatModule(CampaignCheatService cheatService)
        {
            _cheatService = cheatService;
        }

        [Command("setlevel")]
        public async Task SetLevelAsync(IUser user, int level)
        {
            try
            {
                await _cheatService.SetCharacterLevelAsync(Context.User.Id, user.Id, level, Context.Channel.Id);
                await ReplyAsync(String.Format(Messages.CHEAT_LEVEL_CHANGE_SUCCESS, Context.User.Mention));
            }
            catch (Exception e)
            {
                await ReplyAsync($"{Messages.FAILURE_EMOJI} {e.Message}");
                return;
            }
        }

        [Command("giveitem")]
        public async Task GiveItemAsync(IUser user, string itemName)
        {
            await ReplyAsync();
            throw new NotImplementedException("bro hold on until items are at least present before trying to cheat");

            // var receiver = await _playerService.GetPlayerAsync(user.Id);
            // var giver = await _playerService.GetPlayerAsync(Context.User.Id);

            // var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);
            // if (campaign == null)
            // {
            //     await ReplyAsync(String.Format(Messages.ERR_CAMP_CHANNEL_COMMAND, Context.User.Mention));
            //     return;
            // }
            // if (!campaign.Moderators.Contains(giver))
            // {
            //     await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
            //     return;
            // }
            // if (!campaign.Players.Contains(receiver))
            // {
            //     await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_A_MEMBER, Context.User.Mention));
            //     return;
            // }
            // var character = await _characterService.GetPlayerCharacterAsync(receiver.DiscordId);
            // if (character == null)
            // {
            //     await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, user.Mention));
            //     return;
            // }
            // i swear this might actually work in the future
            // var item = await _itemService.GetItemAsync(itemName);
            // await _itemService.AddItemAsync(item, character)
            // OR
            // character.Inventory.Add(item);
            // await _characterService.SaveCharacterAsync(character);
            // await ReplyAsync("Item added successfully!");
        }
    }
}
