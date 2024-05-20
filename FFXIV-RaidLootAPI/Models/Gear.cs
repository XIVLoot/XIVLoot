namespace FFXIV_RaidLootAPI.Models
{
    public class Gear
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int GearLevel { get; set; }

        public GearStage GearStage { get; set; }

        public GearType GearType { get; set; }
    }

    public enum GearStage
    {
        Preparation = 1,
        Tomes = 2,
        Upgraded_Tomes = 3,
        Raid = 4
    }

    public enum GearType
    {
        Weapon = 1,
        Head = 2,
        Body = 3,
        Hands = 4,
        Legs = 5,
        Feet = 6,
        Earrings = 7,
        Necklace = 8,
        Bracelets = 9,
        RightRing = 10,
        LeftRing = 11
    }
}
