using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.DTO;
using Microsoft.AspNetCore.Mvc;

namespace FFXIV_RaidLootAPI.Models
{

public enum Tier
{
    None = -1,
    SEVEN_ZERO = 0,
    SEVEN_TWO,
    SEVEN_4,
    EIGhT_ZERO,
    EIGHT_TWO,
    HEIGHT_FOUR
}

    public class Static
    {
        /// <summary>
        /// Id of the static. Unique identifier used to identify the static.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// UUID of the static. What is in URL of static.
        /// </summary>
        public string UUID { get; set; } = "";

        /// <summary>
        /// Name of the static.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Tier of the static.
        /// </summary>
        public Tier Tier {get;set;} = Tier.SEVEN_ZERO;





        public decimal GearScoreA {get;set;} = 0.0m;
        public decimal GearScoreB {get;set;} = 0.0m;
        public decimal GearScoreC {get;set;} = 0.0m;
        public string ownerIdString {get;set;} = string.Empty;
        public string LOCK_PARAM {get;set;} = "FALSE;FALSE;1;TRUE;1;FALSE;FALSE;1;1;1"; // Default values.
        /*
        - BOOL_LOCK_PLAYERS; (FALSE)
        - BOOL_LOCK_IF_NOT_CONTESTED; (TRUE)
        - RESET_TIME_IN_WEEK; (1)
        - BOOL_FOR_1_FIGHT; (TRUE)
        - INT_NUMBER_OF_PIECES_UNTIL_LOCK; (1)
        - LOCK_IF_TOME_AUGMENT; (FALSE)
        - BOOL_IF_ROLE_CHANGES_NUMBER_PIECES; (FALSE)
        - DPS_NUMBER;TANK_NUMBER;HEALER_NUMBER (1),(1),(1)
        */


        public Dictionary<DateOnly, List<GearAcquisitionDTO.GearAcqInfo>> GetAllTimestampOfStatic(DateOnly minDate, DataContext context){
            return GearAcquisitionTimestamp.GetAllTimestampOfStatic(Id,minDate, context);
        }
        public List<decimal> GetGearScoreParameter(){return new List<decimal>{GearScoreA, GearScoreB, GearScoreC};}
        public List<decimal> ComputeNumberRaidBuffsAndGroupAvgLevel(DataContext context){
            var playerList = context.Players.Where(p => p.staticId == Id && p.IsAlt == false).ToList(); // Only takes non alt
            decimal IlevelSum = 0.0m;
            decimal NumberRaidBuffs = 0.0m;
            foreach (Players player in playerList){
                IlevelSum += player.get_avg_item_level(context:context);
                switch (player.Job){
                    case Job.Astrologian:
                        NumberRaidBuffs+=1.0m;
                            break;
                    case Job.Scholar:
                        NumberRaidBuffs+=1.0m;
                            break;
                    case Job.Dancer:
                        NumberRaidBuffs+=1.5m;
                            break;
                    case Job.RedMage:
                        NumberRaidBuffs+=1m;
                            break;
                    case Job.Bard:
                        NumberRaidBuffs+=1.5m;
                            break;
                    case Job.Ninja:
                        NumberRaidBuffs+=1m;
                            break;
                    case Job.Reaper:
                        NumberRaidBuffs+=1m;
                            break;
                    case Job.Dragoon:
                        NumberRaidBuffs+=1m;
                            break;
                    case Job.Summoner:
                        NumberRaidBuffs+=1m;
                            break;
                    case Job.Monk:
                        NumberRaidBuffs+=1m;
                        break;
                }
            }
            decimal TeamAverageItemLevel = IlevelSum/8.0m;
            return new List<decimal> {NumberRaidBuffs, TeamAverageItemLevel};
        }
        public List<Tuple<int, decimal>> ComputePlayerGearScore(DataContext context){
            List<decimal> ScoreParam = GetGearScoreParameter();
            List<decimal> info = ComputeNumberRaidBuffsAndGroupAvgLevel(context);
            decimal GroupAvgLevel = info[1];
            decimal NumberRaidBuffs = info[0];

            List<Tuple<int, decimal>> PlayerGearScoreList = new List<Tuple<int, decimal>>();
            var playerList = context.Players.Where(p => p.staticId == Id).ToList();

            foreach (Players player in playerList){
                decimal PlayerGearScore = player.ComputePlayerGearScore(ScoreParam[0], ScoreParam[1], ScoreParam[2], GroupAvgLevel, NumberRaidBuffs, context);
                PlayerGearScoreList.Add(new Tuple<int, decimal>(player.Id, PlayerGearScore));
            }
            //Console.WriteLine(string.Join(", ", PlayerGearScoreList.Select(x => $"Player ID: {x.Item1}, Gear Score: {x.Item2}")));
            return PlayerGearScoreList;
            
        }
        public Dictionary<string, int> GetLockParam(){
            List<string> paramList = LOCK_PARAM.Split(";").ToList();
            return new Dictionary<string, int>(){
                {"BOOL_LOCK_PLAYERS" , paramList[0] == "TRUE" ? 1 : 0},
                {"BOOL_LOCK_IF_NOT_CONTESTED" , paramList[1] == "TRUE" ? 1 : 0},
                {"RESET_TIME_IN_WEEK" , int.Parse(paramList[2])},
                {"BOOL_FOR_1_FIGHT" , paramList[3] == "TRUE" ? 1 : 0},
                {"INT_NUMBER_OF_PIECES_UNTIL_LOCK" , int.Parse(paramList[4])},
                {"LOCK_IF_TOME_AUGMENT" , paramList[5] == "TRUE" ? 1 : 0},
                {"BOOL_IF_ROLE_CHANGES_NUMBER_PIECES" , paramList[6] == "TRUE" ? 1 : 0},
                {"DPS_NUMBER" , int.Parse(paramList[7])},
                {"TANK_NUMBER" , int.Parse(paramList[8])},
                {"HEALER_NUMBER" , int.Parse(paramList[9])},
            };
        }

        public static string DictParamToString(Dictionary<string, int> LockParam){
            string res = "";
            res += (LockParam["BOOL_LOCK_PLAYERS"] == 1 ? "TRUE" : "FALSE") + ";";
            res += (LockParam["BOOL_LOCK_IF_NOT_CONTESTED"] == 1 ? "TRUE" : "FALSE") + ";";
            res += LockParam["RESET_TIME_IN_WEEK"].ToString() + ";";
            res += (LockParam["BOOL_FOR_1_FIGHT"] == 1 ? "TRUE" : "FALSE") + ";";
            res += LockParam["INT_NUMBER_OF_PIECES_UNTIL_LOCK"].ToString() + ";";
            res += (LockParam["LOCK_IF_TOME_AUGMENT"] == 1 ? "TRUE" : "FALSE") + ";";
            res += (LockParam["BOOL_IF_ROLE_CHANGES_NUMBER_PIECES"] == 1 ? "TRUE" : "FALSE") + ";";
            res += LockParam["DPS_NUMBER"].ToString() + ";";
            res += LockParam["TANK_NUMBER"].ToString() + ";";
            res += LockParam["HEALER_NUMBER"].ToString();
            return res;
        }

        public List<Players> GetPlayers(DataContext context){
            return context.Players.Where(p => p.staticId == Id).ToList();
        }

        public void UpdateLockParam(Dictionary<string, int> NewLockParam){
            LOCK_PARAM = DictParamToString(NewLockParam);
        }

    }
}
