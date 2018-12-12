using System.Collections.Generic;

namespace FalloutRPG.Models
{
    public class StatisticValue : BaseModel
    {
        public Statistic Statistic { get; set; }

        public int Value { get; set; }
    }
}
