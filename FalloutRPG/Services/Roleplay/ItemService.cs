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
    public class ItemService
    {
        private readonly IRepository<Item> _itemRepo;

        public ItemService(IRepository<Item> itemRepo)
        {
            _itemRepo = itemRepo;
        }

        public async Task<Item> GetItemAsync(string name) =>
            await _itemRepo.Query.Where(x => x.Name.Equals(name)).FirstOrDefaultAsync();

        public List<Item> GetEquippedItems(Character character) =>
            character.Inventory.Where(x => x.Equipped == true).ToList();

        public int GetDamageThreshold(Character character) =>
            GetEquippedItems(character).OfType<ItemApparel>().Sum(x => x.DamageThreshold);

        public double GetDamageSkillMultiplier(ItemWeapon weapon, int skillValue)
        {
            double skillMultiplier = skillValue / weapon.SkillMinimum;
            
            if (skillMultiplier < 0.5)
                skillMultiplier = 0.5;
            else if (skillMultiplier > 1)
                skillMultiplier = 1;

            return skillMultiplier;
        }

        public bool HasAmmo(Character character, ItemWeapon weapon) =>
            character.Inventory.OfType<ItemAmmo>().Where(x => x.Equals(weapon.Ammo)).Count() >= weapon.AmmoOnAttack;
    }
}
