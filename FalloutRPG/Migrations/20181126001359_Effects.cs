using Microsoft.EntityFrameworkCore.Migrations;

namespace FalloutRPG.Migrations
{
    public partial class Effects : Migration
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
                    OwnerId = table.Column<ulong>(nullable: false),
                    CharacterId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Effects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Effects_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateIndex(
                name: "IX_Effects_CharacterId",
                table: "Effects",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectSkill_EffectId",
                table: "EffectSkill",
                column: "EffectId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectSpecial_EffectId",
                table: "EffectSpecial",
                column: "EffectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EffectSkill");

            migrationBuilder.DropTable(
                name: "EffectSpecial");

            migrationBuilder.DropTable(
                name: "Effects");
        }
    }
}
