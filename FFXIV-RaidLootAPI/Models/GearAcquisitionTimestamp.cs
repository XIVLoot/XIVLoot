
using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.DTO;

namespace FFXIV_RaidLootAPI.Models
{
    public class GearAcquisitionTimestamp
    {
        // This records when a player gets a gear.
        public int Id {get;set;}
        public DateOnly Timestamp {get;set;}
        public int GearId {get;set;}
        public int PlayerId {get;set;}


        public static Dictionary<GearType, Gear?> ComputeGearAtTimestamp(int PlayerId, DateOnly Timestamp, DataContext context){
            List<GearAcquisitionTimestamp> listValid = context.GearAcquisitionTimestamps.Where(p => p.PlayerId == PlayerId && p.Timestamp <= Timestamp).
            OrderByDescending(p => p.Timestamp).
            ToList();

            Dictionary<GearType, Gear?> response = new Dictionary<GearType, Gear?>()
            {
                {GearType.Weapon , null},
                {GearType.Head , null},
                {GearType.Body , null},
                {GearType.Hands , null},
                {GearType.Legs , null},
                {GearType.Feet , null},
                {GearType.Earrings , null},
                {GearType.Necklace , null},
                {GearType.Bracelets , null},
                {GearType.LeftRing , null},
                {GearType.RightRing , null}
            };

            foreach (GearAcquisitionTimestamp p in listValid){
                Gear? trialGear = context.Gears.FirstOrDefault(q => q.Id == p.GearId);
                if (trialGear is null)
                    continue;
                
                if (!(response[trialGear.GearType] is null)){
                    response[trialGear.GearType] = trialGear;
                }
            }
            return response;
        }

        public static Dictionary<DateOnly, List<GearAcquisitionDTO.GearAcqInfo>> GetAllTimestampOfStatic(int staticId, DataContext context){

            List<Players> players = context.Players.Where(p => p.staticId == staticId).ToList();
            
            Dictionary<DateOnly, List<GearAcquisitionDTO.GearAcqInfo>> response = new Dictionary<DateOnly, List<GearAcquisitionDTO.GearAcqInfo>>();

            foreach (Players player in players){
                List<GearAcquisitionTimestamp> list = context.GearAcquisitionTimestamps.Where(p => p.PlayerId == player.Id).ToList();

                foreach (GearAcquisitionTimestamp p in list){
                    Gear? gear = context.Gears.FirstOrDefault(g => g.Id == p.GearId);
                    if (gear is null)
                        continue;
                    
                    if (response.ContainsKey(p.Timestamp)){
                        response[p.Timestamp].Add(new GearAcquisitionDTO.GearAcqInfo(){
                            GearType = gear.GearType.ToString(),
                            PlayerId = p.PlayerId,
                            IsAugment = gear.GearStage == GearStage.Upgraded_Tomes
                        });
                    }
                    else{
                        response[p.Timestamp] = new List<GearAcquisitionDTO.GearAcqInfo>() { 
                            new GearAcquisitionDTO.GearAcqInfo(){
                            GearType = gear.GearType.ToString(),
                            PlayerId = p.PlayerId,
                            IsAugment = gear.GearStage == GearStage.Upgraded_Tomes
                        }};
                    }
                }
            }
            return response;
        }

    }
}