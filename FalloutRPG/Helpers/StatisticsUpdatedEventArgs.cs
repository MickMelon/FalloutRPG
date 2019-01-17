using FalloutRPG.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FalloutRPG.Helpers
{
    public enum StatisticOperation
    {
        Added,
        Deleted,
        Overwritten
    }

    public class StatisticsUpdatedEventArgs : EventArgs
    {
        public StatisticOperation Operation { get; }
        public Statistic ChangedStatistic { get; }

        public StatisticsUpdatedEventArgs(StatisticOperation operation, Statistic statistic)
        {
            Operation = operation;
            ChangedStatistic = statistic;
        }
    }
}
