﻿// <auto-generated />
using System;
using FalloutRPG.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FalloutRPG.Migrations
{
    [DbContext(typeof(RpgContext))]
    [Migration("20181127025235_EffectCharacter")]
    partial class EffectCharacter
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932");

            modelBuilder.Entity("FalloutRPG.Models.Character", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Description");

                    b.Property<ulong>("DiscordId");

                    b.Property<int>("Experience");

                    b.Property<bool>("IsReset");

                    b.Property<int>("Level");

                    b.Property<long>("Money");

                    b.Property<string>("Name");

                    b.Property<float>("SkillPoints");

                    b.Property<string>("Story");

                    b.HasKey("Id");

                    b.ToTable("Characters");
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

            modelBuilder.Entity("FalloutRPG.Models.SkillSheet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Barter");

                    b.Property<int>("CharacterId");

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

                    b.HasIndex("CharacterId")
                        .IsUnique();

                    b.ToTable("SkillSheet");
                });

            modelBuilder.Entity("FalloutRPG.Models.Special", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Agility");

                    b.Property<int>("CharacterId");

                    b.Property<int>("Charisma");

                    b.Property<int>("Endurance");

                    b.Property<int>("Intelligence");

                    b.Property<int>("Luck");

                    b.Property<int>("Perception");

                    b.Property<int>("Strength");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId")
                        .IsUnique();

                    b.ToTable("Special");
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

            modelBuilder.Entity("FalloutRPG.Models.SkillSheet", b =>
                {
                    b.HasOne("FalloutRPG.Models.Character")
                        .WithOne("Skills")
                        .HasForeignKey("FalloutRPG.Models.SkillSheet", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FalloutRPG.Models.Special", b =>
                {
                    b.HasOne("FalloutRPG.Models.Character")
                        .WithOne("Special")
                        .HasForeignKey("FalloutRPG.Models.Special", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}