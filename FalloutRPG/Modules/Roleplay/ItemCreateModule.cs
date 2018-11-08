using Discord;
using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using FalloutRPG.Services.Roleplay;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("item create"), Alias("items create")]
    public class ItemCreateModule : ModuleBase<SocketCommandContext>
    {
        private readonly CampaignService _campaignService;
        private readonly ItemService _itemService;

        public ItemCreateModule(CampaignService campaignService, ItemService itemService)
        {
            _campaignService = campaignService;
            _itemService = itemService;
        }

        [Command("ammo")]
        public async Task CreateItemAmmoAsync(string name, string desc, int value, double weight) =>
            await CreateItemAmmoAsync(name, desc, value, weight, 1, 0);

        [Command("ammo")]
        public async Task CreateItemAmmoAsync(string name, string desc, int value, double weight, double dtMult, int dtReduction)
        {
            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);

            if (!await _campaignService.IsModeratorAsync(campaign, Context.User.Id))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
                return;
            }

            await _itemService.AddItemAsync(
                new ItemAmmo
                {
                    Campaign = campaign,
                    Name = name,
                    Description = desc,
                    Value = value,
                    Weight = weight,
                    DTMultiplier = dtMult,
                    DTReduction = dtReduction
                });

            await ReplyAsync(String.Format(Messages.ITEM_CREATE_SUCCESS, name, "Ammo", Context.User.Mention));
        }

        [Command("apparel")]
        public async Task CreateItemApparelAsync(string name, string desc, int value, double weight, string slot, int dt)
        {
            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);

            if (!await _campaignService.IsModeratorAsync(campaign, Context.User.Id))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
                return;
            }

            if (Enum.TryParse(slot, true, out ApparelSlot appSlot))
            {
                await _itemService.AddItemAsync(
                    new ItemApparel
                    {
                        Campaign = campaign,
                        Name = name,
                        Description = desc,
                        Value = value,
                        Weight = weight,
                        ApparelSlot = appSlot,
                        DamageThreshold = dt
                    });

                await ReplyAsync(String.Format(Messages.ITEM_CREATE_SUCCESS, name, "Ammo", Context.User.Mention));
            }
            else
                await ReplyAsync(String.Format(Messages.ERR_ITEM_INVALID_SLOT, Context.User.Mention));
        }

        [Command("consumable")]
        public async Task CreateItemConsumableAsync(string name, string desc, int value, double weight)
        {
            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);

            if (!await _campaignService.IsModeratorAsync(campaign, Context.User.Id))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
                return;
            }

            await _itemService.AddItemAsync(
                new ItemConsumable
                {
                    Campaign = campaign,
                    Name = name,
                    Description = desc,
                    Value = value,
                    Weight = weight
                });

            await ReplyAsync(String.Format(Messages.ITEM_CREATE_SUCCESS, name, "Consumable", Context.User.Mention));
        }

        [Command("misc")]
        public async Task CreateItemMiscAsync(string name, string desc, int value, double weight)
        {
            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);

            if (!await _campaignService.IsModeratorAsync(campaign, Context.User.Id))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
                return;
            }

            await _itemService.AddItemAsync(
                new ItemMisc
                {
                    Campaign = campaign,
                    Name = name,
                    Description = desc,
                    Value = value,
                    Weight = weight
                });

            await ReplyAsync(String.Format(Messages.ITEM_CREATE_SUCCESS, name, "Misc", Context.User.Mention));
        }

        [Command("weapon")]
        public async Task CreateItemWeaponAsync(string name, string desc, int value, double weight, int damage,
            Globals.SkillType skill, int skillMin, string ammo, int ammoCapacity, int ammoOnAttack)
        {
            var campaign = await _campaignService.GetCampaignAsync(Context.Channel.Id);

            if (!await _campaignService.IsModeratorAsync(campaign, Context.User.Id))
            {
                await ReplyAsync(String.Format(Messages.ERR_CAMP_NOT_MODERATOR, Context.User.Mention));
                return;
            }

            Item item = await _itemService.GetItemAsync(ammo, campaign);

            if (item is ItemAmmo ammoItem)
            {
                var weapon = new ItemWeapon
                {
                    Campaign = campaign,
                    Name = name,
                    Description = desc,
                    Value = value,
                    Weight = weight,
                    Damage = damage,
                    Skill = skill,
                    SkillMinimum = skillMin,
                    AmmoCapacity = ammoCapacity,
                    AmmoOnAttack = ammoOnAttack,
                    AmmoRemaining = ammoCapacity
                };
                weapon.Ammo.Add(ammoItem);
                await _itemService.AddItemAsync(weapon);

                await ReplyAsync(String.Format(Messages.ITEM_CREATE_SUCCESS, name, "Weapon", Context.User.Mention));
            }
            else
            {
                await ReplyAsync(String.Format(Messages.ERR_ITEM_INVALID_AMMO, Context.User.Mention));
            }
        }
    }
}
