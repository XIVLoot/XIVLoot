using System.Numerics;
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
        public string weekDoneString {get;set;} = string.Empty;
        /*
        This string is a ';' separated list of gear name in the order the player wants to get them. A value of "Empty" means the player has not yet selected it.
        A value of "Locked" means a future week is using the entirety of tomestones from this week and so it cannot be used.
        */


    public List<List<string>> GetGearPlanOrder()
    {   
        Console.WriteLine("GearPlanString : " + gearPlanOrder);
        List<string> rList = gearPlanOrder.Split(';').ToList();
        Console.WriteLine("HERE");
        Console.WriteLine(string.Join(", ", rList));
        List<List<string>> rList2 = new List<List<string>>();
        for (int i = 0; i < rList.Count; i++)
        {
            Console.WriteLine("IN-");
            List<string> rList3 = rList[i].Split('/').ToList();
            rList3.RemoveAll(s => s == "");
            rList2.Add(rList3);   
            Console.WriteLine(string.Join(", ", rList[i].Split('/').ToList()));
        }
        return rList2;
    }

    public List<bool> GetWeekDoneList()
    {
        return weekDoneString.Split(';').Select(s => s == "1").ToList();

    }

    public void SetWeekDone(int week, bool done)
    {
        List<bool> weekDoneList = GetWeekDoneList();
        weekDoneList[week] = done;
        weekDoneString = string.Join(";", weekDoneList.Select(b => b ? "1" : "0").ToList());
    }

    public void RemoveWeekFromGearPlan(int weekindex)
    {
        List<List<string>> gearPlanOrderList = GetGearPlanOrder();
        List<bool> weekDoneList = GetWeekDoneList();
        gearPlanOrderList.RemoveAt(weekindex);
        weekDoneList.RemoveAt(weekindex);
        ReconstructGearPlanString(gearPlanOrderList);
        weekDoneString = string.Join(";", weekDoneList.Select(b => b ? "1" : "0").ToList());
    }

    public void RemoveGearFromWeek(int week, GearType type)
    {
        List<List<string>> gearPlanOrderList = GetGearPlanOrder();
        Console.WriteLine(string.Join(", ", gearPlanOrderList));
        Console.WriteLine("Trying to remove from week : " + week + type); 
        gearPlanOrderList[week].Remove(Enum.GetName(typeof(GearType), type)!);
        ReconstructGearPlanString(gearPlanOrderList);
    }

    public void ReconstructGearPlanString(List<List<string>> gearPlanOrderList)
    {
        List<string> rList = new List<string>();
        for (int i = 0;i< gearPlanOrderList.Count;i++)
        {
            rList.Add(string.Join("/", gearPlanOrderList[i]));
        }
        gearPlanOrder = string.Join(";", rList);
    }

    public void AddGearFromWeek(int week, GearType type)
    {
        List<List<string>> gearPlanOrderList = GetGearPlanOrder();
        Console.WriteLine(string.Join(", ", gearPlanOrderList));
        Console.WriteLine("Trying to add from week : " + week + type); 
        gearPlanOrderList[week].Add(Enum.GetName(typeof(GearType), type)!);
        ReconstructGearPlanString(gearPlanOrderList);
    }

    public Tuple<List<GearPlanSingle>,int> ComputeGearPlanInfo()
    {
        List<List<string>> gearPlanOrderList = GetGearPlanOrder();
        List<GearPlanSingle> rList = new List<GearPlanSingle>();

        int curTomesAmount = numberStartTomes - numberOffsetTomes + MaxTomePerWeek; // Assume more than 0
        int i = gearPlanOrderList.Count-1;
        int tomeAmountNeed = 0;
        int amountUsedFromSurplusAtStart = 0;
        int totalCost = 0;

        /* First loop goes through list in reverse order. Gatters what is needed from past weeks for future weeks.
           This way we can detect if we need some weeks to wait to make the schedule work.
           This also allows us to check which empty week should be locked (done in the other loop which traverse in the increasing order and adds surplus + eow).
        */

        while (true)
        {
            List<string> weekGearList = gearPlanOrderList[i];

            /*if (i == 0 && weekGearList.Count == 0 && gearPlanOrderList.Count > 1 && tomeAmountNeed <= numberStartTomes - numberOffsetTomes)
            {
                gearPlanOrder = gearPlanOrder.Substring(1);
                if (tomeAmountNeed <= numberStartTomes - numberOffsetTomes)
                {
                    amountUsedFromSurplusAtStart = tomeAmountNeed;
                }
                break;
            }*/
                

            
            int costOfWeek = 0;
            foreach (string gear in weekGearList){
                int cost = 0;
                switch (gear){
                    case nameof(GearType.Weapon):
                        cost = Gear.WEAPON_TOME_COST;
                        break;
                    case nameof(GearType.Head):
                    case nameof(GearType.Hands):
                    case nameof(GearType.Feet):
                        cost = Gear.ARMOR_LOW_COST;
                        break;
                    case nameof(GearType.Body):
                    case nameof(GearType.Legs):
                        cost = Gear.ARMOR_HIGH_COST;
                        break;
                    case nameof(GearType.Empty):
                    case "":
                        cost = 0; 
                        break;
                    case nameof(GearType.Earrings):
                    case nameof(GearType.Necklace):
                    case nameof(GearType.Bracelets):
                    case nameof(GearType.RightRing):
                    case nameof(GearType.LeftRing):
                        cost = Gear.ACCESSORY_TOME_COST;
                        break;
                    default:
                        break;
                }
                costOfWeek += cost;
            }
            totalCost += costOfWeek;

            int futureTomeNeed = Math.Max(0,tomeAmountNeed);
            int tomeLeeWayAmount = MaxTomePerWeek - futureTomeNeed;
            
            tomeAmountNeed = costOfWeek - tomeLeeWayAmount;

            rList.Insert(0,new GearPlanSingle(){
                gearName=weekGearList,
                tomeAmountByEOW=0,
                tomeLeeWayAmount=tomeLeeWayAmount,
                futureTomeNeed = futureTomeNeed,
                CostOfWeek = costOfWeek,
                surplusTome=Math.Max(0,-1 * tomeAmountNeed),
                OptionList=new List<string>()
            });

            if (i == 0 && tomeAmountNeed > 0 && tomeAmountNeed > numberStartTomes - numberOffsetTomes)
            {
                i=1; // Put pointer back
                gearPlanOrderList.Insert(0, new List<string>(){""}); // Add needed week
                gearPlanOrder = ";" + gearPlanOrder;
            }
            else if (i == 0 && tomeAmountNeed > 0 && tomeAmountNeed <= numberStartTomes - numberOffsetTomes)
            {
                amountUsedFromSurplusAtStart = tomeAmountNeed;
            }
            i--;

            if (i == -1)
                break;
        }

        /* Second loop that goes in increasing order */
        Console.WriteLine("Amount used from surplus at start : " + amountUsedFromSurplusAtStart);
        int curSurplus = numberStartTomes - numberOffsetTomes  - amountUsedFromSurplusAtStart;
        for(i = 0;i<rList.Count;i++)
        {   
            GearPlanSingle plan = rList[i];
            int oldSurplus = plan.surplusTome;
            plan.surplusTome = plan.surplusTome + curSurplus;
            // Will update the leeway of all weeks based on the surplus amount of all weeks.

            int available = Math.Min(plan.surplusTome, 2000);

            if (available >= Gear.ARMOR_HIGH_COST)
                plan.OptionList!.AddRange(new[] { nameof(GearType.Body), nameof(GearType.Legs) });
            if (available >= Gear.WEAPON_TOME_COST)
                plan.OptionList!.AddRange(new[] { nameof(GearType.Weapon)});
            if (available >= Gear.ARMOR_LOW_COST)
                plan.OptionList!.AddRange(new[] { nameof(GearType.Head), nameof(GearType.Hands),nameof(GearType.Feet)});
            if (available >= Gear.ACCESSORY_TOME_COST)
                plan.OptionList!.AddRange(new[] { nameof(GearType.Earrings), nameof(GearType.Necklace),nameof(GearType.Bracelets),nameof(GearType.LeftRing),nameof(GearType.RightRing)});

            

            curSurplus += oldSurplus;
            plan.surplusTome = available;
            curSurplus = Math.Min(2000, curSurplus);


            

            plan.tomeAmountByEOW = Math.Min(2000,(i == 0 ? numberStartTomes - numberOffsetTomes : rList[i-1].tomeAmountByEOW) - plan.CostOfWeek + MaxTomePerWeek); // Should never be under 0. if under 0 investigate   


            

        }

        return Tuple.Create(rList,totalCost);

        
    }



    }


}