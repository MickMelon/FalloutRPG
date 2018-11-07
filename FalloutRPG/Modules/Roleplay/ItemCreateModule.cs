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
    [RequireUserPermission(GuildPermission.Administrator)]
    public class ItemCreateModule : ModuleBase<SocketCommandContext>
    {
        private readonly ItemService _itemService;
        private readonly IRepository<Item> _itemRepo;

        public ItemCreateModule(ItemService itemService, IRepository<Item> itemRepository)
        {
            _itemService = itemService;
            _itemRepo = itemRepository;
        }

        [Command("ammo")]
        public async Task CreateItemAmmoAsync(string name, string desc, int value, double weight) =>
            await CreateItemAmmoAsync(name, desc, value, weight, 1, 0);

        [Command("ammo")]
        public async Task CreateItemAmmoAsync(string name, string desc, int value, double weight, double dtMult, int dtReduction)
        {
            await _itemRepo.AddAsync(
                new ItemAmmo
                {
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
            if (Enum.TryParse(slot, true, out ApparelSlot appSlot))
            {
                await _itemRepo.AddAsync(
                    new ItemApparel
                    {
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
            await _itemRepo.AddAsync(
                new ItemConsumable
                {
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
            await _itemRepo.AddAsync(
                new ItemMisc
                {
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
            Item item = await _itemService.GetItemAsync(ammo);

            if (item is ItemAmmo ammoItem)
            {
                var weapon = new ItemWeapon
                {
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
                await _itemRepo.AddAsync(weapon);

                await ReplyAsync(String.Format(Messages.ITEM_CREATE_SUCCESS, name, "Weapon", Context.User.Mention));
            }
            else
            {
                await ReplyAsync(String.Format(Messages.ERR_ITEM_INVALID_AMMO, Context.User.Mention));
            }
        }
    }
}
