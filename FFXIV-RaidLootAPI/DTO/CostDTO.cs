
namespace FFXIV_RaidLootAPI.DTO;

public class CostDTO
{
    // Returns cost values of BiS Vs. current set
    public int TomeCost {get;set;}
    public int TwineCost {get;set;}
    public int ShineCost {get;set;}
    public int SolventCost {get;set;}
    public int WeaponTomestoneCost {get;set;}

    public static CostDTO SumCost(List<CostDTO> iter)
    {
        CostDTO ret = new CostDTO();
        foreach (CostDTO cost in iter)
        {
            ret.TomeCost += cost.TomeCost;
            ret.TwineCost += cost.TwineCost;
            ret.ShineCost += cost.ShineCost;
            ret.SolventCost += cost.SolventCost;
            ret.WeaponTomestoneCost += cost.WeaponTomestoneCost;
        }
        return ret;
    }

}