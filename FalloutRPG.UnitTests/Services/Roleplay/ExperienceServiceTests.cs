using System.Threading.Tasks;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using FalloutRPG.Services.Roleplay;
using Xunit;

namespace FalloutRPG.UnitTests.Services.Roleplay
{
    public class ExperienceServiceTests
    {
        #region GiveExperienceAsync() Tests
        /*
        need to add statistics to the db for this to work due to OnLevelUpAsync
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
            context.Characters.Add(new Character() 
            { 
                DiscordId = (ulong)1,
                Active = true,
                ExperiencePoints = 0,
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
        }
        */

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
        // Maybe there is a better way to do this rather than having all these
        // InlineDatas. I'll look into it
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(17)]
        [InlineData(18)]
        [InlineData(19)]
        [InlineData(20)]
        [InlineData(21)]
        public void CalculateExperienceForLevel_ValidLevels(int value)
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

            // Act
            int expForLevel = expService.CalculateExperienceForLevel(value);

            // Assert
            Assert.Equal(expForLevel, validExpLevels[value - 1]);
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