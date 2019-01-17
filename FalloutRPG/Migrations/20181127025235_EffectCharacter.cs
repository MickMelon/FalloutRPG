using Microsoft.EntityFrameworkCore.Migrations;

namespace FalloutRPG.Migrations
{
    public partial class EffectCharacter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = OFF;");

            migrationBuilder.DropTable("EffectSpecial");
            migrationBuilder.DropTable("EffectSkill");
            migrationBuilder.DropTable("Effects");

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

            migrationBuilder.CreateIndex(
                name: "IX_EffectCharacter_CharacterId",
                table: "EffectCharacter",
                column: "CharacterId");

            migrationBuilder.Sql("PRAGMA foreign_key_check");
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
