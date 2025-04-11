using FFXIV_RaidLootAPI.Models;

namespace FFXIV_RaidLootAPI.DTO;

public class ItemBreakdownDTO
{

    public class PlayerInfoItemBreakdown
    {
        public string Name {get;set;} = string.Empty;
        public bool NeedThisGearType {get;set;}
        public int playerId {get;set;}
        public int needAmount {get;set;}=1;

    }

    public bool PlayerAlreadyNeed(int playerId, Turn turn, string type)
    {

        if (!ItemBreakdown[Enum.GetName(typeof(Turn), turn)!].ContainsKey(type))
        {
            return false;
        }

        foreach (PlayerInfoItemBreakdown info in ItemBreakdown[Enum.GetName(typeof(Turn), turn)!][type])
        {
            if (info.playerId == playerId)
            {
                info.needAmount++;
                return true;
            }
        }
        return false;
    }

    public Dictionary<string, Dictionary<string, List<PlayerInfoItemBreakdown>>> ItemBreakdown {get;set;} = new Dictionary<string, Dictionary<string, List<PlayerInfoItemBreakdown>>>()
    {
        {Enum.GetName(typeof(Turn), Turn.turn_1)!, new Dictionary<string, List<PlayerInfoItemBreakdown>>
        {
            {Enum.GetName(typeof(GearType), GearType.Earrings)!, new List<PlayerInfoItemBreakdown>()},
            {Enum.GetName(typeof(GearType), GearType.Necklace)!, new List<PlayerInfoItemBreakdown>()},
            {"Ring", new List<PlayerInfoItemBreakdown>()},
            {Enum.GetName(typeof(GearType), GearType.Bracelets)!, new List<PlayerInfoItemBreakdown>()},
        }
        },
        {Enum.GetName(typeof(Turn), Turn.turn_2)!, new Dictionary<string, List<PlayerInfoItemBreakdown>>()
        {
            {Enum.GetName(typeof(GearType), GearType.Feet)!, new List<PlayerInfoItemBreakdown>()},
            {Enum.GetName(typeof(GearType), GearType.Head)!, new List<PlayerInfoItemBreakdown>()},
            {Enum.GetName(typeof(GearType), GearType.Hands)!, new List<PlayerInfoItemBreakdown>()},
            {"Shine", new List<PlayerInfoItemBreakdown>()},
        }},
        {Enum.GetName(typeof(Turn), Turn.turn_3)!, new Dictionary<string, List<PlayerInfoItemBreakdown>>()
        {
            {Enum.GetName(typeof(GearType), GearType.Legs)!, new List<PlayerInfoItemBreakdown>()},
            {Enum.GetName(typeof(GearType), GearType.Body)!, new List<PlayerInfoItemBreakdown>()},
            //{Enum.GetName(typeof(GearType), GearType.Feet)!, new List<PlayerInfoItemBreakdown>()},
            //{Enum.GetName(typeof(GearType), GearType.Head)!, new List<PlayerInfoItemBreakdown>()},
            //{Enum.GetName(typeof(GearType), GearType.Hands)!, new List<PlayerInfoItemBreakdown>()},
            {"Twine", new List<PlayerInfoItemBreakdown>()},
        }},
        {Enum.GetName(typeof(Turn), Turn.turn_4)!, new Dictionary<string, List<PlayerInfoItemBreakdown>>()
        {
            {Enum.GetName(typeof(GearType), GearType.Weapon)!, new List<PlayerInfoItemBreakdown>()},
        }},
    };
}