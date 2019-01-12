using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FalloutRPG.Data;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using FalloutRPG.Services.Roleplay;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FalloutRPG.UnitTests.Services.Roleplay
{
    public class CharacterServiceTests
    {
        #region GetCharacterAsync() Tests
        [Fact]
        public async Task GetCharacter_ValidDiscordIdActiveCharacter_ReturnCharacter()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<RpgContext>()
                .UseInMemoryDatabase(databaseName: "ValidDiscordIdActiveCharacter_ReturnCharacter")
                .Options;
            var context = new RpgContext(options);
            context.Characters.Add(new Character() { DiscordId = (ulong)1, Active = true });
            await context.SaveChangesAsync();
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository);

            // Act
            var character = await charService.GetCharacterAsync(1);

            // Assert
            Assert.NotNull(character);
        }

        [Fact]
        public async Task GetCharacter_ValidDiscordIdNoActiveCharacter_ReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<RpgContext>()
                .UseInMemoryDatabase(databaseName: "ValidDiscordIdNoActiveCharacter_ReturnNull")
                .Options;
            var context = new RpgContext(options);
            context.Add(new Character() 
            {
                DiscordId = (ulong)1,
                Active = false // Set active to false
            }); 
            await context.SaveChangesAsync();
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository);

            // Act 
            var character = await charService.GetCharacterAsync(1);

            // Assert
            Assert.Null(character);
        }

        [Fact]
        public async Task GetCharacter_InvalidDiscordId_ReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<RpgContext>()
                .UseInMemoryDatabase(databaseName: "InvalidDiscordId_ReturnNull")
                .Options;
            var context = new RpgContext(options);
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository);

            // Act
            var character = await charService.GetCharacterAsync(1);

            // Assert
            Assert.Null(character);
        }
        #endregion

        #region GetAllCharactersAsync() Tests
        [Fact]
        public async Task GetAllCharacters_ValidDiscordIdMultipleCharacters_ReturnAllCharacters()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<RpgContext>()
                .UseInMemoryDatabase(databaseName: "ValidDiscordIdMultipleCharacters_ReturnAllCharacters")
                .Options;
            var context = new RpgContext(options);
            context.Add(new Character() { DiscordId = (ulong)1 }); 
            context.Add(new Character() { DiscordId = (ulong)1 });
            context.Add(new Character() { DiscordId = (ulong)1 });
            await context.SaveChangesAsync();
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository);

            // Act
            var characters = await charService.GetAllCharactersAsync(1);

            // Assert
            Assert.True(characters.Count == 3);
        }

        [Fact]
        public async Task GetAllCharacters_InvalidDiscordId_ReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<RpgContext>()
                .UseInMemoryDatabase(databaseName: "GetAllCharacters_InvalidDiscordId_ReturnNull")
                .Options;
            var context = new RpgContext(options);
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository);

            // Act
            var characters = await charService.GetAllCharactersAsync(1);

            // Assert
            Assert.True(characters.Count == 0);
        }
        #endregion
    }
}