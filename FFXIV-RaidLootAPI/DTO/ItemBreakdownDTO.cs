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

        if (!ItemBreakdown[turn].ContainsKey(type))
        {
            return false;
        }

        foreach (PlayerInfoItemBreakdown info in ItemBreakdown[turn][type])
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
        {Turn.turn_1.ToString(), new Dictionary<string, List<PlayerInfoItemBreakdown>>
        {
            {GearType.Earrings.ToString(), new List<PlayerInfoItemBreakdown>()},
            {GearType.Necklace.ToString(), new List<PlayerInfoItemBreakdown>()},
            {"Ring", new List<PlayerInfoItemBreakdown>()},
            {GearType.Bracelets.ToString(), new List<PlayerInfoItemBreakdown>()},
        }
        },
        {Turn.turn_2.ToString(), new Dictionary<string, List<PlayerInfoItemBreakdown>>()
        {
            {GearType.Feet.ToString(), new List<PlayerInfoItemBreakdown>()},
            {GearType.Head.ToString(), new List<PlayerInfoItemBreakdown>()},
            {GearType.Hands.ToString(), new List<PlayerInfoItemBreakdown>()},
            {"Shine", new List<PlayerInfoItemBreakdown>()},
        }},
        {Turn.turn_3.ToString(), new Dictionary<string, List<PlayerInfoItemBreakdown>>()
        {
            {GearType.Legs.ToString(), new List<PlayerInfoItemBreakdown>()},
            {GearType.Body.ToString(), new List<PlayerInfoItemBreakdown>()},
            {GearType.Feet.ToString(), new List<PlayerInfoItemBreakdown>()},
            {GearType.Head.ToString(), new List<PlayerInfoItemBreakdown>()},
            {GearType.Hands.ToString(), new List<PlayerInfoItemBreakdown>()},
            {"Twine", new List<PlayerInfoItemBreakdown>()},
        }},
        {Turn.turn_4.ToString(), new Dictionary<string, List<PlayerInfoItemBreakdown>>()
        {
            {GearType.Weapon.ToString(), new List<PlayerInfoItemBreakdown>()},
        }},
    };
}