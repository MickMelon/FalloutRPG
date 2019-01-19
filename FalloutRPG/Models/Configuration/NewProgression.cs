using System;
using System.Collections.Generic;
using System.Text;

namespace FalloutRPG.Models.Configuration
{
    public class NewProgression
    {
        public PriceIncrease PriceIncrease { get; set; }
        
        public Dictionary<string, int> SkillUpgradePrices { get; set; }
        public Dictionary<string, int> SpecialUpgradePrices { get; set; }
    }
}
