namespace FFXIV_RaidLootAPI.Models
{
    public class Static
    {
        public int Id { get; set; }
        
        public string UUID { get; set; }

        public string Name { get; set; } = String.Empty;

        public required List<Players> Players { get; set; }
    }
}
