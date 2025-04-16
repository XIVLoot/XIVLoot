using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.Models;

namespace FFXIV_RaidLootAPI.DTO;

public class ItemBreakdownDTO
{

    /// <summary>
    /// Class encapsulating the information of one drop need for one player.
    /// </summary>
    public class PlayerInfoItemBreakdown
    {
        /// <summary>
        /// Name of the drop
        /// </summary>
        public string Name {get;set;} = string.Empty;

        /// <summary>
        /// If the player need this drop.
        /// </summary>
        public bool NeedThisGearType {get;set;}

        /// <summary>
        /// ID of the player
        /// </summary>
        public int playerId {get;set;}

        /// <summary>
        /// Amount of times the player need this drop.
        /// </summary>
        public int needAmount {get;set;}=1;

    }

    /// <summary>
    /// Dictionary containing all the information of the drops for all the players.
    /// The key of the dictionnary is the turn and then the geartype.
    /// So it is organised by turn and then by gear type.
    /// </summary>
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

    /// <summary>
    /// Checks if the given player already need the item from the given turn. If 
    /// it does we simply update the needAmount.
    /// </summary>
    /// <param name="playerId">ID of te player</param>
    /// <param name="turn">Turn (fight number)</param>
    /// <param name="type">Name of the drop (gear or upgrade)</param>
    /// <returns></returns>
    public bool PlayerAlreadyNeed(int playerId, Turn turn, string type)
    {
        // If the drop with the gven name at the given turn is not present
        // Then the player does not already need it.
        if (!ItemBreakdown[Enum.GetName(typeof(Turn), turn)!].ContainsKey(type))
        {
            return false;
        }

        // Get the drop info for that player for that turn and type.
        PlayerInfoItemBreakdown info = ItemBreakdown[Enum.GetName(typeof(Turn), turn)!][type].FirstOrDefault(info => info.playerId == playerId);
        if (info != null)
        {
            info.needAmount++;
            return true;
        }

        return false;
    }


    /// <summary>
    /// Orders the item breakdown of all turn and geartype to have the players 
    /// appear in that order : Non-Alt and not locked, alt and not locked, non-alt and locked, alt and locked.
    /// In the groups they will appear in the order : DPS - Tank - Healer.
    /// </summary>
    public void OrderItemBreakdown(DataContext context)
    {

        // For each turn
        foreach (KeyValuePair<string, Dictionary<string, List<PlayerInfoItemBreakdown>>> turnInfo in ItemBreakdown)
        {
            // For each drop from that turn
            foreach (KeyValuePair<string, List<PlayerInfoItemBreakdown>> infoPerDrop in turnInfo.Value)
            {
                // Corresponds to each order.
                Dictionary<Class, List<PlayerInfoItemBreakdown>> notLockedPlayer = new Dictionary<Class, List<PlayerInfoItemBreakdown>>()
                {{Class.DPS, new List<PlayerInfoItemBreakdown>()},{Class.Tank, new List<PlayerInfoItemBreakdown>()},{Class.Healer, new List<PlayerInfoItemBreakdown>()}};
                Dictionary<Class, List<PlayerInfoItemBreakdown>> notLockedAlt = new Dictionary<Class, List<PlayerInfoItemBreakdown>>()
                {{Class.DPS, new List<PlayerInfoItemBreakdown>()},{Class.Tank, new List<PlayerInfoItemBreakdown>()},{Class.Healer, new List<PlayerInfoItemBreakdown>()}};;
                Dictionary<Class, List<PlayerInfoItemBreakdown>> lockedPlayer = new Dictionary<Class, List<PlayerInfoItemBreakdown>>()
                {{Class.DPS, new List<PlayerInfoItemBreakdown>()},{Class.Tank, new List<PlayerInfoItemBreakdown>()},{Class.Healer, new List<PlayerInfoItemBreakdown>()}};;
                Dictionary<Class, List<PlayerInfoItemBreakdown>> lockedAlt = new Dictionary<Class, List<PlayerInfoItemBreakdown>>()
                {{Class.DPS, new List<PlayerInfoItemBreakdown>()},{Class.Tank, new List<PlayerInfoItemBreakdown>()},{Class.Healer, new List<PlayerInfoItemBreakdown>()}};;

                // Go through each itemInfo in the list and add them to the different categories above.
                foreach (PlayerInfoItemBreakdown info in infoPerDrop.Value)
                {
                    Players player = context.Players.FirstOrDefault(p => p.Id == info.playerId);
                    if (player != null)
                    {   
                        // If the player is not locked from that fight
                        if (player.IsPlayerLockedFromTurn(Enum.Parse<Turn>(turnInfo.Key)) == false)
                        {
                            // If the player is not an alt
                            if (player.IsAlt == false)
                            {
                                notLockedPlayer[player.GetClassOfPlayer()].Add(info);
                            }
                            // If the player is an alt
                            else
                            {
                                notLockedAlt[player.GetClassOfPlayer()].Add(info);
                            }
                        }
                        // If the player is locked from that fight
                        else
                        {
                            // If the player is not an alt
                            if (player.IsAlt == false)
                            {
                                lockedPlayer[player.GetClassOfPlayer()].Add(info);
                            }
                            // If the player is an alt
                            else
                            {
                                lockedAlt[player.GetClassOfPlayer()].Add(info);
                            }
                        }
                    }
                }

                // Recreate the itemBreakdown list based on the categories.
                List<PlayerInfoItemBreakdown> newList = new List<PlayerInfoItemBreakdown>();
                for (int i = 0;i<4;i++)
                {
                    foreach (Class bin in Enum.GetValues(typeof(Class)))
                    {
                        switch(i)
                        {
                            case 0: newList.AddRange(notLockedPlayer[bin]);break;
                            case 1: newList.AddRange(notLockedAlt[bin]);break;
                            case 2: newList.AddRange(lockedPlayer[bin]);break;
                            case 3: newList.AddRange(lockedAlt[bin]);break;
                        }
                    }
                }

                // Update the breakdown.
                ItemBreakdown[turnInfo.Key][infoPerDrop.Key] = newList;

            }
        }

    }
}