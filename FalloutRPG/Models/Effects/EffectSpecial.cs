using FalloutRPG.Constants;

namespace FalloutRPG.Models.Effects
{
    public class EffectSpecial : BaseModel
    {
        public Globals.SpecialType SpecialAttribute { get; set; }
        public int EffectValue { get; set; }
    }
}
