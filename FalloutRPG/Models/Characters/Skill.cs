using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FalloutRPG.Constants;
using Newtonsoft.Json;

namespace FalloutRPG.Models
{
    public class Skill : Statistic
    {
        public Special Special { get; set; }

        public int MinimumValue { get; set; }
    }
}
