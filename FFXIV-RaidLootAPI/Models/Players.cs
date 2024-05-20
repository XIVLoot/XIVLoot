namespace FFXIV_RaidLootAPI.Models
{
    public class Players
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Role Role { get; set; }

        public bool Locked { get; set; }

        public int staticId { get; set; }

        public string EtroBiS {get; set;} = "";

        public int BisWeaponGearId { get; set; }
        public int BisHeadGearId { get; set; }
        public int BisCoatGearId { get; set; }
        public int BisHandGearId { get; set; }
        public int BisLegGearId { get; set; }
        public int BisFeetGearId { get; set; }
        public int BisEarringGearId { get; set; }
        public int BiSNecklaceGearId { get; set; }
        public int BisBraceletGearId { get; set; }
        public int BisLeftRingGearId { get; set; }
        public int BisRightRingGearId { get; set; }
        public int CurWeaponGearId { get; set; }
        public int CurHeadGearId { get; set; }
        public int CurCoatGearId { get; set; }
        public int CurHandGearId { get; set; }
        public int CurLegGearId { get; set; }
        public int CurFeetGearId { get; set; }
        public int CurEarringGearId { get; set; }
        public int CurNecklaceGearId { get; set; }
        public int CurBraceletGearId { get; set; }
        public int CurLeftRingGearId { get; set; }
        public int CurRightRingGearId { get; set; }

    }

    public enum Role
    {
        Empty = 0,
        Tank = 1,
        Healer = 2,
        DPS = 3
    }
}
