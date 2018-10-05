using Discord;
using Discord.WebSocket;
using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Services.Roleplay
{
    public class CampaignService
    {
        private readonly DiscordSocketClient _client;

        private readonly CharacterService _characterService;
        private readonly PlayerService _playerService;
        
        private readonly IRepository<Campaign> _campaignRepository;

        public CampaignService(DiscordSocketClient client,
            CharacterService characterService,
            PlayerService playerService,
            IRepository<Campaign> campaignRepository)
        {
            _playerService = playerService;
            _characterService = characterService;
            _campaignRepository = campaignRepository;
            _client = client;
        }

        public async Task CreateCampaignAsync(string name, SocketGuild guild, Player player)
        {
            if ((await GetOwnedCampaign(player)) != null)
                throw new Exception(Exceptions.CAMP_TOO_MANY);
            if (guild.TextChannels.Where(x => x.Name.Equals($"{name}-campaign", StringComparison.OrdinalIgnoreCase)).Count() > 0)
                throw new Exception(Exceptions.CAMP_NAME_NOT_UNIQUE);

            var role = await guild.CreateRoleAsync($"{name} Campaigner");

            var channel = await guild.CreateTextChannelAsync($"{name}-campaign");

            // sets permissions so only the bot and the campaigners can see their own channel
            await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, new OverwritePermissions(viewChannel: PermValue.Deny));
            await channel.AddPermissionOverwriteAsync(role, new OverwritePermissions(viewChannel: PermValue.Allow));
            await channel.AddPermissionOverwriteAsync(_client.CurrentUser, new OverwritePermissions(viewChannel: PermValue.Allow, manageChannel: PermValue.Allow));

            await guild.GetUser(player.DiscordId).AddRoleAsync(role);

            var campaign = new Campaign(name, player, role.Id, channel.Id);
            campaign.Moderators.Add(player);
            campaign.Players.Add(player);
            await SaveCampaignAsync(campaign);
        }

        public async Task<Campaign> GetCampaignAsync(string name) =>
            await _campaignRepository.Query.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            .Include(x => x.Players)
            .Include(x => x.Moderators)
            .FirstOrDefaultAsync();

            public async Task<Campaign> GetCampaignAsync(ulong channelId) =>
            await _campaignRepository.Query.Where(x => x.TextChannelId == channelId)
            .Include(x => x.Players)
            .Include(x => x.Moderators)
            .FirstOrDefaultAsync();

        public async Task<Campaign> GetOwnedCampaign(Player owner) =>
            await _campaignRepository.Query.Where(x => x.Owner.Equals(owner))
            .Include(x => x.Players)
            .Include(x => x.Moderators)
            .FirstOrDefaultAsync();

        public async Task<List<Campaign>> GetAllCampaignsAsync(Player player) =>
            await _campaignRepository.Query.Where(x => x.Players.Contains(player))
            .Include(x => x.Players)
            .Include(x => x.Moderators)
            .ToListAsync();

        public async Task SaveCampaignAsync(Campaign campaign) =>
            await _campaignRepository.SaveAsync(campaign);

        public async Task DeleteCampaignAsync(Campaign campaign, SocketGuild guild)
        {
            await guild.GetRole(campaign.RoleId).DeleteAsync();
            await guild.GetChannel(campaign.TextChannelId).DeleteAsync();

            foreach(Character character in campaign.Characters)
                await _characterService.DeleteCharacterAsync(character);

            await _campaignRepository.DeleteAsync(campaign);
        }
    }
}
