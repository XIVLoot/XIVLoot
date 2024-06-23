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
        private static readonly Dictionary<string,List<string>> GEAR_TYPE_NAME = new Dictionary<string,List<string>> 
        {
            {"Head",new List<string> {"Circlet", "Face", "Blinder", "Hat", "Turban", "Headband", "Beret"}},
            {"Body",new List<string> {"Mail", "Cuirass", "Cloak", "Corselet", "Robe", "Surcoat", "Jacket", "Coat"}},
            {"Hands",new List<string> {"Armlet","Gauntlets", "Gloves", "Armguards", "Halfgloves", "Halfgloves"}},
            {"Legs",new List<string> {"Bottom","Hose", "Breeches", "Trousers", "Longkilt", "Poleyns"}},
            {"Feet",new List<string> {"Shoe","Sollerets", "Sabatons", "Longboots", "Sandals", "Boots"}},
            {"Earrings",new List<string> {"Earring"}},
            {"Necklace",new List<string> {"Necklace", "Choker"}},
            {"Bracelets",new List<string> {"Bracelet", "Wristband"}},
            {"Ring",new List<string> {"Ring"}}
        };

        private static readonly Dictionary<GearCategory,List<string>> GEAR_CATEGORY_NAME = new Dictionary<GearCategory,List<string>> 
        {
            {GearCategory.Fending,new List<string> {"Fending"}},
            {GearCategory.Maiming,new List<string> {"Maiming"}},
            {GearCategory.Striking,new List<string> {"Striking"}},
            {GearCategory.Scouting,new List<string> {"Scouting"}},
            {GearCategory.Aiming,new List<string> {"Aiming"}},
            {GearCategory.Casting,new List<string> {"Casting"}},
            {GearCategory.Healing,new List<string> {"Healing"}},
            {GearCategory.Slaying,new List<string> {"Slaying"}}
        }; // Leaving as list for possible changes in the future where its not only 1 work.

        private static readonly Dictionary<Job, string> JOB_TO_ACCRONYM_MAP = new Dictionary<Job, string>
        {
            {Job.Warrior,"WAR"},
            {Job.Gunbreaker,"GNB"},
            {Job.DarkKnight,"DRK"},
            {Job.Paladin,"PLD"},
            {Job.WhiteMage,"WHM"},
            {Job.Scholar,"SCH"},
            {Job.Astrologian,"AST"},
            {Job.Sage,"SGE"},
            {Job.BlackMage,"BLM"},
            {Job.RedMage,"RDM"},
            {Job.Summoner,"SMN"},
            {Job.Pictomancer,"PIC"}, // TODO CHECK THAT
            {Job.Samurai,"SAM"},
            {Job.Monk,"MNK"},
            {Job.Reaper,"RPR"},
            {Job.Dragoon,"DRG"},
            {Job.Ninja,"NIN"},
            {Job.Viper,"VIP"}, // CHECK THAT
            {Job.Bard, "BRD"},
            {Job.Dancer, "DNC"},
            {Job.Machinist, "MCH"}
        };

        public static readonly Dictionary<Job, List<GearCategory>> JOB_TO_GEAR_CATEGORY_MAP = new Dictionary<Job, List<GearCategory>>()
        {
            {Job.Gunbreaker, new List<GearCategory> {GearCategory.Fending, GearCategory.Fending}},
            {Job.DarkKnight, new List<GearCategory> {GearCategory.Fending, GearCategory.Fending}},
            {Job.Warrior, new List<GearCategory> {GearCategory.Fending, GearCategory.Fending}},
            {Job.Paladin, new List<GearCategory> {GearCategory.Fending, GearCategory.Fending}},

            {Job.WhiteMage, new List<GearCategory> {GearCategory.Healing, GearCategory.Healing}},
            {Job.Scholar, new List<GearCategory> {GearCategory.Healing, GearCategory.Healing}},
            {Job.Sage, new List<GearCategory> {GearCategory.Healing, GearCategory.Healing}},
            {Job.Astrologian, new List<GearCategory> {GearCategory.Healing, GearCategory.Healing}},

            {Job.Machinist, new List<GearCategory> {GearCategory.Aiming, GearCategory.Aiming}},
            {Job.Bard, new List<GearCategory> {GearCategory.Aiming, GearCategory.Aiming}},
            {Job.Dancer, new List<GearCategory> {GearCategory.Aiming, GearCategory.Aiming}},

            {Job.BlackMage, new List<GearCategory> {GearCategory.Casting, GearCategory.Casting}},
            {Job.RedMage, new List<GearCategory> {GearCategory.Casting, GearCategory.Casting}},
            {Job.Summoner, new List<GearCategory> {GearCategory.Casting, GearCategory.Casting}},
            {Job.Pictomancer, new List<GearCategory> {GearCategory.Casting, GearCategory.Casting}},  // TODO CHECK THIS

            {Job.Samurai, new List<GearCategory> {GearCategory.Striking, GearCategory.Slaying}},
            {Job.Monk, new List<GearCategory> {GearCategory.Striking, GearCategory.Slaying}},
            {Job.Ninja, new List<GearCategory> {GearCategory.Scouting, GearCategory.Aiming}},
            {Job.Dragoon, new List<GearCategory> {GearCategory.Maiming, GearCategory.Slaying}},
            {Job.Reaper, new List<GearCategory> {GearCategory.Maiming, GearCategory.Slaying}},
            {Job.Viper, new List<GearCategory> {GearCategory.Scouting, GearCategory.Aiming}} // TODO CHECK THIS
        };


        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GearLevel { get; set; }
        public GearStage GearStage { get; set; }
        public GearType GearType { get; set; }
        public GearCategory GearCategory { get; set; }
        public Job GearWeaponCategory {get; set;} = Job.Empty;
        public string IconPath {get; set;} = string.Empty;
        public int EtroGearId {get;set;}

        // Gear object functions

        public static Gear CreateGearFromEtro(string ItemLevel, string name, bool IsWeapon, string JobName, string IconPath, int EtroId)
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
            bool IsRing;
            foreach (KeyValuePair<string, List<string>> pair in GEAR_TYPE_NAME)
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
                            type=GearType.LeftRing; // The function calling this one will check if its a LeftRing. If it is it also creates a RightRing
                        break;
                    }

                }
                if (FoundMatch) break;
            }
            GearCategory category = GearCategory.Weapon;
            FoundMatch = false;
            foreach (KeyValuePair<GearCategory, List<string>> pair in GEAR_CATEGORY_NAME)
            {   

                foreach (string PossibleName in pair.Value)
                {
                    if (name.IndexOf(PossibleName, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        FoundMatch=true;
                        category = pair.Key;
                        break;
                    }

                }
                if (FoundMatch) break;
            }

            Job WeaponCategory = Job.Empty;
            if (IsWeapon)
            {
                foreach (KeyValuePair<Job, string> pair in JOB_TO_ACCRONYM_MAP)
                {   
                    if (JobName.IndexOf(pair.Value, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        FoundMatch=true;
                        WeaponCategory = pair.Key;
                        break;
                    }
                }
            }
            return new Gear() {
                Name=name,
                GearLevel=int.Parse(ItemLevel),
                GearType=type,
                GearStage=stage,
                GearCategory=category,
                GearWeaponCategory=WeaponCategory,
                IconPath=IconPath,
                EtroGearId=EtroId
            };

        }   


        public CostDTO GetCost(Gear BisGear){
            // Returns the Cost required to get from the called upon gear object to the BisGear.

            int TotalTomeCost = 0;
            int TotalTwineCost = 0;
            int TotalShineCost = 0;
            int TotalSolventCost = 0;
            int TotalHermeticCost = 0;

            Console.WriteLine("Getting cost");
            Console.WriteLine(Name);
            Console.WriteLine(BisGear.Name);
            if (GearStage == BisGear.GearStage) // Equal so no cost
                return new CostDTO {TomeCost=TotalTomeCost, TwineCost=TotalTwineCost, ShineCost=TotalShineCost,SolventCost=TotalSolventCost,WeaponTomestoneCost=TotalHermeticCost};

            Console.WriteLine("1");

            if ((GearStage == GearStage.Raid || GearStage == GearStage.Preparation || GearStage == 0) && // Need to buy tome
                (BisGear.GearStage == GearStage.Upgraded_Tomes || BisGear.GearStage == GearStage.Tomes))
                {
                    Console.WriteLine("2");
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

            Console.WriteLine("3");
            // Need to augment (and maybe buy)
            if ((GearStage == GearStage.Raid || GearStage == GearStage.Preparation || GearStage == GearStage.Tomes || GearStage == 0) &&
                (BisGear.GearStage == GearStage.Upgraded_Tomes))
                {
                    Console.WriteLine("4");
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
                        Console.WriteLine("5");
                            TotalTwineCost += 1;
                            break;
                        case GearType.Earrings:
                        case GearType.Necklace:
                        case GearType.Bracelets:
                        case GearType.LeftRing:
                        case GearType.RightRing:
                            Console.WriteLine("6");
                            TotalShineCost += 1;
                            break;
                    }
                }
            Console.WriteLine("7");
            return new CostDTO {TomeCost=TotalTomeCost, TwineCost=TotalTwineCost, ShineCost=TotalShineCost,SolventCost=TotalSolventCost,WeaponTomestoneCost=TotalHermeticCost};

        }

        public static GearOptionsDTO GetGearOptions(GearType GearType, Job Job, DataContext context)
        {   /*Returns a GearOptionsDTO which is a list of GearOption. Each gear options
              has the gear name, the gear ilevel and the gear stage (raid/augmented/crafted/tome)
              Job -> Job to request the gear for 
              GearType -> What gear piece to request (ie. Ring, Weapon, etc.)
            */
            List<GearOptionsDTO.GearOption> OptionList = new List<GearOptionsDTO.GearOption>()
            {
                new GearOptionsDTO.GearOption()
                {
                    GearName = "No Equipment",
                    GearItemLevel = 0,
                    GearStage = "Preparation",
                    GearId = 0
                }
            };
            if (GearType == GearType.Weapon)
            {
                IEnumerable<Gear> GearIterFromDb = context.Gears.Where(g => g.GearWeaponCategory == Job && g.GearCategory == GearCategory.Weapon);
                foreach (Gear gear in GearIterFromDb)
                {
                    OptionList.Add(new GearOptionsDTO.GearOption()
                    {
                        GearName = gear.Name,
                        GearItemLevel = gear.GearLevel,
                        GearStage = gear.GearStage.ToString(),
                        GearId = gear.Id
                    });
                }
            }
            else
            {
                GearCategory GearToChooseFrom = Gear.JOB_TO_GEAR_CATEGORY_MAP[Job][(int) GearType >=7 ? 1 : 0];
                // Left side is index 0 right side is index 1
                IEnumerable<Gear> GearIterFromDb = context.Gears.Where(g => g.GearCategory == GearToChooseFrom && g.GearType == GearType);
                foreach (Gear gear in GearIterFromDb)
                {
                    OptionList.Add(new GearOptionsDTO.GearOption()
                    {
                        GearName = gear.Name,
                        GearItemLevel = gear.GearLevel,
                        GearStage = gear.GearStage.ToString(),
                        GearId = gear.Id
                    });
                }

            }
            return new GearOptionsDTO() 
            {
                GearOptionList=OptionList
            };
        }
    }

    public enum GearCategory
    {
        Fending = 1,
        Maiming = 2,
        Striking = 3,
        Scouting = 4,
        Aiming = 5,
        Casting = 6,
        Healing = 7,
        Slaying = 8,
        Weapon = 9
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
