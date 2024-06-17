using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.DTO;
using Microsoft.AspNetCore.Mvc;

namespace FFXIV_RaidLootAPI.Models
{
    public class Static
    {
        public int Id { get; set; }
        
        public string UUID { get; set; } = "";

        public string Name { get; set; } = string.Empty;

        public decimal GearScoreA {get;set;} = 0.0m;
        public decimal GearScoreB {get;set;} = 0.0m;
        public decimal GearScoreC {get;set;} = 0.0m;

        public List<decimal> GetGearScoreParameter(){return new List<decimal>{GearScoreA, GearScoreB, GearScoreC};}
        public List<decimal> ComputeNumberRaidBuffsAndGroupAvgLevel(DataContext context){
            var playerList = context.Players.Where(p => p.staticId == Id).ToList();
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
            Console.WriteLine(string.Join(", ", PlayerGearScoreList.Select(x => $"Player ID: {x.Item1}, Gear Score: {x.Item2}")));
            return PlayerGearScoreList;
            
        }

        //[HttpPost("SetScoreParam")]

    }
}
