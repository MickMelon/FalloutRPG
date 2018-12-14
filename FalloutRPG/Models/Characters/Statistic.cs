using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FalloutRPG.Constants;

namespace FalloutRPG.Models
{
    public abstract class Statistic : BaseModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Aliases { get; set; }

        [NotMapped]
        public string[] AliasesArray 
        {
            get
            {
                if (!String.IsNullOrEmpty(Aliases))
                    return Aliases.Split("/");

                return null;
            }
        }
    }
}
