namespace FFXIV_RaidLootAPI.Models
{
    public class Players
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Role Role { get; set; }

       public required List<Gear> Gears { get; set; }

        public bool Locked { get; set; }

        public int staticId { get; set; }
    }

    public enum Role
    {
        Tank = 1,
        Healer = 2,
        DPS = 3
    }
}
