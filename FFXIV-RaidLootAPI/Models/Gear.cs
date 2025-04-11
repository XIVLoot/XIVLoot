using System.Text.RegularExpressions;
using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.DTO;

namespace FFXIV_RaidLootAPI.Models
{
    public class Gear
    {
        public static readonly List<Tuple<Tier, int>> LIST_TIER_AND_I_LEVEL = new List<Tuple<Tier, int>>()
        {
            new Tuple<Tier, int>(Tier.SEVEN_ZERO, 700),
            new Tuple<Tier, int>(Tier.SEVEN_TWO, 740),
            new Tuple<Tier, int>(Tier.SEVEN_TWO, 770),
        };
        public static readonly int ACCESSORY_TOME_COST = 375;
        public static readonly int ARMOR_LOW_COST = 495; // Head, Hands, feet
        public static readonly int ARMOR_HIGH_COST = 825; // Body, Legs
        public static readonly int WEAPON_TOME_COST = 500; // Body, Legs
        private static readonly Dictionary<Tier, string> TOME_GEAR = new Dictionary<Tier, string>()
        {
            {Tier.SEVEN_ZERO, "Quetzalli"},
            {Tier.SEVEN_TWO, "Historia"}
        };
        private static readonly Dictionary<Tier, string> RAID_GEAR = 
        new Dictionary<Tier, string>()
        {
            {Tier.SEVEN_ZERO, "Dark Horse"},
            {Tier.SEVEN_TWO, "Babyface"}
        };
        private static readonly Dictionary<Tier, string> AUGMENT_TOME = 
        new Dictionary<Tier, string>()
        {
            {Tier.SEVEN_ZERO, "Augment"},
            {Tier.SEVEN_TWO, "Augment"}
        };

        private static readonly Dictionary<Tier, string> CRAFTED_GEAR = 
        new Dictionary<Tier, string>()
        {
            {Tier.SEVEN_ZERO, "Archeo"},
            {Tier.SEVEN_TWO, "Ceremonial"}
        };
        private static readonly Dictionary<Tier, string> NORMAL_RAID = 
        new Dictionary<Tier, string>()
        {
            {Tier.SEVEN_ZERO, "Light-heavy"},
            {Tier.SEVEN_TWO, "Cruiser"}
        };
        private static readonly Dictionary<Tier, string> EX_TRIAL = 
        new Dictionary<Tier, string>()
        {
            {Tier.SEVEN_ZERO, "Resilient"},
            {Tier.SEVEN_TWO, "NO_NAME"}
        };

        private static readonly Dictionary<Tier, string> EARLY_TOME = 
        new Dictionary<Tier, string>()
        {
            {Tier.SEVEN_ZERO, "Neo Kingdom"},
            {Tier.SEVEN_TWO, "NO_NAME"}
        };

        private static readonly Dictionary<Tier, string> EX_WEAPON = 
        new Dictionary<Tier, string>()
        {
            {Tier.SEVEN_ZERO, "Skyruin"},
            {Tier.SEVEN_TWO, "Queensknight"}
        }; 


        private static readonly Dictionary<string,List<string>> GEAR_TYPE_NAME = new Dictionary<string,List<string>> 
        {
            {"Head",new List<string> {"Circlet", "Face", "Blinder", "Hat", "Turban", "Headband", "Beret","Hood", "Chapeau", "Bandana", ""}},
            {"Body",new List<string> {"Mail", "Cuirass", "Cloak", "Corselet", "Robe", "Surcoat", "Jacket", "Coat", "Tunic", "Tabard"}},
            {"Hands",new List<string> {"Armlet","Gauntlets", "Gloves", "Armguards", "Halfgloves", "Halfgloves"}},
            {"Legs",new List<string> {"Bottom","Hose", "Breeches", "Trousers", "Longkilt", "Poleyns", "Brayettes", "Kecks", "Brais"}},
            {"Feet",new List<string> {"Shoe","Sollerets", "Sabatons", "Longboots", "Sandals", "Boots", "Thighboots"}},
            {"Earrings",new List<string> {"Earring"}},
            {"Necklace",new List<string> {"Necklace", "Choker"}},
            {"Bracelets",new List<string> {"Bracelet", "Wristband", "Bangle"}},
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
            {Job.Pictomancer,"PCT"}, 
            {Job.Samurai,"SAM"},
            {Job.Monk,"MNK"},
            {Job.Reaper,"RPR"},
            {Job.Dragoon,"DRG"},
            {Job.Ninja,"NIN"},
            {Job.Viper,"VPR"},
            {Job.Bard, "BRD"},
            {Job.Dancer, "DNC"},
            {Job.Machinist, "MCH"}
        };

        public static readonly int MIN_LEVEL = 690; // Min item level of gear to display to user choice.

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
        public Tier Tier {get;set;}
        //public int XivApiGearId {get;set;}
        
        // XIVGear and Etro both share Ids matching with XIVAPI. To facilitate we will simply use the field EtroGearId which represents the ID available on XIVAPI.

        // Gear object functions

        public static Gear CreateGearFromInfo(string ItemLevel, string name, bool IsWeapon, string JobName, string IconPath, int Externalid, GearType gearType)
        {   
            // First figure out the tier of the gear.
            int.TryParse(ItemLevel, out int iLevel);
            Tuple<Tier, int> tierofGear = LIST_TIER_AND_I_LEVEL.FirstOrDefault(t => t.Item2 >= iLevel) ?? LIST_TIER_AND_I_LEVEL.First();
            Tier gearTier = Tier.SEVEN_TWO;
            if (tierofGear != null)
            {
                gearTier = tierofGear.Item1;
            }

            GearStage stage = GearStage.Preparation; // If nothing it will be preperation, ie : crafted.
            if (name.IndexOf(TOME_GEAR[gearTier], StringComparison.OrdinalIgnoreCase) >= 0)
                stage = GearStage.Tomes;
             // Not doing else if here so it will catch the upgrade if it is but won't if its only tome.
            if (name.IndexOf(AUGMENT_TOME[gearTier], StringComparison.OrdinalIgnoreCase) >= 0)
                stage = GearStage.Upgraded_Tomes;
            else if (name.IndexOf(RAID_GEAR[gearTier], StringComparison.OrdinalIgnoreCase) >= 0)
                stage = GearStage.Raid;
            else if (name.IndexOf(EX_TRIAL[gearTier], StringComparison.OrdinalIgnoreCase) >= 0)
                stage = GearStage.Extreme;
            else if (name.IndexOf(NORMAL_RAID[gearTier], StringComparison.OrdinalIgnoreCase) >= 0)
                stage = GearStage.Raid_Normal;
            else if (name.IndexOf(EARLY_TOME[gearTier], StringComparison.OrdinalIgnoreCase) >= 0)
                stage = GearStage.Tome_Early;
            else if (name.IndexOf(EX_WEAPON[gearTier], StringComparison.OrdinalIgnoreCase) >= 0)
                stage = GearStage.Extreme;
            else if (name.IndexOf(CRAFTED_GEAR[gearTier], StringComparison.OrdinalIgnoreCase) >= 0)
                stage = GearStage.Preparation;

            GearType type = gearType;
            bool FoundMatch = true;
            /*
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
            if (!FoundMatch && !IsWeapon)
                Console.WriteLine("Gear type not found for : " + name);
            */
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
                        Console.WriteLine("Found match for weapon : " + name + " : " + JobName + " : " + WeaponCategory.ToString() + " : " + pair.Value);
                        break;
                    }
                }
            }
            if (!FoundMatch)
                Console.WriteLine("Did not found match for weapon : " + name + " : " + JobName);
            return new Gear() {
                Name=name,
                GearLevel=int.Parse(ItemLevel),
                GearType=type,
                GearStage=stage,
                GearCategory=category,
                GearWeaponCategory=WeaponCategory,
                IconPath=IconPath,
                EtroGearId=Externalid,
                Tier=gearTier
            };

        }   


        public CostDTO GetCost(Gear BisGear){
            // Returns the Cost required to get from the called upon gear object to the BisGear.

            int TotalTomeCost = 0;
            int TotalTwineCost = 0;
            int TotalShineCost = 0;
            int TotalSolventCost = 0;
            int TotalHermeticCost = 0;

            ////Console.WriteLine("Getting cost");
            ////Console.WriteLine(Name);
            ////Console.WriteLine(BisGear.Name);
            if (GearStage == BisGear.GearStage) // Equal so no cost
                return new CostDTO {TomeCost=TotalTomeCost, TwineCost=TotalTwineCost, ShineCost=TotalShineCost,SolventCost=TotalSolventCost,WeaponTomestoneCost=TotalHermeticCost};

            ////Console.WriteLine("1");
            //Console.WriteLine("Current gearstage : " + GearStage.ToString());
            //Console.WriteLine("Current bisgearstage : " + BisGear.GearStage.ToString());

            if ((GearStage == GearStage.Raid || GearStage == GearStage.Preparation || GearStage == 0 || GearStage == GearStage.Extreme || 
                 GearStage == GearStage.Artifact || GearStage == GearStage.Raid_Normal || GearStage == GearStage.Tome_Early) && // Need to buy tome
                (BisGear.GearStage == GearStage.Upgraded_Tomes || BisGear.GearStage == GearStage.Tomes))
                {
                    ////Console.WriteLine("2");
                    
                    switch (BisGear.GearType)
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

            ////Console.WriteLine("3");
            // Need to augment (and maybe buy)
            if ((GearStage == GearStage.Raid || GearStage == GearStage.Preparation || GearStage == GearStage.Tomes || GearStage == 0 || GearStage == GearStage.Extreme || 
                 GearStage == GearStage.Artifact || GearStage == GearStage.Raid_Normal || GearStage == GearStage.Tome_Early) &&
                (BisGear.GearStage == GearStage.Upgraded_Tomes))
                {
                    ////Console.WriteLine("4");
                    switch (BisGear.GearType)
                    {
                        case GearType.Weapon:
                            TotalSolventCost+=1;
                            break;
                        case GearType.Head:
                        case GearType.Body:
                        case GearType.Hands:
                        case GearType.Legs:
                        case GearType.Feet:
                        ////Console.WriteLine("5");
                            TotalTwineCost += 1;
                            break;
                        case GearType.Earrings:
                        case GearType.Necklace:
                        case GearType.Bracelets:
                        case GearType.LeftRing:
                        case GearType.RightRing:
                            ////Console.WriteLine("6");
                            TotalShineCost += 1;
                            break;
                    }
                }
            ////Console.WriteLine("7");
            return new CostDTO {TomeCost=TotalTomeCost, TwineCost=TotalTwineCost, ShineCost=TotalShineCost,SolventCost=TotalSolventCost,WeaponTomestoneCost=TotalHermeticCost};

        }

        public static GearOptionsDTO GetGearOptions(GearType GearType, Job Job, DataContext context, Tier Tier)
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
                List<Gear> GearIterFromDb = context.Gears.Where(g => g.GearWeaponCategory == Job && g.GearCategory == GearCategory.Weapon && (g.Tier == Tier || g.Tier == Tier.None)).OrderBy(g => g.GearLevel).ToList();
                foreach (Gear gear in GearIterFromDb)
                {
                    if (gear.GearLevel >= MIN_LEVEL)
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
                List<Gear> GearIterFromDb = context.Gears.Where(g => g.GearCategory == GearToChooseFrom && g.GearType == GearType && (g.Tier == Tier || g.Tier == Tier.None)).OrderBy(g => g.GearLevel).ToList();
                foreach (Gear gear in GearIterFromDb)
                {   
                    if (gear.GearLevel >= MIN_LEVEL)
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
        Raid = 4,
        Extreme = 5,
        Raid_Normal = 6,
        Tome_Early = 7,
        Artifact = 8,
        Alliance_Raid = 9
    }

    public enum GearType
    {
        Empty = 0,
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
