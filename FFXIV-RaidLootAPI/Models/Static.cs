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

        public List<Decimal> GetGearScoreParameter(){return new List<decimal>{GearScoreA, GearScoreB, GearScoreC};}

        //[HttpPost("SetScoreParam")]

    }
}
