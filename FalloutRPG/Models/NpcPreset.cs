using FalloutRPG.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FalloutRPG.Models
{
    public class NpcPreset : BaseModel
    {
        public string Name { get; set; }

        public bool Enabled { get; set; }

        public IList<StatisticValue> Statistics { get; set; }
    }
}
