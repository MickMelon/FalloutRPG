using Microsoft.EntityFrameworkCore.Migrations;

namespace FalloutRPG.Migrations
{
    public partial class Statistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EffectSkill");

            migrationBuilder.DropTable(
                name: "EffectSpecial");

            migrationBuilder.DropTable(
                name: "NpcPresets");

            migrationBuilder.DropTable(
                name: "SkillSheet");

            migrationBuilder.DropTable(
                name: "Special");

            migrationBuilder.CreateTable(
                name: "NpcPresets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NpcPresets", x => x.Id);
                });

            // Drop and add new columns to Character
            migrationBuilder.Sql(@"
                PRAGMA foreign_keys = 0;
                
                CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                                          FROM Characters;
                
                DROP TABLE Characters;
                
                CREATE TABLE Characters (
                    Id               INTEGER         NOT NULL,
                    DiscordId        NUMERIC (20, 0) NOT NULL,
                    Description      NTEXT,
                    Story            NTEXT,
                    Experience       INT             NOT NULL,
                    Money            BIGINT,
                    Level            INT,
                    IsReset          BIT,
                    Name             NVARCHAR (64),
                    Active           BIT,
                    ExperiencePoints INTEGER,
                    SpecialPoints    INTEGER,
                    TagPoints        INTEGER,
                    CONSTRAINT PK_Characters PRIMARY KEY (
                        Id
                    )
                );
                
                INSERT INTO Characters (
                                           Id,
                                           DiscordId,
                                           Description,
                                           Story,
                                           Experience,
                                           Money,
                                           Level,
                                           IsReset,
                                           Name,
                                           Active
                                       )
                                       SELECT Id,
                                              DiscordId,
                                              Description,
                                              Story,
                                              Experience,
                                              Money,
                                              Level,
                                              IsReset,
                                              Name,
                                              Active
                                         FROM sqlitestudio_temp_table;
                
                DROP TABLE sqlitestudio_temp_table;
                
                PRAGMA foreign_keys = 1;");

            migrationBuilder.CreateTable(
                name: "Statistic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Aliases = table.Column<string>(nullable: true),
                    StatisticFlag = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    SpecialId = table.Column<int>(nullable: true),
                    MinimumValue = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Statistic_Statistic_SpecialId",
                        column: x => x.SpecialId,
                        principalTable: "Statistic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StatisticValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StatisticId = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false),
                    CharacterId = table.Column<int>(nullable: true),
                    EffectId = table.Column<int>(nullable: true),
                    NpcPresetId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatisticValue_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StatisticValue_Effects_EffectId",
                        column: x => x.EffectId,
                        principalTable: "Effects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StatisticValue_NpcPresets_NpcPresetId",
                        column: x => x.NpcPresetId,
                        principalTable: "NpcPresets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StatisticValue_Statistic_StatisticId",
                        column: x => x.StatisticId,
                        principalTable: "Statistic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Statistic_SpecialId",
                table: "Statistic",
                column: "SpecialId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticValue_CharacterId",
                table: "StatisticValue",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticValue_EffectId",
                table: "StatisticValue",
                column: "EffectId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticValue_NpcPresetId",
                table: "StatisticValue",
                column: "NpcPresetId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticValue_StatisticId",
                table: "StatisticValue",
                column: "StatisticId");

            migrationBuilder.AddForeignKey(
                name: "FK_Statistic_Statistic_SpecialId",
                table: "Statistic",
                column: "SpecialId",
                principalTable: "Statistic",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
