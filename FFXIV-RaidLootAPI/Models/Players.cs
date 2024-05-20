using FFXIV_RaidLootAPI.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FFXIV_RaidLootAPI.Models
{



    public class Players
    {   
        private static readonly int GEARSETSIZE = 11;
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public Job Job {get; set; }

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
        public int BisNecklaceGearId { get; set; }
        public int BisBraceletsGearId { get; set; }
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
        public int CurBraceletsGearId { get; set; }
        public int CurLeftRingGearId { get; set; }
        public int CurRightRingGearId { get; set; }


        // Player object functions

        public Dictionary<string,Gear?> get_gearset_as_dict(bool useBis, DataContext context){    
            /*This function returns the gearset of the player as a dictionnary where keys are name of gear type and
              they map to the Gear object.
              bool useBis -> If set to true uses Bis gear set. If false uses current gearset.
              DataContext context -> Data context to request gear.
            */

            // TODO : Check if returned gear is null

            Dictionary<string,Gear?> GearDict;

            if (useBis)
            {
                GearDict= new Dictionary<string, Gear?>() {
                {"Weapon" , context.Gears.Find(BisWeaponGearId)},
                {"Head" , context.Gears.Find(BisHeadGearId)},
                {"Body" , context.Gears.Find(BisBodyGearId)},
                {"Hands" , context.Gears.Find(BisHandsGearId)},
                {"Legs" , context.Gears.Find(BisLegsGearId)},
                {"Feet" , context.Gears.Find(BisFeetGearId)},
                {"Earrings" , context.Gears.Find(BisEarringsGearId)},
                {"Necklace" , context.Gears.Find(BisNecklaceGearId)},
                {"Bracelets" , context.Gears.Find(BisBraceletsGearId)},
                {"RightRing" , context.Gears.Find(BisLeftRingGearId)},
                {"LeftRing" , context.Gears.Find(BisRightRingGearId)}
            };
            } else 
            {
                GearDict= new Dictionary<string, Gear?>() {
                {"Weapon" , context.Gears.Find(CurWeaponGearId)},
                {"Head" , context.Gears.Find(CurHeadGearId)},
                {"Body" , context.Gears.Find(CurBodyGearId)},
                {"Hands" , context.Gears.Find(CurHandsGearId)},
                {"Legs" , context.Gears.Find(CurLegsGearId)},
                {"Feet" , context.Gears.Find(CurFeetGearId)},
                {"Earrings" , context.Gears.Find(CurEarringsGearId)},
                {"Necklace" , context.Gears.Find(CurNecklaceGearId)},
                {"Bracelets" , context.Gears.Find(CurBraceletsGearId)},
                {"RightRing" , context.Gears.Find(CurLeftRingGearId)},
                {"LeftRing" , context.Gears.Find(CurRightRingGearId)}
            };
            }

            return GearDict;
        }

        public int get_avg_item_level(Dictionary<string,Gear?>? GearDict=null, bool UseBis=false, DataContext context=null){
            /*Returns the average item level of the player. If GearDict is given a value uses that GearDict to compute it.
            Otherwise calls get_gearset_as_dict to get it. If GearDict is not null a DataContext must be specified.
            Returns -1 if the an error occured.
            GearDict -> GearDict formatted using get_gearset_as_dict.
            UseBis -> If true computes avg ilevel for Bis. If a non null GearDict is given this value does not matter.
            */

            if (GearDict == null){
                if (context == null){return -1;}
                GearDict = get_gearset_as_dict(UseBis, context);
            }
            int TotalItemLevel = 0;
            foreach (KeyValuePair<string, Gear?> pair in GearDict)
            {
                if (!(pair.Value is null)) 
                    TotalItemLevel += pair.Value.GearLevel;
            }

            return TotalItemLevel/GEARSETSIZE;
        }
    
        public void change_gear_piece(GearType GearToChange, bool UseBis, int NewGearId)
        {/*Updates the id of the specified gear piece for the player..
        GearToChange -> Value of the GearType to change.
        UseBis -> If true changes the value for Bis. Else for current.
        NewGearId -> Id (in database) of new gear to change to.
        */

        switch (GearToChange){
            case GearType.Weapon:
                if (UseBis)
                    BisWeaponGearId = NewGearId;
                else 
                    CurWeaponGearId = NewGearId;
                return;
            case GearType.Head:
                if (UseBis)
                    BisHeadGearId = NewGearId;
                else 
                    CurHeadGearId = NewGearId;
                return;
            case GearType.Body:
                if (UseBis)
                    BisBodyGearId = NewGearId;
                else 
                    CurBodyGearId = NewGearId;
                return;
            case GearType.Hands:
                if (UseBis)
                    BisHandsGearId = NewGearId;
                else 
                    CurHandsGearId = NewGearId;
                return;
            case GearType.Legs:
                if (UseBis)
                    BisLegsGearId = NewGearId;
                else 
                    CurLegsGearId = NewGearId;
                return;
            case GearType.Feet:
                if (UseBis)
                    BisFeetGearId = NewGearId;
                else 
                    CurFeetGearId = NewGearId;
                return;
            case GearType.Earrings:
                if (UseBis)
                    BisEarringsGearId = NewGearId;
                else 
                    CurEarringsGearId = NewGearId;
                return;
            case GearType.Necklace:
                if (UseBis)
                    BisNecklaceGearId = NewGearId;
                else 
                    CurNecklaceGearId = NewGearId;
                return;
            case GearType.Bracelets:
                if (UseBis)
                    BisBraceletsGearId = NewGearId;
                else 
                    CurBraceletsGearId = NewGearId;
                return;
            case GearType.RightRing: // TODO Add logic for both rings.
                if (UseBis)
                    BisRightRingGearId = NewGearId;
                else 
                    CurRightRingGearId = NewGearId;
                return;
            case GearType.LeftRing: // TODO Add logic for both rings.
                if (UseBis)
                    BisLeftRingGearId = NewGearId;
                else 
                    CurLeftRingGearId = NewGearId;
                return;
        }

        }
    }

    public enum Job
    {
    BlackMage = 1,
    Summoner = 2,
    RedMage = 3,
    WhiteMage = 4,
    Astrologian = 5,
    Sage = 6,
    Scholar = 7,
    Ninja = 8,
    Samurai = 9,
    Reaper = 10,
    Monk = 11,
    Dragoon = 12,
    Gunbreaker = 13,
    DarkKnight = 14,
    Paladin = 15,
    Warrior = 16,
    Machinist = 17,
    Bard = 18,
    Dancer = 19
    }
}
