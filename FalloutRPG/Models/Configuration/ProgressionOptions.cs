using System;
using System.Collections.Generic;
using System.Text;

namespace FalloutRPG.Models.Configuration
{
    public class ProgressionOptions
    {
        public bool UseOldProgression { get; set; }

        public NewProgression NewProgression { get; set; }
        public OldProgression OldProgression { get; set; }
    }
}
