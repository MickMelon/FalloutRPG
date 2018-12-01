using FalloutRPG.Models;
using FalloutRPG.Models.Effects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace FalloutRPG.Data
{
    public class RpgContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Effect> Effects { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<NpcPreset> NpcPresets { get; set; }

        public RpgContext(DbContextOptions<RpgContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EffectCharacter>()
                .HasKey(ec => new { ec.EffectId, ec.CharacterId });

            modelBuilder.Entity<EffectCharacter>()
                .HasOne(ec => ec.Effect)
                .WithMany(e => e.EffectCharacters)
                .HasForeignKey(ec => ec.EffectId);

            modelBuilder.Entity<EffectCharacter>()
                .HasOne(ec => ec.Character)
                .WithMany(c => c.EffectCharacters)
                .HasForeignKey(ec => ec.CharacterId);

            modelBuilder.Entity<PlayerCharacter>();
            modelBuilder.Entity<ItemAmmo>();
            modelBuilder.Entity<ItemApparel>();
            modelBuilder.Entity<ItemConsumable>();
            modelBuilder.Entity<ItemMisc>();
            modelBuilder.Entity<ItemWeapon>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
