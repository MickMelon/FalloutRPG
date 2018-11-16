using Microsoft.EntityFrameworkCore.Migrations;

namespace FalloutRPG.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscordId = table.Column<ulong>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Story = table.Column<string>(nullable: true),
                    Experience = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    SkillPoints = table.Column<float>(nullable: false),
                    IsReset = table.Column<bool>(nullable: false),
                    Money = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillSheet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterId = table.Column<int>(nullable: false),
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
                    table.ForeignKey(
                        name: "FK_SkillSheet_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Special",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterId = table.Column<int>(nullable: false),
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
                    table.ForeignKey(
                        name: "FK_Special_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SkillSheet_CharacterId",
                table: "SkillSheet",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Special_CharacterId",
                table: "Special",
                column: "CharacterId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SkillSheet");

            migrationBuilder.DropTable(
                name: "Special");

            migrationBuilder.DropTable(
                name: "Characters");
        }
    }
}
