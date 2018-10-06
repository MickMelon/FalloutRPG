namespace FalloutRPG.Models
{
    public class ItemApparel : Item
    {
        public ApparelSlot ApparelSlot { get; set; }

        public int DamageThreshold { get; set; }
    }

    public enum ApparelSlot
    {
        UnderBody,
        WholeBody,
        Hat,
        Eye,
        Mouth,
        Chest,
        ArmLeft,
        ArmRight,
        LegLeft,
        LegRight
    }
}
