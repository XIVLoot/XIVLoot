namespace FFXIV_RaidLootAPI.DTO;

public class PlayerGearScoreDTO
{   
    public class PlayerGearScoreDTOInside{
    public int id { get; set; }
    
    public decimal score {get; set; }
    }

    public List<PlayerGearScoreDTOInside> PlayerGearScoreList { get; set; } = new List<PlayerGearScoreDTOInside>();
}