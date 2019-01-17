using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Data.Repositories;
using FalloutRPG.Helpers;
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
        private readonly IRepository<Statistic> _statRepo;

        private static List<Statistic> _statistics = new List<Statistic>();
        public static ReadOnlyCollection<Statistic> Statistics => _statistics.AsReadOnly();

        public event EventHandler<StatisticsUpdatedEventArgs> StatisticsUpdated;

        public StatisticsService(IRepository<Statistic> statRepo)
        {
            _statRepo = statRepo;

            _statistics = _statRepo.FetchAll();
        }

        protected virtual void OnStatisticsUpdated(StatisticOperation operation, Statistic stat)
        {
            ReloadStatistics(operation, stat);
            StatisticsUpdated?.Invoke(this, new StatisticsUpdatedEventArgs(operation, stat));
        }

        public async Task AddStatisticAsync(Statistic statistic)
        {
            await _statRepo.AddAsync(statistic);
            OnStatisticsUpdated(StatisticOperation.Added, statistic);
        }

        public async Task DeleteStatisticAsync(Statistic stat)
        {
            await _statRepo.DeleteAsync(stat);
            OnStatisticsUpdated(StatisticOperation.Deleted, stat);
        }

        public async Task SaveStatisticAsync(Statistic stat)
        {
            await _statRepo.SaveAsync(stat);
            OnStatisticsUpdated(StatisticOperation.Overwritten, stat);
        }

        private void ReloadStatistics(StatisticOperation operation, Statistic stat)
        {
            switch (operation)
            {
                case StatisticOperation.Added:
                        _statistics.Add(stat);
                    break;
                case StatisticOperation.Deleted:
                        _statistics.Remove(stat);
                    break;
            }
        }

        public bool NameExists(string name) =>
            Statistics.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public bool NameOrAliasExists(string name) =>
            Statistics.Any(x => x.AliasesArray.Contains(name, StringComparer.OrdinalIgnoreCase));

        /// <summary>
        /// Returns the value of the specified special.
        /// </summary>
        /// <returns>Returns 0 if stat values are null.</returns>
        public int GetStatistic(IList<StatisticValue> statSheet, Statistic stat)
        {
            var match = statSheet.FirstOrDefault(x => x.Statistic.Equals(stat));

            if (match == null)
                return 0;

            return match.Value;
        }

        /// <summary>
        /// Returns the value of the specified character's statistic matching the given Statistic Flag.
        /// </summary>
        /// <returns>Returns 0 if a matching statistic flag is not found.</returns>
        public int GetStatistic(IList<StatisticValue> statSheet, Globals.StatisticFlag flag)
        {
            var match = statSheet.FirstOrDefault(x => x.Statistic.StatisticFlag.Equals(flag));

            if (match == null)
                return 0;

            return match.Value;
        }

        /// <summary>
        /// Returns the value of the specified character's given stat.
        /// </summary>
        /// <returns>Returns 0 if character or stat values are null.</returns>
        public int GetStatistic(Character character, Statistic stat) =>
            GetStatistic(character?.Statistics, stat);

        /// <summary>
        /// Returns the value of the specified character's statistic matching the given Statistic Flag.
        /// </summary>
        /// <returns>Returns 0 if a matching statistic flag is not found.</returns>
        public int GetStatistic(Character character, Globals.StatisticFlag flag) =>
            GetStatistic(character?.Statistics, flag);

        /// <summary>
        /// Sets the value of the specified character's given special.
        /// If the stat does not exist in the Statistics list, the list is initialized first.
        /// </summary>
        /// <returns>Returns false if special is null.</returns>
        public void SetStatistic(IList<StatisticValue> statSheet, Statistic stat, int newValue)
        {
            var match = statSheet.FirstOrDefault(x => x.Statistic.Equals(stat));

            if (match == null)
            {
                InitializeStatistics(statSheet);

                match = statSheet.FirstOrDefault(x => x.Statistic.Equals(stat));
            }

            match.Value = newValue;
        }

        /// <summary>
        /// Sets the value of the specified character's given special.
        /// If the stat does not exist in the character's Statistics list, the list is initialized first.
        /// </summary>
        public void SetStatistic(Character character, Statistic stat, int newValue) =>
            SetStatistic(character?.Statistics, stat, newValue);

        /// <summary>
        /// Adds missing entries in a StatisticValue IList if
        /// the list does not already contain an entry for every statistic in the database.
        /// </summary>
        public void InitializeStatistics(IList<StatisticValue> statValues)
        {
            if (statValues == null) statValues = new List<StatisticValue>();

            foreach (var stat in Statistics)
            {
                if (!statValues.Select(x => x.Statistic).Contains(stat))
                {
                    int value = 0;

                    if (stat is Special) value = SpecialService.SPECIAL_MIN;

                    statValues.Add(
                        new StatisticValue
                        {
                            Statistic = stat,
                            Value = value
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
