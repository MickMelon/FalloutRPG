﻿using Discord.Commands;
using FalloutRPG.Constants;
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
        private readonly ItemService _itemService;

        public ItemModule(ItemService itemService)
        {
            _itemService = itemService;
        }

        [Command]
        [Alias("info")]
        public async Task ViewItemInfoAsync([Remainder]string itemName)
        {
            var item = await _itemService.GetItemAsync(itemName);

            if (item == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_ITEM_NOT_FOUND, Context.User.Mention));
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"**Description:** {item.Description}\n" +
                $"**Value:** {item.Value}\n" +
                $"**Weight:** {item.Weight} lbs\n");

            if (item is ItemWeapon wep)
            {
                sb.Append($"**Damage:** {wep.Damage}\n");
                sb.Append($"**Ammo Type:** {wep.Ammo.Name}\n");
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
    }
}