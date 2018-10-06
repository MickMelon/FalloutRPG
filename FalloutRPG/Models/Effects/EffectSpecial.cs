using FalloutRPG.Constants;

namespace FalloutRPG.Models
{
    public class EffectSpecial : BaseModel
    {
        public Globals.SpecialType SpecialAttribute { get; set; }
        public int EffectValue { get; set; }
    }
}
