using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FalloutRPG.Services.Roleplay
{
    public class StatisticsService
    {
        private readonly CharacterService _charService;

        private readonly IRepository<Statistic> _statRepo;

        public ReadOnlyCollection<Statistic> Statistics { get; private set; }

        public StatisticsService(CharacterService charService, IRepository<Statistic> statRepo)
        {
            _charService = charService;
            _statRepo = statRepo;
        }

        public async Task AddStatisticAsync(Statistic statistic)
        {
            await _statRepo.AddAsync(statistic);
            await ReloadStatisticsAsync();
        }

        public async Task DeleteStatisticAsync(Statistic stat)
        {
            await _statRepo.DeleteAsync(stat);
            await ReloadStatisticsAsync();
        }

        private async Task ReloadStatisticsAsync() =>
            Statistics = new ReadOnlyCollection<Statistic>(await _statRepo.FetchAllAsync());

        public bool NameExists(string name) =>
            Statistics.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Returns the value of the specified special.
        /// </summary>
        /// <returns>Returns -1 if special values are null.</returns>
        public int GetStatistic(IList<StatisticValue> statSheet, Statistic stat)
        {
            var match = statSheet.FirstOrDefault(x => x.Statistic.Equals(stat));

            if (match == null)
                return -1;

            return match.Value;
        }

        /// <summary>
        /// Returns the value of the specified character's given special.
        /// </summary>
        /// <returns>Returns -1 if character or special values are null.</returns>
        public int GetStatistic(Character character, Statistic stat) =>
            GetStatistic(character?.Statistics, stat);

        /// <summary>
        /// Sets the value of the specified character's given special.
        /// </summary>
        /// <returns>Returns false if special is null.</returns>
        public bool SetStatistic(IList<StatisticValue> statSheet, Statistic stat, int newValue)
        {
            var match = statSheet.FirstOrDefault(x => x.Statistic.Equals(stat));

            if (match == null)
                return false;

            match.Value = newValue;
            return true;
        }

        /// <summary>
        /// Sets the value of the specified character's given special.
        /// </summary>
        /// <returns>Returns false if character or special are null.</returns>
        public bool SetStatistic(Character character, Statistic stat, int newValue) =>
            SetStatistic(character?.Statistics, stat, newValue);

        /// <summary>
        /// Adds missing entries in a StatisticValue IList if
        /// the list does not already contain an entry for every statistic in the database.
        /// </summary>
        public void InitializeStatistics(IList<StatisticValue> statValues)
        {
            foreach (var stat in Statistics)
            {
                if (!statValues.Select(x => x.Statistic).Contains(stat))
                {
                    statValues.Add(
                        new StatisticValue
                        {
                            Statistic = stat,
                            Value = 0
                        }
                    );
                }
            }
        }

        public IList<StatisticValue> CloneStatistics(IList<StatisticValue> statistics)
        {
            var copy = new List<StatisticValue>();

            foreach (var stat in statistics)
                copy.Add(new StatisticValue { Statistic = stat.Statistic, Value = stat.Value });

            return copy;
        }
    }
}
