namespace FFXIV_RaidLootAPI.DTO;

public class TomePlanEditDTO
{
    public int playerId {get;set;}
    public int weekToEdit {get;set;}
    public string GearToAdd {get;set;} = string.Empty;
    public string GearToRemove {get;set;} = string.Empty;
    public int numberStartTomes {get;set;}
    public int numberOffsetTomes {get;set;}
}