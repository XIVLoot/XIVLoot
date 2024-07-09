namespace FFXIV_RaidLootAPI.DTO;

public class ClaimedPlayerInfoDTO
{
    public string Name { get; set; } = string.Empty;
    public string Job {get;set;} = string.Empty;
    public int pId {get;set;}
    public int PlayerId {get;set;}
    public string StaticName {get;set;} = string.Empty;
    public string StaticUUID {get;set;} = string.Empty;
    public int CurrentAverageItemLevel {get;set;}
    public int BisAverageItemLevel {get;set;}
    public CostDTO Cost {get;set;} = new CostDTO();

}