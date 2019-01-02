using FalloutRPG.Constants;
using FalloutRPG.Models.Effects;
using FalloutRPG.Services.Roleplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FalloutRPG.Models
{
    public class Character : BaseModel
    {
        public ulong DiscordId { get; set; }
        public bool Active { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public string Story { get; set; }

        public int Experience { get; set; }
        public int Level
        {
            get
            {
                if (ExperienceService.UseOldProgression)
                {
                    if (Experience == 0) return 1;
                    return Convert.ToInt32((Math.Sqrt(Experience + 125) / (10 * Math.Sqrt(5))));
                }
                return Experience / 1000 + 1;
            }

            private set { }
        }

        public int ExperiencePoints { get; set; }
        public int SpecialPoints { get; set; }
        public int TagPoints { get; set; }
        
        public bool IsReset { get; set; }

        public IList<StatisticValue> Statistics { get; set; }

        [NotMapped]
        public IList<StatisticValue> Skills => Statistics.Where(x => x.Statistic is Skill).ToList();
        [NotMapped]
        public IList<StatisticValue> Special => Statistics.Where(x => x.Statistic is Special).ToList();

        public virtual IList<EffectCharacter> EffectCharacters { get; set; }

        public long Money { get; set; }
    }
}
