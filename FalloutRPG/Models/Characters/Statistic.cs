using System;
using System.ComponentModel.DataAnnotations.Schema;
using static FalloutRPG.Constants.Globals;

namespace FalloutRPG.Models
{
    public abstract class Statistic : BaseModel, IEquatable<Statistic>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Aliases { get; set; }

        public StatisticFlag StatisticFlag { get; set; } = StatisticFlag.None;

        [NotMapped]
        public string[] AliasesArray 
        {
            get
            {
                if (!String.IsNullOrEmpty(Aliases))
                    return Aliases.Split("/");
                else
                    return new string[] { Name };
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Statistic stat)
                return Equals(stat);

            return false;
        }

        public bool Equals(Statistic other)
        {
            return other != null &&
                   Name == other.Name &&
                   Description == other.Description &&
                   Aliases == other.Aliases &&
                   StatisticFlag == other.StatisticFlag;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Description, Aliases, StatisticFlag);
        }
    }
}
