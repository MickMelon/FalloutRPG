using System.Collections.Generic;
using System.Threading.Tasks;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using FalloutRPG.Services.Roleplay;
using Xunit;
using static FalloutRPG.Constants.Globals;

namespace FalloutRPG.UnitTests.Services.Roleplay
{
    public class ExperienceServiceTests
    {
        #region GiveExperienceAsync() Tests
        /*
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(500)]
        [InlineData(999999)]
        [InlineData(-999999)]
        public async Task GiveExperience_ValidValues_Success(int value)
        {
            // Arrange
            var context = TestHelper.SetupTestRpgContext();
            var intelligence = new Special() { StatisticFlag = StatisticFlag.Intelligence };
            await context.Specials.AddAsync(intelligence);
            await context.Characters.AddAsync(new Character() 
            { 
                DiscordId = (ulong)1,
                Active = true,
                ExperiencePoints = 0,
                Statistics = new List<StatisticValue>()
                {
                    new StatisticValue() { Statistic = intelligence,Value = 1 }
                }
            });            
            await context.SaveChangesAsync();
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository);
            var expService = new ExperienceService(charService, statsService, null, TestHelper.BuildConfig(), new System.Random());

            // Act
            var character = await charService.GetCharacterAsync(1);
            int initialExp = character.Experience;
            bool result = await expService.GiveExperienceAsync(character, value);

            // Assert
            Assert.Equal(initialExp + value, character.Experience);
        }       */ 

        // public async Task GiveExperience_EnoughToLevelUp_LevelUp()
        // public async Task GiveExperience_ExactlyEnoughToLevelUp_LevelUp()
        // public async Task GiveExperience_OneBeforeLevelingUp_DontLevelUp()
        #endregion

        #region GetExperienceFromMessage() Tests
        // public void GetExperienceFromMessage_ValidCharValidValues()
        // public void GetExperienceFromMessage_ValidCharInvalidValues()
        // public void GetExperienceFromMessage_InvalidChar()
        // public void GetExperienceFromMessage_NullChar()
        #endregion

        #region CalculateExperienceForLevel() Tests
        [Fact]
        public void CalculateExperienceForLevel_ValidLevels()
        {
            // Arrange
            var expService = new ExperienceService(null, null, null, TestHelper.BuildConfig(), new System.Random());
            // Got the validExpLevels from https://fallout.wikia.com/wiki/Level
            // Problem with this way, it only tests up to level 21. Could manually add them all the way but
            // it is a pain and doesn't seem right. Also didn't want to just assert it against the formula
            // because what would be the point in testing it? Maybe there is a better way.
            int[] validExpLevels = 
            { 
                0, 1000, 3000, 6000, 10000, 15000,
                21000, 28000, 36000, 45000, 55000,
                66000, 78000, 91000, 105000, 120000,
                136000, 153000, 171000, 190000, 210000 
            }; 

            // Alternative to using Theory with 21 InlineData tags
            for (int i = 1; i < 22; i++)
            {
                // Act
                int expForLevel = expService.CalculateExperienceForLevel(i);

                // Assert
                Assert.Equal(expForLevel, validExpLevels[i - 1]);
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1001)]
        public void CalculateExperienceForLevel_InvalidLevels(int value)
        {
            // Arrange
            var expService = new ExperienceService(null, null, null, TestHelper.BuildConfig(), new System.Random());

            // Act
            int expForLevel = expService.CalculateExperienceForLevel(value);

            // Assert
            Assert.Equal(expForLevel, -1);
        }
        #endregion
    }
}