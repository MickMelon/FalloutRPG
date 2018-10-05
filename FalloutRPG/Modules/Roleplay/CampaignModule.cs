using Discord;
using Discord.Addons.Interactive;
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
    [Group("campaign")]
    [Alias("cam", "camp")]
    [RequireBotPermission(GuildPermission.ManageRoles | GuildPermission.ManageChannels)]
    public class CampaignModule : InteractiveBase<SocketCommandContext>
    {
        private readonly CampaignService _campaignService;
        private readonly PlayerService _playerService;
        private readonly CharacterService _characterService;

        public CampaignModule(CampaignService campaignService, PlayerService playerService, CharacterService characterService)
        {
            _campaignService = campaignService;
            _playerService = playerService;
            _characterService = characterService;
        }

        [Command("convert")]
        public async Task ConvertCharacterToCampaign()
        {
            var character = await _characterService.GetPlayerCharacterAsync(Context.User.Id);
            if (character == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, Context.User.Mention));
                return;
            }
            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);
            if (campaign == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_CHANNEL_COMMAND, Context.User.Mention));
                return;
            }
            var player = await _playerService.GetPlayerAsync(Context.User.Id);
            if (!campaign.Players.Contains(player))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_A_MEMBER, Context.User.Mention));
                return;
            }

            await Context.User.SendMessageAsync(String.Format(Messages.CAMP_CHAR_CONVERT_CONFIRMATION, character.Name, character.Level, Context.User.Mention));

            var response = await NextMessageAsync(new EnsureFromUserCriterion(Context.User.Id));

            if (response != null && response.Content.Equals(character.Name, StringComparison.OrdinalIgnoreCase))
            {
                character.Campaign = campaign;
                await _characterService.SaveCharacterAsync(character);

                var channel = Context.Guild.GetTextChannel(campaign.TextChannelId);
                await channel.SendMessageAsync(String.Format(Messages.CAMP_CHAR_CONVERT_SUCCESS, character.Name));
            }
            else
            {
                var channel = Context.Guild.GetTextChannel(campaign.TextChannelId);
                await channel.SendMessageAsync(String.Format(Messages.CAMP_CHAR_CONVERT_FAILURE, character.Name));
            }
        }

        [Command("create")]
        [Alias("new")]
        public async Task CreateCampaignAsync([Remainder]string name)
        {
            if (name.Length > 24 || name.Length < 2)
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NAME_LENGTH, Context.User.Mention));
                return;
            }

            var userInfo = Context.User;
            var player = await _playerService.GetPlayerAsync(userInfo.Id);

            try
            {
                await _campaignService.CreateCampaignAsync(name, Context.Guild, player);
                await ReplyAsync(String.Format(Messages.CAMP_CREATED_SUCCESS, userInfo.Mention));
            }
            catch (Exception e)
            {
                await ReplyAsync($"{Messages.FAILURE_EMOJI} {e.Message} ({userInfo.Mention})");
                return;
            }
        }

        [Command("add")]
        [Alias("invite")]
        public async Task AddMemberAsync(IUser userToAdd, [Remainder] string campaignName)
        {
            if (campaignName.Length > 24 || campaignName.Length < 2)
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NAME_LENGTH, Context.User.Mention));
                return;
            }

            var modInfo = Context.User;
            var campMod = await _playerService.GetPlayerAsync(modInfo.Id);

            var playerToAdd = await _playerService.GetPlayerAsync(userToAdd.Id);

            var campaign = await _campaignService.GetCampaignAsync(campaignName);
            
            if (campaign == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_FOUND, Context.User.Mention));
                return;
            }
            if (campaign.Players.Contains(playerToAdd))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_ALREADY_IN, modInfo.Mention));
                return;
            }
            if (!campaign.Moderators.Contains(campMod))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, modInfo.Mention));
                return;
            }

            campaign.Players.Add(playerToAdd);
            await _campaignService.SaveCampaignAsync(campaign);

            var role = Context.Guild.GetRole(campaign.RoleId);
            await Context.Guild.GetUser(playerToAdd.DiscordId).AddRoleAsync(role);

            var channel = Context.Guild.GetTextChannel(campaign.TextChannelId);
            await channel.SendMessageAsync(String.Format(Messages.CAMP_JOIN_SUCCESS, userToAdd.Mention));
        }

        [Command("delete")]
        [Alias("disband", "finish", "remove", "del")]
        public async Task DeleteCampaignAsync()
        {
            var player = await _playerService.GetPlayerAsync(Context.User.Id);
            var campaign = await _campaignService.GetOwnedCampaign(player);

            if (campaign == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_FOUND, Context.User.Mention));
                return;
            }

            if (campaign.Owner.Equals(player))
            {
                int memberCount = campaign.Players.Count;

                await ReplyAsync(String.Format(Messages.CAMP_REMOVE_CONFIRM, campaign.Name, memberCount, Context.User.Mention));
                var response = await NextMessageAsync();

                if (response != null && response.Content.Equals(campaign.Name, StringComparison.OrdinalIgnoreCase))
                {
                    await _campaignService.DeleteCampaignAsync(campaign, Context.Guild);
                    await ReplyAsync(String.Format(Messages.CAMP_REMOVE_SUCCESS, Context.User.Mention));
                }
                else
                {
                    await ReplyAsync(String.Format(Messages.CAMP_NOT_REMOVED, campaign.Name, Context.User.Mention));
                }
            }
            else
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_OWNER, Context.User.Mention));
            }
        }

        [Command("mod")]
        public async Task AddModeratorAsync(IUser user)
        {
            // gets the player of the person sending the command
            var senderPlayer = await _playerService.GetPlayerAsync(Context.User.Id);

            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);
            if (campaign == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_CHANNEL_COMMAND, Context.User.Mention));
                return;
            }
            if (!campaign.Owner.Equals(senderPlayer))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_OWNER, Context.User.Mention));
                return;
            }

            var modPlayer = await _playerService.GetPlayerAsync(user.Id);
            
            if (!campaign.Players.Contains(modPlayer))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_A_MEMBER, Context.User.Mention));
                return;
            }

            campaign.Moderators.Add(modPlayer);
            await _campaignService.SaveCampaignAsync(campaign);
            await ReplyAsync(String.Format(Messages.CAMP_MOD_SUCCESS, user.Mention, Context.User.Mention));
        }
    }
}
