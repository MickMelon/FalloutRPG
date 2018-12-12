using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FalloutRPG.Constants;

namespace FalloutRPG.Models
{
    public class Skill : Statistic
    {
        public override Globals.StatisticType StatisticType => Globals.StatisticType.Skill;

        public Special Special { get; set; }

        public int MinimumValue { get; set; }
    }
}
