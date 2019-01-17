using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FalloutRPG.Models
{
    public class StatisticValue : BaseModel
    {
        [Required]
        public Statistic Statistic { get; set; }

        public int Value { get; set; }
    }
}
