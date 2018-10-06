using System;

namespace FalloutRPG.Models
{
    public abstract class Item : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int Value { get; set; }

        public double Weight { get; set; }

        public bool Equipped { get; set; }
    }
}
