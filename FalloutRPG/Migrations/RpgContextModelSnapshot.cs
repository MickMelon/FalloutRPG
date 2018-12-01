﻿// <auto-generated />
using System;
using FalloutRPG.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FalloutRPG.Migrations
{
    [DbContext(typeof(RpgContext))]
    partial class RpgContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932");

            modelBuilder.Entity("FalloutRPG.Models.Character", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ArmorClass");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<int>("Experience");

                    b.Property<int>("HitPoints");

                    b.Property<long>("Money");

                    b.Property<string>("Name");

                    b.Property<int?>("SkillsId");

                    b.Property<int?>("SpecialId");

                    b.HasKey("Id");

                    b.HasIndex("SkillsId");

                    b.HasIndex("SpecialId");

                    b.ToTable("Characters");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Character");
                });

            modelBuilder.Entity("FalloutRPG.Models.Effects.Effect", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<ulong>("OwnerId");

                    b.HasKey("Id");

                    b.ToTable("Effects");
                });

            modelBuilder.Entity("FalloutRPG.Models.Effects.EffectCharacter", b =>
                {
                    b.Property<int>("EffectId");

                    b.Property<int>("CharacterId");

                    b.HasKey("EffectId", "CharacterId");

                    b.HasIndex("CharacterId");

                    b.ToTable("EffectCharacter");
                });

            modelBuilder.Entity("FalloutRPG.Models.Effects.EffectSkill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("EffectId");

                    b.Property<int>("EffectValue");

                    b.Property<int>("Skill");

                    b.HasKey("Id");

                    b.HasIndex("EffectId");

                    b.ToTable("EffectSkill");
                });

            modelBuilder.Entity("FalloutRPG.Models.Effects.EffectSpecial", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("EffectId");

                    b.Property<int>("EffectValue");

                    b.Property<int>("SpecialAttribute");

                    b.HasKey("Id");

                    b.HasIndex("EffectId");

                    b.ToTable("EffectSpecial");
                });

            modelBuilder.Entity("FalloutRPG.Models.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CharacterId");

                    b.Property<string>("Description");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<bool>("Equipped");

                    b.Property<string>("Name");

                    b.Property<int?>("NpcPresetId");

                    b.Property<int>("Value");

                    b.Property<double>("Weight");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.HasIndex("NpcPresetId");

                    b.ToTable("Items");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Item");
                });

            modelBuilder.Entity("FalloutRPG.Models.NpcPreset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Enabled");

                    b.Property<string>("Name");

                    b.Property<int?>("SpecialId");

                    b.Property<int>("Tag1");

                    b.Property<int>("Tag2");

                    b.Property<int>("Tag3");

                    b.HasKey("Id");

                    b.HasIndex("SpecialId");

                    b.ToTable("NpcPresets");
                });

            modelBuilder.Entity("FalloutRPG.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("DiscordId");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("FalloutRPG.Models.SkillSheet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Barter");

                    b.Property<int>("EnergyWeapons");

                    b.Property<int>("Explosives");

                    b.Property<int>("Guns");

                    b.Property<int>("Lockpick");

                    b.Property<int>("Medicine");

                    b.Property<int>("MeleeWeapons");

                    b.Property<int>("Repair");

                    b.Property<int>("Science");

                    b.Property<int>("Sneak");

                    b.Property<int>("Speech");

                    b.Property<int>("Survival");

                    b.Property<int>("Unarmed");

                    b.HasKey("Id");

                    b.ToTable("SkillSheet");
                });

            modelBuilder.Entity("FalloutRPG.Models.Special", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Agility");

                    b.Property<int>("Charisma");

                    b.Property<int>("Endurance");

                    b.Property<int>("Intelligence");

                    b.Property<int>("Luck");

                    b.Property<int>("Perception");

                    b.Property<int>("Strength");

                    b.HasKey("Id");

                    b.ToTable("Special");
                });

            modelBuilder.Entity("FalloutRPG.Models.PlayerCharacter", b =>
                {
                    b.HasBaseType("FalloutRPG.Models.Character");

                    b.Property<bool>("Active");

                    b.Property<string>("Description");

                    b.Property<bool>("IsReset");

                    b.Property<int?>("PlayerId");

                    b.Property<float>("SkillPoints");

                    b.Property<string>("Story");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerCharacter");

                    b.HasDiscriminator().HasValue("PlayerCharacter");
                });

            modelBuilder.Entity("FalloutRPG.Models.ItemAmmo", b =>
                {
                    b.HasBaseType("FalloutRPG.Models.Item");

                    b.Property<double>("DTMultiplier");

                    b.Property<int>("DTReduction");

                    b.Property<double>("DamageMultiplier");

                    b.Property<int?>("ItemWeaponId");

                    b.HasIndex("ItemWeaponId");

                    b.ToTable("ItemAmmo");

                    b.HasDiscriminator().HasValue("ItemAmmo");
                });

            modelBuilder.Entity("FalloutRPG.Models.ItemApparel", b =>
                {
                    b.HasBaseType("FalloutRPG.Models.Item");

                    b.Property<int>("ApparelSlot");

                    b.Property<int>("DamageThreshold");

                    b.ToTable("ItemApparel");

                    b.HasDiscriminator().HasValue("ItemApparel");
                });

            modelBuilder.Entity("FalloutRPG.Models.ItemConsumable", b =>
                {
                    b.HasBaseType("FalloutRPG.Models.Item");


                    b.ToTable("ItemConsumable");

                    b.HasDiscriminator().HasValue("ItemConsumable");
                });

            modelBuilder.Entity("FalloutRPG.Models.ItemMisc", b =>
                {
                    b.HasBaseType("FalloutRPG.Models.Item");


                    b.ToTable("ItemMisc");

                    b.HasDiscriminator().HasValue("ItemMisc");
                });

            modelBuilder.Entity("FalloutRPG.Models.ItemWeapon", b =>
                {
                    b.HasBaseType("FalloutRPG.Models.Item");

                    b.Property<int>("AmmoCapacity");

                    b.Property<int>("AmmoOnAttack");

                    b.Property<int>("Damage");

                    b.Property<int>("Skill");

                    b.Property<int>("SkillMinimum");

                    b.Property<int>("StrengthMinimum");

                    b.ToTable("ItemWeapon");

                    b.HasDiscriminator().HasValue("ItemWeapon");
                });

            modelBuilder.Entity("FalloutRPG.Models.Character", b =>
                {
                    b.HasOne("FalloutRPG.Models.SkillSheet", "Skills")
                        .WithMany()
                        .HasForeignKey("SkillsId");

                    b.HasOne("FalloutRPG.Models.Special", "Special")
                        .WithMany()
                        .HasForeignKey("SpecialId");
                });

            modelBuilder.Entity("FalloutRPG.Models.Effects.EffectCharacter", b =>
                {
                    b.HasOne("FalloutRPG.Models.Character", "Character")
                        .WithMany("EffectCharacters")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FalloutRPG.Models.Effects.Effect", "Effect")
                        .WithMany("EffectCharacters")
                        .HasForeignKey("EffectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FalloutRPG.Models.Effects.EffectSkill", b =>
                {
                    b.HasOne("FalloutRPG.Models.Effects.Effect")
                        .WithMany("SkillAdditions")
                        .HasForeignKey("EffectId");
                });

            modelBuilder.Entity("FalloutRPG.Models.Effects.EffectSpecial", b =>
                {
                    b.HasOne("FalloutRPG.Models.Effects.Effect")
                        .WithMany("SpecialAdditions")
                        .HasForeignKey("EffectId");
                });

            modelBuilder.Entity("FalloutRPG.Models.Item", b =>
                {
                    b.HasOne("FalloutRPG.Models.Character")
                        .WithMany("Inventory")
                        .HasForeignKey("CharacterId");

                    b.HasOne("FalloutRPG.Models.NpcPreset")
                        .WithMany("InitialInventory")
                        .HasForeignKey("NpcPresetId");
                });

            modelBuilder.Entity("FalloutRPG.Models.NpcPreset", b =>
                {
                    b.HasOne("FalloutRPG.Models.Special", "Special")
                        .WithMany()
                        .HasForeignKey("SpecialId");
                });

            modelBuilder.Entity("FalloutRPG.Models.PlayerCharacter", b =>
                {
                    b.HasOne("FalloutRPG.Models.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId");
                });

            modelBuilder.Entity("FalloutRPG.Models.ItemAmmo", b =>
                {
                    b.HasOne("FalloutRPG.Models.ItemWeapon")
                        .WithMany("Ammo")
                        .HasForeignKey("ItemWeaponId");
                });
#pragma warning restore 612, 618
        }
    }
}
