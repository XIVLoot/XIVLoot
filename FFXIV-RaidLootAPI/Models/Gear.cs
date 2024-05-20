namespace FFXIV_RaidLootAPI.Models
{
    public class Gear
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int GearItemLevel { get; set; }

        public GearLevel GearLevel { get; set; }

        public GearType GearType { get; set; }
    }

    public enum GearLevel
    {
        Preparation = 1,
        Tomes = 2,
        Upgraded_Tomes = 3,
        Raid = 4
    }

    public enum GearType
    {
        Head = 1,
        Chest = 2,
        Gloves = 3,
        Pants = 4,
        Boots = 5,
        Earring = 6,
        Necklace = 7,
        Wristband = 8,
        Ring = 9
    }
}
