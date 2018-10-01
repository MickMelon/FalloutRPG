﻿using Discord;
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
        private readonly PlayerService _playerService;

        private readonly IRepository<Player> _playerRepository;
        private readonly IRepository<Campaign> _campaignRepository;

        public CampaignService(PlayerService playerService, IRepository<Player> playerRepository, DiscordSocketClient client, IRepository<Campaign> campaignRepository)
        {
            _playerService = playerService;
            _playerRepository = playerRepository;
            _client = client;
            _campaignRepository = campaignRepository;
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
            await SaveCampaignAsync(campaign);
        }

        public async Task<Campaign> GetCampaignAsync(string name) =>
            await _campaignRepository.Query.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync();

        public async Task<Campaign> GetOwnedCampaign(Player owner) =>
            await _campaignRepository.Query.Where(x => x.Owner.Equals(owner)).FirstOrDefaultAsync();

        public async Task<List<Campaign>> GetAllCampaignsAsync(Player player) =>
            await _campaignRepository.Query.Where(x => x.Players.Contains(player)).ToListAsync();

        public async Task SaveCampaignAsync(Campaign campaign) =>
            await _campaignRepository.SaveAsync(campaign);

        public async Task DeleteCampaignAsync(Campaign campaign, SocketGuild guild)
        {
            await guild.GetRole(campaign.RoleId).DeleteAsync();
            await guild.GetChannel(campaign.TextChannelId).DeleteAsync();

            await _campaignRepository.DeleteAsync(campaign);
        }
    }
}
