
namespace FFXIV_RaidLootAPI.DTO;

public class GearOptionsDTO
{
    public class GearOption 
    {
        public string GearName {get; set;} = string.Empty;
        public string GearStage {get; set;} = string.Empty;
        public int GearItemLevel {get; set;}
    }

    public List<GearOption> GearOptionList {get;set;} = new List<GearOption>();
}