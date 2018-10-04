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
        private readonly CampaignService _campaignService;
        private readonly PlayerService _playerService;
        private readonly CharacterService _characterService;
        private readonly ExperienceService _experienceService;

        public CampaignCheatModule(
            CampaignService campaignService, 
            PlayerService playerService, 
            CharacterService characterService,
            ExperienceService experienceService)
        {
            _campaignService = campaignService;
            _playerService = playerService;
            _characterService = characterService;
            _experienceService = experienceService;
        }

        [Command("setlevel")]
        public async Task SetLevelAsync(IUser user, int level)
        {
            if (level < 1)
            {
                await ReplyAsync("no.");
                return;
            }

            var receiver = await _playerService.GetPlayerAsync(user.Id);
            var moderator = await _playerService.GetPlayerAsync(Context.User.Id);

            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);
            if (campaign == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_CHANNEL_COMMAND, Context.User.Mention));
                return;
            }
            if (!campaign.Moderators.Contains(moderator))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
                return;
            }
            if (!campaign.Players.Contains(receiver))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_A_MEMBER, Context.User.Mention));
                return;
            }
            var character = await _characterService.GetPlayerCharacterAsync(receiver.DiscordId);
            if (character.Campaign == null || !character.Campaign.Equals(campaign))
            {
                await ReplyAsync("yo this character isn't converted yet/for this campaign");
                return;
            }

            int expForLevel = _experienceService.CalculateExperienceForLevel(level);
            if (expForLevel < character.Experience)
            {
                await ReplyAsync("no.");
                return;
            }
            int difference = expForLevel - character.Experience;

            await _experienceService.GiveExperienceAsync(character, difference);

            await ReplyAsync("the deed is done");
        }

        [Command("giveitem")]
        public async Task GiveItemAsync(IUser user, string itemName)
        {
            throw new NotImplementedException("bro hold on until items are at least present before trying to cheat");

            var receiver = await _playerService.GetPlayerAsync(user.Id);
            var giver = await _playerService.GetPlayerAsync(Context.User.Id);

            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);
            if (campaign == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_CHANNEL_COMMAND, Context.User.Mention));
                return;
            }
            if (!campaign.Moderators.Contains(giver))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
                return;
            }
            if (!campaign.Players.Contains(receiver))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_A_MEMBER, Context.User.Mention));
                return;
            }
            var character = await _characterService.GetPlayerCharacterAsync(receiver.DiscordId);
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
