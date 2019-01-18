using System.Collections.Generic;
using System.Threading.Tasks;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using FalloutRPG.Services.Roleplay;
using static FalloutRPG.Constants.Globals;
using Xunit;
using Moq;
using Discord.WebSocket;
using Discord;

namespace FalloutRPG.Tests.Services.Roleplay
{
    public class ExperienceServiceTests
    {
        #region GiveExperienceAsync() Tests
        /*
        This doesn't work now because need to mock SocketGuildUser but it doesn't
        have a constructor so can't. The main issue is having SendMessageAsync
        in a service.

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(500)]
        [InlineData(999999)]
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

            var client = new Mock<DiscordSocketClient>();
            client.Setup(x => x.GetUser(1)).Returns(new Mock<SocketGuildUser>().Object);
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository);
            var expService = new ExperienceService(charService, statsService, client.Object, TestHelper.BuildConfig(), new System.Random());

            // Act
            var character = await charService.GetCharacterAsync(1);
            int initialExp = character.Experience;
            bool result = await expService.GiveExperienceAsync(character, value);

            // Assert
            Assert.Equal(initialExp + value, character.Experience);
        }*/

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
            for (int i = 0; i < 21; i++)
            {
                // Act
                int expForLevel = expService.CalculateExperienceForLevel(i + 1);

                // Assert
                Assert.Equal(validExpLevels[i], expForLevel);
            }
        }

        [Theory]
        [InlineData(-2)]
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
            Assert.Equal(-1, expForLevel);
        }
        #endregion

        #region CalculateRemainingExperienceToNextLevel() Tests
        [Theory]
        [InlineData(0, 1000)]
        [InlineData(500, 500)]
        [InlineData(999, 1)]
        [InlineData(1000, 0)]
        [InlineData(1001, 1999)]        
        [InlineData(3000, 0)]
        [InlineData(3001, 2999)]
        public void CalculateRemainingExperience_ValidValues(int currentExp, int expectedRemaining)
        {
            // Arrange
            var expService = new ExperienceService(null, null, null, TestHelper.BuildConfig(), new System.Random());
            int[] expLevels = 
            { 
                0, 1000, 3000, 6000, 10000, 15000,
                21000, 28000, 36000, 45000, 55000,
                66000, 78000, 91000, 105000, 120000,
                136000, 153000, 171000, 190000, 210000 
            }; 

            // Act
            int expToNextLevel = expService.CalculateRemainingExperienceToNextLevel(currentExp);

            // Assert
            Assert.Equal(expectedRemaining, expToNextLevel);
            
        }
        
        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        public void CalculateRemainingExperience_InvalidValues(int value)
        {
            // Arrange
            var expService = new ExperienceService(null, null, null, TestHelper.BuildConfig(), new System.Random());

            // Act
            int expToNextLevel = expService.CalculateRemainingExperienceToNextLevel(value);

            // Assert
            Assert.Equal(-1, expToNextLevel);
        }
        #endregion

        #region CalculateLevelForExperience() Tests
        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(1000, 1)]
        [InlineData(1001, 2)]
        [InlineData(3000, 2)]
        [InlineData(3001, 3)]
        [InlineData(6000, 3)]
        [InlineData(6001, 4)]
        public void CalculateLevelForExperience_ValidValues(int currentExp, int expectedLevel)
        {
            // Arrange
            var expService = new ExperienceService(null, null, null, TestHelper.BuildConfig(), new System.Random());

            // Act
            int expToNextLevel = expService.CalculateLevelForExperience(currentExp);

            // Assert
            Assert.Equal(expectedLevel, expToNextLevel);
        }
        #endregion

        #region IsInExperienceEnabledChannel() Tests
        [Fact]
        public void IsInExperienceEnabledChannel_ValidChannel_True()
        {
            // Arrange
            var expService = new ExperienceService(null, null, null, TestHelper.BuildConfig(), new System.Random());

            // TestConfig.json contains channels 1,2,3,4,5
            for (int i = 1; i < 6; i++)
            {
                // Act
                bool result = expService.IsInExperienceEnabledChannel((ulong)i);

                // Assert
                Assert.True(result);
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(6)] // 6 is not in TestConfig.json
        public void IsInExperienceEnabledChannel_InvalidChannel_False(int value)
        {
            // Arrange
            var expService = new ExperienceService(null, null, null, TestHelper.BuildConfig(), new System.Random());

            // Act
            bool result = expService.IsInExperienceEnabledChannel((ulong)value);

            // Assert
            Assert.False(result);
        }
        #endregion

        #region GetPriceMultiplier() Tests
        /*
        Will finish when new config is set up
        [Fact] 
        public void GetPriceMultiplier_PriceIncreaseDisabledValidLevel()
        {
            // Arrange
            var config = TestHelper.BuildConfig();
            config.
            var expService = new ExperienceService(null, null, null, TestHelper.BuildConfig(), new System.Random());

            // Act
            double result = expService.GetPriceMultiplier(1);

            // Assert
        }*/
        #endregion
    } 
}