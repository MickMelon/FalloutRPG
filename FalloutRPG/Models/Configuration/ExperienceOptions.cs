using System;
using System.Collections.Generic;
using System.Text;

namespace FalloutRPG.Models.Configuration
{
    public class ExperienceOptions
    {
        public ulong[] EnabledChannels { get; set; }

        public int AllowedConsecutiveMessages { get; set; }
        public int MessageLengthDivisor { get; set; }

        public double IntelligenceMultiplier { get; set; }
    }
}
