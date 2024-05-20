using System.Text.RegularExpressions;
using FFXIV_RaidLootAPI.DTO;

namespace FFXIV_RaidLootAPI.Models
{
    public class Gear
    {
        private static readonly int ACCESSORY_TOME_COST = 375;
        private static readonly int ARMOR_LOW_COST = 495; // Head, Hands, feet
        private static readonly int ARMOR_HIGH_COST = 825; // Body, Legs
        private static readonly int WEAPON_TOME_COST = 500; // Body, Legs
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int GearLevel { get; set; }

        public GearStage GearStage { get; set; }

        public GearType GearType { get; set; }


        // Gear object functions

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
