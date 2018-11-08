using Discord;
using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using FalloutRPG.Services.Roleplay;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("item")]
    [Alias("items")]
    public class ItemModule : ModuleBase<SocketCommandContext>
    {
        private readonly CampaignService _campaignService;
        private readonly ItemService _itemService;
        private readonly IRepository<Item> _itemRepo;

        public ItemModule(CampaignService campaignService, ItemService itemService, IRepository<Item> itemRepo)
        {
            _campaignService = campaignService;
            _itemService = itemService;
            _itemRepo = itemRepo;
        }

        [Command("info")]
        public async Task ViewItemInfoAsync([Remainder]string itemName)
        {
            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);

            if (!await _campaignService.IsModeratorAsync(campaign, Context.User.Id))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
                return;
            }

            var item = await _itemService.GetItemAsync(itemName, campaign);

            if (item == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_ITEM_NOT_FOUND, Context.User.Mention));
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"**Description:** {item.Description}\n" +
                $"**Value:** {item.Value}\n" +
                $"**Weight:** {item.Weight} lbs\n" +
                $"**Effects:** {String.Join(", ", item.Effects)}\n");

            if (item is ItemWeapon wep)
            {
                sb.Append($"**Damage:** {wep.Damage}\n");
                sb.Append($"**Ammo Type:** {String.Join(", ", wep.Ammo)}\n");
                sb.Append($"**Capacity:** {wep.AmmoCapacity}\n");
                sb.Append($"**Ammo Usage:** {wep.AmmoOnAttack}/Attack\n");
            }
            else if (item is ItemAmmo ammo)
            {
                if (ammo.DTMultiplier != 1)
                    sb.Append($"**DT Multiplier:** {ammo.DTMultiplier}\n");
                if (ammo.DTReduction != 0)
                    sb.Append($"**DT Reduction:** {ammo.DTReduction}\n");
            }
            else if (item is ItemApparel apparel)
            {
                sb.Append($"**Damage Threshold:** {apparel.DamageThreshold}\n");
                sb.Append($"**Apparel Slot:** {apparel.ApparelSlot}\n");
            }

            await ReplyAsync(embed: Helpers.EmbedHelper.BuildBasicEmbed(item.Name, sb.ToString()), message: Context.User.Mention);
        }

        [Command("addammo")]
        [Alias("ammoadd", "add ammo", "ammo add")]
        public async Task AddAmmoAsync(string weaponName, string ammoName)
        {
            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);

            if (!await _campaignService.IsModeratorAsync(campaign, Context.User.Id))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
                return;
            }

            Item weaponItem = await _itemService.GetItemAsync(weaponName, campaign);
            Item ammoItem = await _itemService.GetItemAsync(ammoName, campaign);

            if (weaponItem is ItemWeapon weapon && ammoItem is ItemAmmo ammo)
            {
                weapon.Ammo.Add(ammo);
                await _itemRepo.SaveAsync(ammo);
                await ReplyAsync(Messages.SUCCESS_EMOJI + " done successfully baby");
            }
            else
            {
                await ReplyAsync("invalid weapon or ammo");
            }
        }
    }
}
