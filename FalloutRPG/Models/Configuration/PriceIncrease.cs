using System;
using System.Collections.Generic;
using System.Text;

namespace FalloutRPG.Models.Configuration
{
    public class PriceIncrease
    {
        public bool Enabled { get; set; }

        public int StartingLevel { get; set; }
        public int IncreaseEvery { get; set; }

        public double MultiplierAddition { get; set; }
    }
}
