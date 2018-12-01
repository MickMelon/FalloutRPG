using Microsoft.EntityFrameworkCore.Migrations;

namespace FalloutRPG.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Effects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    OwnerId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Effects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscordId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillSheet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Barter = table.Column<int>(nullable: false),
                    EnergyWeapons = table.Column<int>(nullable: false),
                    Explosives = table.Column<int>(nullable: false),
                    Guns = table.Column<int>(nullable: false),
                    Lockpick = table.Column<int>(nullable: false),
                    Medicine = table.Column<int>(nullable: false),
                    MeleeWeapons = table.Column<int>(nullable: false),
                    Repair = table.Column<int>(nullable: false),
                    Science = table.Column<int>(nullable: false),
                    Sneak = table.Column<int>(nullable: false),
                    Speech = table.Column<int>(nullable: false),
                    Survival = table.Column<int>(nullable: false),
                    Unarmed = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillSheet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Special",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Strength = table.Column<int>(nullable: false),
                    Perception = table.Column<int>(nullable: false),
                    Endurance = table.Column<int>(nullable: false),
                    Charisma = table.Column<int>(nullable: false),
                    Intelligence = table.Column<int>(nullable: false),
                    Agility = table.Column<int>(nullable: false),
                    Luck = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Special", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EffectSkill",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Skill = table.Column<int>(nullable: false),
                    EffectValue = table.Column<int>(nullable: false),
                    EffectId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EffectSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EffectSkill_Effects_EffectId",
                        column: x => x.EffectId,
                        principalTable: "Effects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EffectSpecial",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SpecialAttribute = table.Column<int>(nullable: false),
                    EffectValue = table.Column<int>(nullable: false),
                    EffectId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EffectSpecial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EffectSpecial_Effects_EffectId",
                        column: x => x.EffectId,
                        principalTable: "Effects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Experience = table.Column<int>(nullable: false),
                    SpecialId = table.Column<int>(nullable: true),
                    SkillsId = table.Column<int>(nullable: true),
                    HitPoints = table.Column<int>(nullable: false),
                    ArmorClass = table.Column<int>(nullable: false),
                    Money = table.Column<long>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    PlayerId = table.Column<int>(nullable: true),
                    Active = table.Column<bool>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Story = table.Column<string>(nullable: true),
                    SkillPoints = table.Column<float>(nullable: true),
                    IsReset = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_SkillSheet_SkillsId",
                        column: x => x.SkillsId,
                        principalTable: "SkillSheet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Special_SpecialId",
                        column: x => x.SpecialId,
                        principalTable: "Special",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NpcPresets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Enabled = table.Column<bool>(nullable: false),
                    SpecialId = table.Column<int>(nullable: true),
                    Tag1 = table.Column<int>(nullable: false),
                    Tag2 = table.Column<int>(nullable: false),
                    Tag3 = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NpcPresets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NpcPresets_Special_SpecialId",
                        column: x => x.SpecialId,
                        principalTable: "Special",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EffectCharacter",
                columns: table => new
                {
                    EffectId = table.Column<int>(nullable: false),
                    CharacterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EffectCharacter", x => new { x.EffectId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_EffectCharacter_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EffectCharacter_Effects_EffectId",
                        column: x => x.EffectId,
                        principalTable: "Effects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Value = table.Column<int>(nullable: false),
                    Weight = table.Column<double>(nullable: false),
                    Equipped = table.Column<bool>(nullable: false),
                    CharacterId = table.Column<int>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    NpcPresetId = table.Column<int>(nullable: true),
                    DamageMultiplier = table.Column<double>(nullable: true),
                    DTMultiplier = table.Column<double>(nullable: true),
                    DTReduction = table.Column<int>(nullable: true),
                    ItemWeaponId = table.Column<int>(nullable: true),
                    ApparelSlot = table.Column<int>(nullable: true),
                    DamageThreshold = table.Column<int>(nullable: true),
                    Damage = table.Column<int>(nullable: true),
                    Skill = table.Column<int>(nullable: true),
                    SkillMinimum = table.Column<int>(nullable: true),
                    StrengthMinimum = table.Column<int>(nullable: true),
                    AmmoCapacity = table.Column<int>(nullable: true),
                    AmmoOnAttack = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_NpcPresets_NpcPresetId",
                        column: x => x.NpcPresetId,
                        principalTable: "NpcPresets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_Items_ItemWeaponId",
                        column: x => x.ItemWeaponId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_SkillsId",
                table: "Characters",
                column: "SkillsId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_SpecialId",
                table: "Characters",
                column: "SpecialId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_PlayerId",
                table: "Characters",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectCharacter_CharacterId",
                table: "EffectCharacter",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectSkill_EffectId",
                table: "EffectSkill",
                column: "EffectId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectSpecial_EffectId",
                table: "EffectSpecial",
                column: "EffectId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CharacterId",
                table: "Items",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_NpcPresetId",
                table: "Items",
                column: "NpcPresetId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemWeaponId",
                table: "Items",
                column: "ItemWeaponId");

            migrationBuilder.CreateIndex(
                name: "IX_NpcPresets_SpecialId",
                table: "NpcPresets",
                column: "SpecialId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EffectCharacter");

            migrationBuilder.DropTable(
                name: "EffectSkill");

            migrationBuilder.DropTable(
                name: "EffectSpecial");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Effects");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "NpcPresets");

            migrationBuilder.DropTable(
                name: "SkillSheet");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Special");
        }
    }
}
