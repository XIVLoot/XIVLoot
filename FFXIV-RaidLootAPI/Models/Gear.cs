using System.Text.RegularExpressions;
using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.DTO;

namespace FFXIV_RaidLootAPI.Models
{
    public class Gear
    {
        private static readonly int ACCESSORY_TOME_COST = 375;
        private static readonly int ARMOR_LOW_COST = 495; // Head, Hands, feet
        private static readonly int ARMOR_HIGH_COST = 825; // Body, Legs
        private static readonly int WEAPON_TOME_COST = 500; // Body, Legs
        private static readonly string TOME_GEAR = "Credendum";
        private static readonly string RAID_GEAR = "Ascension";
        private static readonly string AUGMENT_TOME = "Augment";
        private static readonly string CRAFTED_GEAR = "Crafted";
        private static readonly Dictionary<string,List<string>> POSSIBLE_NAME = new Dictionary<string,List<string>> 
        {
            {"Head",new List<string> {"Circlet", "Face", "Blinder", "Hat", "Turban"}},
            {"Body",new List<string> {"Mail", "Cuirass", "Cloak", "Corselet", "Robe", "Surcoat"}},
            {"Hands",new List<string> {"Gauntlets", "Gloves", "Armguards", "Halfgloves"}},
            {"Legs",new List<string> {"Hose", "Breeches", "Trousers", "Longkilt"}},
            {"Feet",new List<string> {"Sollerets", "Sabatons", "Longboots", "Sandals"}},
            {"Earrings",new List<string> {"Earring"}},
            {"Necklace",new List<string> {"Necklace"}},
            {"Bracelets",new List<string> {"Bracelet"}},
            {"Ring",new List<string> {"Ring"}}
        };


        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int GearLevel { get; set; }

        public GearStage GearStage { get; set; }

        public GearType GearType { get; set; }


        // Gear object functions

        public static Gear CreateGearFromEtro(string ItemLevel, string name)
        {   
            GearStage stage = GearStage.Preparation;
            if (name.IndexOf(TOME_GEAR, StringComparison.OrdinalIgnoreCase) >= 0)
                stage = GearStage.Tomes;
             // Not doing else if here so it will catch the upgrade if it is but won't if its only tome.
            if (name.IndexOf(AUGMENT_TOME, StringComparison.OrdinalIgnoreCase) >= 0)
                stage = GearStage.Upgraded_Tomes;
            else if (name.IndexOf(RAID_GEAR, StringComparison.OrdinalIgnoreCase) >= 0)
                stage = GearStage.Raid;

            GearType type = GearType.Weapon;
            bool FoundMatch = false;
            bool IsRing = false;
            foreach (KeyValuePair<string, List<string>> pair in POSSIBLE_NAME)
            {   

                foreach (string PossibleName in pair.Value)
                {
                    if (name.IndexOf(PossibleName, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        FoundMatch=true;
                        IsRing = pair.Key == "Ring";
                        if (!IsRing)
                            type = (GearType) Enum.Parse(typeof(GearType), pair.Key);
                        else
                            type=GearType.LeftRing; // TODO MAKE IT NOT ALWAYS LEFT RING
                        break;
                    }

                }
                if (FoundMatch) break;
            }

            return new Gear() {
                Name=name,
                GearLevel=int.Parse(ItemLevel),
                GearType=type,
                GearStage=stage
            };

        }   


        public CostDTO GetCost(Gear BisGear){
            // Returns the Cost required to get from the called upon gear object to the BisGear.

            int TotalTomeCost = 0;
            int TotalTwineCost = 0;
            int TotalShineCost = 0;
            int TotalSolventCost = 0;
            int TotalHermeticCost = 0;


            if (GearStage == BisGear.GearStage) // Equal so no cost
                return new CostDTO {TomeCost=TotalTomeCost, TwineCost=TotalTwineCost, ShineCost=TotalShineCost,SolventCost=TotalSolventCost,WeaponTomestoneCost=TotalHermeticCost};

            if ((GearStage == GearStage.Raid || GearStage == GearStage.Preparation) && // Need to buy tome
                (BisGear.GearStage == GearStage.Upgraded_Tomes || BisGear.GearStage == GearStage.Tomes))
                {
                    switch (GearType)
                    {
                        case GearType.Weapon:
                            TotalTomeCost += WEAPON_TOME_COST;
                            TotalHermeticCost += 1;
                            break;
                        case GearType.Head:
                        case GearType.Hands:
                        case GearType.Feet:
                            TotalTomeCost += ARMOR_LOW_COST;
                            break;
                        case GearType.Body:
                        case GearType.Legs:
                            TotalTomeCost += ARMOR_HIGH_COST;
                            break;
                        case GearType.Earrings:
                        case GearType.Necklace:
                        case GearType.Bracelets:
                        case GearType.LeftRing:
                        case GearType.RightRing:
                            TotalTomeCost += ACCESSORY_TOME_COST;
                            break;
                    }
                }

            
            // Need to augment (and maybe buy)
            if ((GearStage == GearStage.Raid | GearStage == GearStage.Preparation | BisGear.GearStage == GearStage.Tomes) &&
                (BisGear.GearStage == GearStage.Upgraded_Tomes))
                {
                    switch (GearType)
                    {
                        case GearType.Weapon:
                            TotalSolventCost+=1;
                            break;
                        case GearType.Head:
                        case GearType.Body:
                        case GearType.Hands:
                        case GearType.Legs:
                        case GearType.Feet:
                            TotalTwineCost += 1;
                            break;
                        case GearType.Earrings:
                        case GearType.Necklace:
                        case GearType.Bracelets:
                        case GearType.LeftRing:
                        case GearType.RightRing:
                            TotalShineCost += 1;
                            break;
                    }
                }
            return new CostDTO {TomeCost=TotalTomeCost, TwineCost=TotalTwineCost, ShineCost=TotalShineCost,SolventCost=TotalSolventCost,WeaponTomestoneCost=TotalHermeticCost};

        }

    }

    public enum GearStage
    {
        Preparation = 1,
        Tomes = 2,
        Upgraded_Tomes = 3,
        Raid = 4
    }

    public enum GearType
    {
        Weapon = 1,
        Head = 2,
        Body = 3,
        Hands = 4,
        Legs = 5,
        Feet = 6,
        Earrings = 7,
        Necklace = 8,
        Bracelets = 9,
        RightRing = 10,
        LeftRing = 11
    }
}
