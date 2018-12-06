using Microsoft.EntityFrameworkCore.Migrations;

namespace FalloutRPG.Migrations
{
    public partial class Npcs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove foreign key constraints from SkillSheet and Special
            migrationBuilder.Sql(@"
            PRAGMA foreign_keys = 0;

            CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                                      FROM SkillSheet;

            DROP TABLE SkillSheet;

            CREATE TABLE SkillSheet (
                Id            INTEGER NOT NULL,
                CharacterId   INT     NOT NULL,
                Barter        INT     NOT NULL,
                EnergyWeapons INT     NOT NULL,
                Explosives    INT     NOT NULL,
                Guns          INT     NOT NULL,
                Lockpick      INT     NOT NULL,
                Medicine      INT     NOT NULL,
                MeleeWeapons  INT     NOT NULL,
                Repair        INT     NOT NULL,
                Science       INT     NOT NULL,
                Sneak         INT     NOT NULL,
                Speech        INT     NOT NULL,
                Survival      INT     NOT NULL,
                Unarmed       INT     NOT NULL,
                CONSTRAINT PK_SkillSheet PRIMARY KEY (
                    Id
                )
            );

            INSERT INTO SkillSheet (
                                       Id,
                                       CharacterId,
                                       Barter,
                                       EnergyWeapons,
                                       Explosives,
                                       Guns,
                                       Lockpick,
                                       Medicine,
                                       MeleeWeapons,
                                       Repair,
                                       Science,
                                       Sneak,
                                       Speech,
                                       Survival,
                                       Unarmed
                                   )
                                   SELECT Id,
                                          CharacterId,
                                          Barter,
                                          EnergyWeapons,
                                          Explosives,
                                          Guns,
                                          Lockpick,
                                          Medicine,
                                          MeleeWeapons,
                                          Repair,
                                          Science,
                                          Sneak,
                                          Speech,
                                          Survival,
                                          Unarmed
                                     FROM sqlitestudio_temp_table;

            DROP TABLE sqlitestudio_temp_table;

            CREATE UNIQUE INDEX IX_SkillSheet_CharacterId ON SkillSheet (
                CharacterId ASC
            );

            PRAGMA foreign_keys = 1;            
            ");

            migrationBuilder.Sql(@"
            PRAGMA foreign_keys = 0;

            CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                                      FROM Special;

            DROP TABLE Special;

            CREATE TABLE Special (
                Id           INTEGER NOT NULL,
                CharacterId  INT     NOT NULL,
                Strength     INT     NOT NULL,
                Perception   INT     NOT NULL,
                Endurance    INT     NOT NULL,
                Charisma     INT     NOT NULL,
                Intelligence INT     NOT NULL,
                Agility      INT     NOT NULL,
                Luck         INT     NOT NULL,
                CONSTRAINT PK_Special PRIMARY KEY (
                    Id
                )
            );

            INSERT INTO Special (
                                    Id,
                                    CharacterId,
                                    Strength,
                                    Perception,
                                    Endurance,
                                    Charisma,
                                    Intelligence,
                                    Agility,
                                    Luck
                                )
                                SELECT Id,
                                       CharacterId,
                                       Strength,
                                       Perception,
                                       Endurance,
                                       Charisma,
                                       Intelligence,
                                       Agility,
                                       Luck
                                  FROM sqlitestudio_temp_table;

            DROP TABLE sqlitestudio_temp_table;

            CREATE UNIQUE INDEX IX_Special_CharacterId ON Special (
                CharacterId ASC
            );

            PRAGMA foreign_keys = 1;");

            // Add foreign keys to Character table (SkillsId & SpecialId)
            migrationBuilder.Sql(@"
            PRAGMA foreign_keys = 0;

            CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                                      FROM Characters;
            
            DROP TABLE Characters;
            
            CREATE TABLE Characters (
                Id          INTEGER         NOT NULL,
                DiscordId   NUMERIC (20, 0) NOT NULL,
                Description NTEXT,
                Story       NTEXT,
                Experience  INT             NOT NULL,
                SkillPoints REAL            NOT NULL,
                Money       BIGINT,
                Level       INT,
                IsReset     BIT,
                Name        NVARCHAR (64),
                Active      BIT,
                SkillsId    INTEGER,
                SpecialId   INTEGER,
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
                                       SkillPoints,
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
                                          SkillPoints,
                                          Money,
                                          Level,
                                          IsReset,
                                          Name,
                                          Active
                                     FROM sqlitestudio_temp_table;
            
            DROP TABLE sqlitestudio_temp_table;
            
            PRAGMA foreign_keys = 1;");

            // Copy Special's & Skill's Ids to new foreign keys
            migrationBuilder.Sql(@"
            update Characters 
                set SkillsId = (
                    select SkillSheet.Id 
                    from SkillSheet 
                    where SkillSheet.CharacterId = Characters.Id
                )
                where exists (
                    select * 
                    from SkillSheet 
                    where SkillSheet.CharacterId = Characters.Id
                );");

            migrationBuilder.Sql(@"
            update Characters 
                set SpecialId = (
                    select Special.Id 
                    from Special 
                    where Special.CharacterId = Characters.Id
                )
            where exists (
                select * 
                from Special 
                where Special.CharacterId = Characters.Id
            )");

            // Drop CharacterId from SkillSheet and Special tables
            migrationBuilder.Sql(@"
            PRAGMA foreign_keys = 0;

            CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                                      FROM SkillSheet;

            DROP TABLE SkillSheet;

            CREATE TABLE SkillSheet (
                Id            INTEGER NOT NULL,
                Barter        INT     NOT NULL,
                EnergyWeapons INT     NOT NULL,
                Explosives    INT     NOT NULL,
                Guns          INT     NOT NULL,
                Lockpick      INT     NOT NULL,
                Medicine      INT     NOT NULL,
                MeleeWeapons  INT     NOT NULL,
                Repair        INT     NOT NULL,
                Science       INT     NOT NULL,
                Sneak         INT     NOT NULL,
                Speech        INT     NOT NULL,
                Survival      INT     NOT NULL,
                Unarmed       INT     NOT NULL,
                CONSTRAINT PK_SkillSheet PRIMARY KEY (
                    Id
                )
            );

            INSERT INTO SkillSheet (
                                       Id,
                                       Barter,
                                       EnergyWeapons,
                                       Explosives,
                                       Guns,
                                       Lockpick,
                                       Medicine,
                                       MeleeWeapons,
                                       Repair,
                                       Science,
                                       Sneak,
                                       Speech,
                                       Survival,
                                       Unarmed
                                   )
                                   SELECT Id,
                                          Barter,
                                          EnergyWeapons,
                                          Explosives,
                                          Guns,
                                          Lockpick,
                                          Medicine,
                                          MeleeWeapons,
                                          Repair,
                                          Science,
                                          Sneak,
                                          Speech,
                                          Survival,
                                          Unarmed
                                     FROM sqlitestudio_temp_table;

            DROP TABLE sqlitestudio_temp_table;

            PRAGMA foreign_keys = 1;");


            migrationBuilder.Sql(@"
            PRAGMA foreign_keys = 0;

            CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                                      FROM Special;

            DROP TABLE Special;

            CREATE TABLE Special (
                Id           INTEGER NOT NULL,
                Strength     INT     NOT NULL,
                Perception   INT     NOT NULL,
                Endurance    INT     NOT NULL,
                Charisma     INT     NOT NULL,
                Intelligence INT     NOT NULL,
                Agility      INT     NOT NULL,
                Luck         INT     NOT NULL,
                CONSTRAINT PK_Special PRIMARY KEY (
                    Id
                )
            );

            INSERT INTO Special (
                                    Id,
                                    Strength,
                                    Perception,
                                    Endurance,
                                    Charisma,
                                    Intelligence,
                                    Agility,
                                    Luck
                                )
                                SELECT Id,
                                       Strength,
                                       Perception,
                                       Endurance,
                                       Charisma,
                                       Intelligence,
                                       Agility,
                                       Luck
                                  FROM sqlitestudio_temp_table;

            DROP TABLE sqlitestudio_temp_table;

            PRAGMA foreign_keys = 1;");

            // add foreign key constraints to Characters

            migrationBuilder.Sql(@"
            PRAGMA foreign_keys = 0;

            CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                                      FROM Characters;

            DROP TABLE Characters;

            CREATE TABLE Characters (
                Id          INTEGER         NOT NULL,
                DiscordId   NUMERIC (20, 0) NOT NULL,
                Description NTEXT,
                Story       NTEXT,
                Experience  INT             NOT NULL,
                SkillPoints REAL            NOT NULL,
                Money       BIGINT,
                Level       INT,
                IsReset     BIT,
                Name        NVARCHAR (64),
                Active      BIT,
                SkillsId    INTEGER,
                SpecialId   INTEGER,
                CONSTRAINT PK_Characters PRIMARY KEY (
                    Id
                ),
                CONSTRAINT FK_Characters_SkillSheet_SkillsId FOREIGN KEY (
                    SkillsId
                )
                REFERENCES SkillSheet (Id),
                CONSTRAINT FK_Characters_Special_SpecialId FOREIGN KEY (
                    SpecialId
                )
                REFERENCES Special (Id)
            );

            INSERT INTO Characters (
                                       Id,
                                       DiscordId,
                                       Description,
                                       Story,
                                       Experience,
                                       SkillPoints,
                                       Money,
                                       Level,
                                       IsReset,
                                       Name,
                                       Active,
                                       SkillsId,
                                       SpecialId
                                   )
                                   SELECT Id,
                                          DiscordId,
                                          Description,
                                          Story,
                                          Experience,
                                          SkillPoints,
                                          Money,
                                          Level,
                                          IsReset,
                                          Name,
                                          Active,
                                          SkillsId,
                                          SpecialId
                                     FROM sqlitestudio_temp_table;

            DROP TABLE sqlitestudio_temp_table;

            PRAGMA foreign_keys = 1;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
