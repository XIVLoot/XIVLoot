using FFXIV_RaidLootAPI.Models;

namespace FFXIV_RaidLootAPI.DTO;

public class GearAcquisitionDTO
{
    public class GearAcqInfo
    {
        public string GearType {get;set;} = string.Empty;
        
        public bool IsAugment {get;set;}

        public int PlayerId {get;set;}
    }

    public Dictionary<DateOnly, List<GearAcqInfo>> info {get;set;} = new Dictionary<DateOnly, List<GearAcqInfo>>();
}