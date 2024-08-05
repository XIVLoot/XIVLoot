using ffxiRaidLootAPI.DTO;
using FFXIV_RaidLootAPI.Models;

namespace ffxiRaidLootAPI.Models
{
    public class PlayerTomePlan
    {

        private readonly int MaxTomePerWeek = 450;

        public int Id {get;set;}
        public int playerId {get;set;}
        public int numberWeeks {get;set;}
        public int numberStartTomes {get;set;}
        public int numberOffsetTomes {get;set;}
        public string gearPlanOrder {get;set;} = string.Empty; 
        /*
        This string is a ';' separated list of gear name in the order the player wants to get them. A value of "Empty" means the player has not yet selected it.
        A value of "Locked" means a future week is using the entirety of tomestones from this week and so it cannot be used.
        */


    public List<string> GetGearPlanOrder()
    {
        return gearPlanOrder.Split(';').ToList();
    }

    public void EditGearPlanOrder(int index, GearType gearType)
    {
        List<string> gearPlanOrderList = GetGearPlanOrder();
        gearPlanOrderList[index] = Enum.GetName(typeof(GearType), gearType)!;
        // TODO : CHECK IF LEGAL MODIFICATION (NEVER GO UNDER NEEDED TOME)
        gearPlanOrder = string.Join(";", gearPlanOrderList);
    }

    public List<GearPlanSingle> ComputeGearPlanInfo()
    {
        List<string> gearPlanOrderList = GetGearPlanOrder();
        List<GearPlanSingle> rList = new List<GearPlanSingle>();

        int curTomesAmount = numberStartTomes - numberOffsetTomes + MaxTomePerWeek; // Assume more than 0
        int i = 0;

        foreach (string gear in gearPlanOrderList)
        {
            switch (gear){
                case nameof(GearType.Empty):
                    break;
                case nameof(GearType.Weapon):
                    curTomesAmount-= Gear.WEAPON_TOME_COST;
                    break;
                case nameof(GearType.Head):
                case nameof(GearType.Hands):
                case nameof(GearType.Feet):
                    curTomesAmount-= Gear.ARMOR_LOW_COST;
                    break;
                case nameof(GearType.Body):
                case nameof(GearType.Legs):
                    curTomesAmount-= Gear.ARMOR_HIGH_COST;
                    break;
                case nameof(GearType.Earrings):
                case nameof(GearType.Necklace):
                case nameof(GearType.Bracelets):
                case nameof(GearType.RightRing):
                case nameof(GearType.LeftRing):
                    curTomesAmount-= Gear.ACCESSORY_TOME_COST;
                    break;
                default:
                    break;
            }
            rList.Add(new GearPlanSingle(){
                gearName=gear,
                tomeAmountByEOW=curTomesAmount,
                tomeLeeWayAmount=0
            });
            curTomesAmount+=MaxTomePerWeek;
            i++;
        }
        return rList;
    }



    }


}