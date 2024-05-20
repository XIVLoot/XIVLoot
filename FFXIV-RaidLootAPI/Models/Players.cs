using FFXIV_RaidLootAPI.Data;

namespace FFXIV_RaidLootAPI.Models
{
    public class Players
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Role Role { get; set; }

        public bool Locked { get; set; }

        public int staticId { get; set; }

        public string EtroBiS {get; set;} = "";
        public int BisWeaponGearId { get; set; }
        public int BisHeadGearId { get; set; }
        public int BisBodyGearId { get; set; }
        public int BisHandsGearId { get; set; }
        public int BisLegsGearId { get; set; }
        public int BisFeetGearId { get; set; }
        public int BisEarringsGearId { get; set; }
        public int BiSNecklaceGearId { get; set; }
        public int BisBraceletGearId { get; set; }
        public int BisLeftRingGearId { get; set; }
        public int BisRightRingGearId { get; set; }
        public int CurWeaponGearId { get; set; }
        public int CurHeadGearId { get; set; }
        public int CurBodyGearId { get; set; }
        public int CurHandsGearId { get; set; }
        public int CurLegsGearId { get; set; }
        public int CurFeetGearId { get; set; }
        public int CurEarringsGearId { get; set; }
        public int CurNecklaceGearId { get; set; }
        public int CurBraceletGearId { get; set; }
        public int CurLeftRingGearId { get; set; }
        public int CurRightRingGearId { get; set; }



        public Dictionary<string,Gear> GetGearSetAsDict(bool useBis, DataContext context){    
            /*This function returns the gearset of the player as a dictionnary where keys are name of gear type and
              they map to the Gear object.
              bool useBis -> If set to true uses Bis gear set. If false uses current gearset.
              DataContext context -> Data context to request gear.
            */

            // TODO : Check if returned gear is null

            Dictionary<string,Gear> GearDict;

            if (useBis)
            {
                GearDict= new Dictionary<string, Gear>() {
                {"Weapon" , context.Gears.Find(BisWeaponGearId)},
                {"Head" , context.Gears.Find(BisHeadGearId)},
                {"Body" , context.Gears.Find(BisBodyGearId)},
                {"Hands" , context.Gears.Find(BisHandsGearId)},
                {"Legs" , context.Gears.Find(BisLegsGearId)},
                {"Feet" , context.Gears.Find(BisFeetGearId)},
                {"Earrings" , context.Gears.Find(BisEarringsGearId)},
                {"Necklace" , context.Gears.Find(BiSNecklaceGearId)},
                {"Bracelets" , context.Gears.Find(BisBraceletGearId)},
                {"Ring" , context.Gears.Find(BisLeftRingGearId)},
                {"Ring" , context.Gears.Find(BisRightRingGearId)}
            };
            } else 
            {
                GearDict= new Dictionary<string, Gear>() {
                {"Weapon" , context.Gears.Find(CurWeaponGearId)},
                {"Head" , context.Gears.Find(CurHeadGearId)},
                {"Body" , context.Gears.Find(CurBodyGearId)},
                {"Hands" , context.Gears.Find(CurHandsGearId)},
                {"Legs" , context.Gears.Find(CurLegsGearId)},
                {"Feet" , context.Gears.Find(CurFeetGearId)},
                {"Earrings" , context.Gears.Find(CurEarringsGearId)},
                {"Necklace" , context.Gears.Find(CurNecklaceGearId)},
                {"Bracelets" , context.Gears.Find(CurBraceletGearId)},
                {"Ring" , context.Gears.Find(CurLeftRingGearId)},
                {"Ring" , context.Gears.Find(CurRightRingGearId)}
            };
            }

            return GearDict;
        }

    }

    public enum Role
    {
        Empty = 0,
        Tank = 1,
        Healer = 2,
        DPS = 3
    }
}
