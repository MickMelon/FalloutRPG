using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FalloutRPG.Data;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Helpers;
using FalloutRPG.Models;
using FalloutRPG.Services.Roleplay;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FalloutRPG.Tests.Services.Roleplay
{
    public class CharacterServiceTests
    {
        #region GetCharacterAsync() Tests
        [Fact]
        public async Task GetCharacter_ValidDiscordIdActiveCharacter_ReturnCharacter()
        {
            // Arrange
            var context = TestHelper.SetupTestRpgContext();
            await context.Characters.AddAsync(new Character() { DiscordId = (ulong)1, Active = true });
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
            var context = TestHelper.SetupTestRpgContext();
            await context.AddAsync(new Character() { DiscordId = (ulong)1, Active = false }); 
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
            var context = TestHelper.SetupTestRpgContext();
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
            var context = TestHelper.SetupTestRpgContext();
            await context.AddAsync(new Character() { DiscordId = (ulong)1 }); 
            await context.AddAsync(new Character() { DiscordId = (ulong)1 });
            await context.AddAsync(new Character() { DiscordId = (ulong)1 });
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
            var context = TestHelper.SetupTestRpgContext();
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

        #region DeleteCharacterAsync() Tests
        [Fact]
        public async Task DeleteCharacter_ValidCharacter_CharacterDeleted()
        {
            // Arrange
            var context = TestHelper.SetupTestRpgContext();
            var charToDelete = new Character() { Id = 1 };
            await context.AddAsync(charToDelete);
            await context.SaveChangesAsync();
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository);

            // Act
            await charService.DeleteCharacterAsync(charToDelete);
            var character = await charRepository.Query.Where(c => c.Id == charToDelete.Id).FirstOrDefaultAsync();

            // Assert
            Assert.Null(character);
        }

        [Fact]
        public async Task DeleteCharacter_InvalidCharacter_DoNothing()
        {
            // Arrange
            var context = TestHelper.SetupTestRpgContext();
            var nonExistingCharacter = new Character() { Id = 1 };
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository);

            // Act
            await charService.DeleteCharacterAsync(nonExistingCharacter);
        }

        [Fact]
        public async Task DeleteCharacter_NullCharacter_ThrowException()
        {
            // Arrange
            var context = TestHelper.SetupTestRpgContext();
            var nonExistingCharacter = new Character() { Id = 1 };
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository);

            // Act
            async Task act() => await charService.DeleteCharacterAsync(null);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }
        #endregion
    
        #region SaveCharacterAsync() Tests
        [Fact]
        public async Task SaveCharacter_ValidCharacter_SaveSuccessful()
        {
            // Arrange
            var context = TestHelper.SetupTestRpgContext();
            var character = new Character() { Id = 1, Money = 1000 };
            await context.AddAsync(character);
            await context.SaveChangesAsync();
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository);   

            // Act
            character.Money = 5000;
            await charService.SaveCharacterAsync(character);

            // Assert
            var characterDb = await charRepository.Query.Where(c => c.Id == character.Id).FirstOrDefaultAsync();
            Assert.Equal(character.Money, characterDb.Money);
        }

        [Fact]
        public async Task SaveCharacter_InvalidCharacter_SaveUnsuccessful()
        {
            // Arrange
            var context = TestHelper.SetupTestRpgContext();
            var character = new Character() { Id = 1 };
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository);   

            // Act
            await charService.SaveCharacterAsync(character);
        }

        [Fact]
        public async Task SaveCharacter_NullCharacter_ThrowException()
        {
            // Arrange
            var context = TestHelper.SetupTestRpgContext();
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository);   

            // Act
            async Task act() => await charService.SaveCharacterAsync(null);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }
        #endregion
    
        #region CreateCharacterAsync() Tests
        [Fact]
        public async Task CreateCharacter_ValidDetails_CharacterCreated()
        {
            // Arrange
            var context = TestHelper.SetupTestRpgContext();
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository); 

            // Act
            var character = await charService.CreateCharacterAsync(1, "Foo");

            // Assert
            var characterDb = await charRepository.Query.Where(c => c.Id == character.Id).FirstOrDefaultAsync();
            Assert.Equal(character, characterDb);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("Abcdefghijklmnopqrstuvwxy")] // Length: 25
        [InlineData("@@")]
        [InlineData("123")]
        [InlineData("Abc123")]
        public async Task CreateCharacter_InvalidName_ThrowException(string value)
        {
            // Arrange
            var context = TestHelper.SetupTestRpgContext(value);
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository); 

            /// Act
            async Task act() => await charService.CreateCharacterAsync(1, value);

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        [Fact]
        public async Task CreateCharacter_DuplicateName_ThrowException()
        {
            // Arrange
            var context = TestHelper.SetupTestRpgContext();
            await context.Characters.AddAsync(new Character() { DiscordId = 1, Name = "Foo"});
            await context.SaveChangesAsync();
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository); 

            // Act
            async Task act() => await charService.CreateCharacterAsync(1, "Foo");

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        [Fact]
        public async Task CreateCharacter_TooManyCharacters_ThrowException()
        {
            // Arrange
            var context = TestHelper.SetupTestRpgContext();
            var characters = Enumerable.Range(1, 25)
                .Select(i => new Character() { Id = i, DiscordId = 1, Name = $"Mock{i}" });
            await context.Characters.AddRangeAsync(characters);
            await context.SaveChangesAsync();
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository); 

            // Act
            async Task act() => await charService.CreateCharacterAsync(1, "Foo");

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }
        #endregion

        #region GetTotalCharactersAsync() Tests
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public async Task GetTotalCharacters_ReturnCorrectAmount(int value)
        {
            // Arrange
            var context = TestHelper.SetupTestRpgContext(value.ToString());
            for (int i = 0; i < value; i++)
                await context.AddAsync(new Character() { DiscordId = (ulong)i, Name = $"Mock{i}" });
            await context.SaveChangesAsync();
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository); 

            // Act
            int total = await charService.GetTotalCharactersAsync();

            // Assert
            Assert.Equal(value, total);
        }
        #endregion

        #region HasDuplicateName() Tests
        [Fact]
        public async Task HasDuplicateName_DuplicateName_True()
        {
            // Arrange
            var context = TestHelper.SetupTestRpgContext();
            await context.AddAsync(new Character() { DiscordId = 1, Name = "Foo"});
            await context.SaveChangesAsync();
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository); 

            // Act
            bool duplicate = await charService.HasDuplicateName(1, "Foo");

            // Assert
            Assert.True(duplicate);
        }

        [Fact]
        public async Task HasDuplicateName_NoDuplicateName_False()
        {
            // Arrange
            var context = TestHelper.SetupTestRpgContext();
            await context.AddAsync(new Character() { DiscordId = 1, Name = "Foo"});
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository); 

            // Act
            bool duplicate = await charService.HasDuplicateName(1, "Bar");

            // Assert
            Assert.False(duplicate);
        }

        [Fact]
        public async Task HasDuplicateName_InvalidDiscordId_False()
        {
            // Arrange
            var context = TestHelper.SetupTestRpgContext();
            var statsRepository = new EfSqliteRepository<Statistic>(context);
            var charRepository = new EfSqliteRepository<Character>(context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository); 

            // Act
            bool duplicate = await charService.HasDuplicateName(1, "Foo");

            // Assert
            Assert.False(duplicate);
        }
        #endregion
    }
}