using Discord.Commands;
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
        public async Task ViewItemInfoAsync(string itemName)
        {
            var item = await _itemService.GetItemAsync(itemName);

            if (item == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_ITEM_NOT_FOUND, Context.User.Mention));
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"*{item.Name}*\n" +
                $"**Description:** {item.Description}\n" +
                $"**Value:** {item.Value}\n" +
                $"**Weight:** {item.Weight} lbs\n");

            if (item is ItemWeapon)
            {
                sb.Append($"**Damage:** {((ItemWeapon)item).Damage}");
                sb.Append($"**Ammo Type:** {((ItemWeapon)item).Ammo.Name}");
                sb.Append($"**Capacity:** {((ItemWeapon)item).AmmoCapacity}");
                sb.Append($"**Ammo usage on Attack:** {((ItemWeapon)item).AmmoOnAttack}");
            }
            else if (item is ItemAmmo)
            {
                if (((ItemAmmo)item).DTMultiplier != 1)
                    sb.Append($"**DT Multiplier:** {((ItemAmmo)item).DTMultiplier}");
                if (((ItemAmmo)item).DTReduction != 0)
                    sb.Append($"**DT Reduction:** {((ItemAmmo)item).DTReduction}");
            }
            else if (item is ItemApparel)
            {
                sb.Append($"**Damage Threshold:**{((ItemApparel)item).DamageThreshold}");
                sb.Append($"**Apparel Slot:** {((ItemApparel)item).ApparelSlot}");
            }

            await ReplyAsync($"{sb.ToString()} ({Context.User.Mention})");
        }
    }
}
