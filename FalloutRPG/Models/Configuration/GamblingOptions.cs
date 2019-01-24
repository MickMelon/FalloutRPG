using System;
using System.Collections.Generic;
using System.Text;

namespace FalloutRPG.Models.Configuration
{
    public class GamblingOptions
    {
        public ulong[] EnabledChannels { get; set; }

        public int MinBet { get; set; }
        public int MaxBet { get; set; }
    }
}
