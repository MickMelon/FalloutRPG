using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FalloutRPG.Constants;

namespace FalloutRPG.Models
{
    public class Special : Statistic
    {
        public override Globals.StatisticType StatisticType => Globals.StatisticType.Special;
    }
}
