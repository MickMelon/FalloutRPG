using FalloutRPG.Models;
using FalloutRPG.Models.Effects;
using Microsoft.EntityFrameworkCore;
using System;

namespace FalloutRPG.Data
{
    public class RpgContext : DbContext
    {
        public DbSet<Character> Characters { get; set; }
        public DbSet<Effect> Effects { get; set; }

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
        }
    }
}
