namespace ffxiRaidLootAPI.DTO;

public class GearPlanSingle
{
    public List<string>? gearName {get;set;}
    public int tomeAmountByEOW {get;set;}

    public int tomeLeeWayAmount {get;set;} // This represents the amount of tomestone that can be used in this week given that the next week has a purchase requirement.
                                           // This is used to show what can be bought (if anything) if for example the user wants to buy head on week 2 (so week 1 could only be acc.)
    public int futureTomeNeed {get;set;}
    public int surplusTome {get;set;}
    public int CostOfWeek {get;set;}
    public List<string>? OptionList {get;set;}
}
public class PlayerTomePlanDto
{
        public int numberWeeks {get;set;}
        public int numberStartTomes {get;set;}
        public int numberOffsetTomes {get;set;}
        public int totalCost {get;set;}
        public List<GearPlanSingle>? gearPlanOrder {get;set;}

}