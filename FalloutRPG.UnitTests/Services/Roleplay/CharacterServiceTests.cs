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
        private RpgContext _context;

        public CharacterServiceTests()
        {
            InitContext();
        }

        private void InitContext()
        {
            var options = new DbContextOptionsBuilder<RpgContext>()
                .UseInMemoryDatabase(databaseName: "CharacterService")
                .Options;

            var context = new RpgContext(options);
            
            // Add all required test data here
            var characters = Enumerable.Range(1, 10)
                .Select(i => new Character()
                {
                    Id = i,
                    DiscordId = (ulong)i,
                    Name = $"Mock{i}",
                    Experience = i,
                    ExperiencePoints = i,
                    SpecialPoints = i,
                    TagPoints = i,
                    IsReset = false,
                    Money = i,
                    Statistics = new List<StatisticValue>(),
                    Active = true
                });
            context.Characters.AddRange(characters);
            context.SaveChanges();
            _context = context;
        }

        [Fact]
        public async Task GetCharacter_ExistingId_ShouldReturnCharacter()
        {
            // Arrange
            var statsRepository = new EfSqliteRepository<Statistic>(_context);
            var charRepository = new EfSqliteRepository<Character>(_context);
            var statsService = new StatisticsService(statsRepository);
            var charService = new CharacterService(statsService, charRepository);

            // Act
            var account = await charService.GetCharacterAsync(1);

            // Assert
            Assert.NotNull(account);
        }
    }
}