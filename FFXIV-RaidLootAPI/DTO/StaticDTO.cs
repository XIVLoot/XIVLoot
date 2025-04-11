using System.ComponentModel.DataAnnotations;
using FFXIV_RaidLootAPI.Models;

namespace FFXIV_RaidLootAPI.DTO;

public class StaticDTO
{

    public class PlayerInfoDTO
    {
        public int Id { get; set; }
        public bool IsClaimed {get;set;}
        public string Name { get; set; } = "No name";
        public string Job {get; set; } = string.Empty;
        public string EtroBiS {get;set;} = string.Empty;
        public bool Locked { get; set; }
        public List<DateTime> LockedList {get;set;} = new List<DateTime>();
        public Dictionary<string,GearOptionsDTO.GearOption?> CurrentGearSet {get;set;} = new Dictionary<string,GearOptionsDTO.GearOption?>();
        public Dictionary<string,GearOptionsDTO.GearOption?> BisGearSet {get;set;} = new Dictionary<string,GearOptionsDTO.GearOption?>();
        public Dictionary<string, GearOptionsDTO> GearOptionPerGearType {get;set;} = new Dictionary<string, GearOptionsDTO>();
        public int AverageItemLevelCurrent {get;set;}
        public int AverageItemLevelBis {get;set;}
        public decimal PlayerGearScore {get;set;}
        public CostDTO Cost {get;set;} = new CostDTO();
        public bool IsAlt {get;set;}
    }


    public class PlayerInfoSoftDTO
    {
        public List<DateTime> LockedList {get;set;} = new List<DateTime>();
        public int AverageItemLevelCurrent {get;set;}
        public int AverageItemLevelBis {get;set;}
        public CostDTO Cost {get;set;} = new CostDTO();
        public bool IsClaimed {get;set;}
    }

    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string UUID { get; set; } = string.Empty;
    public Dictionary<string, int> LockParam { get; set; } = new Dictionary<string, int>();
    public List<PlayerInfoDTO> PlayersInfoList { get; set; } = new List<PlayerInfoDTO>();
    public Tier Tier {get;set;} = Tier.SEVEN_ZERO;
}