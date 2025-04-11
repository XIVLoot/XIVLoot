using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.DTO;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FFXIV_RaidLootAPI.Models
{

    public enum Turn{

        turn_0 = 0, // turn_0 is used as an ignore case.
        turn_1 = 1,
        turn_2 = 2,
        turn_3 = 3,
        turn_4 = 4,
        turn_5 = 5, // turn_5 is used as a lock (or unlock) in all fight case.
    }

    public class Players
    {   
        private static readonly int GEARSETSIZE = 11;
        public int Id { get; set; }
        public int GearScore {get;set;}
        /*
        GearScore is a value computed that represents a score dependant on the individual, the player's amount of fed damage in buffs
        and a score relative to the willingness to share gear unoptimally.
        
        The score is computed with this function (It contains one constant which is the max item level obtainable):

        decimal score = a * 10 * JobScoreMultiplier[Job] * (PlayerILevel/GroupAvgLevel) + b * 100 * (GroupAvgLevel-PlayerILevel)/(GroupAvgLevel-660) +  
                        c * NRaidBuff * JobGroupMultiplier[Job];

        The first part of the function is a value associted with the solo impact of a job on the group's DPS. nDPSRatio is the ratio of max expected nDPS to the job's expected nDPS (taken on fflogs).
        Its impact can be changed by changing the value of a.

        The second part is a measure of how behind in terms of main stat a player is. This value can be changed with b.

        The 3rd part is a measure of how much a player would affect the group's DPS with the number of buffs in the comp. Similar to first measure but benefits
        more jobs that have good burst in buffs. Can be changed with c.                   

        https://docs.google.com/spreadsheets/d/1g91GQ68w2kF9U2qO0Wdsmbw2pDYNqIy3JHEy7VFnuTc/edit#gid=0

        */

        public string Name { get; set; } = "Enter the name here";
        public bool IsClaimed {get;set;} = false;
        public Job Job {get; set; }

        public bool Locked { get; set; }
        public bool IsAlt {get;set;} = false;
        public DateTime Turn1LockedUntilDate {get;set;}
        public DateTime Turn2LockedUntilDate {get;set;}
        public DateTime Turn3LockedUntilDate {get;set;}
        public DateTime Turn4LockedUntilDate {get;set;}
        public int staticId { get; set; }

        public string EtroBiS {get; set;} = ""; // THe etrobis field will be used for both etro.gg and xivgear.app link. The front-end will parse the value to differentiate between both.
        public int BisWeaponGearId { get; set; }
        public int BisHeadGearId { get; set; }
        public int BisBodyGearId { get; set; }
        public int BisHandsGearId { get; set; }
        public int BisLegsGearId { get; set; }
        public int BisFeetGearId { get; set; }
        public int BisEarringsGearId { get; set; }
        public int BisNecklaceGearId { get; set; }
        public int BisBraceletsGearId { get; set; }
        public int BisLeftRingGearId { get; set; }
        public int BisRightRingGearId { get; set; }
        public int CurWeaponGearId { get; set; }
        public int CurHeadGearId { get; set; }
        public int CurBodyGearId { get; set; }
        public int CurHandsGearId { get; set; }
        public int CurLegsGearId { get; set; }
        public int CurFeetGearId { get; set; }
        public int CurEarringsGearId { get; set; }
        public int CurNecklaceGearId { get; set; }
        public int CurBraceletsGearId { get; set; }
        public int CurLeftRingGearId { get; set; }
        public int CurRightRingGearId { get; set; }


        public static Dictionary<Job, decimal> JobScoreMultiplier = new Dictionary<Job, decimal>()
        {
            {Job.BlackMage, 1.0m/0.818m},
            {Job.Samurai, 1.0m},
            {Job.Ninja, 1.0m/0.89m},
            {Job.Monk, 1.0m/0.888m},
            {Job.Reaper, 1.0m/0.877m},
            {Job.Dragoon, 1.0m/0.875m},
            {Job.Machinist, 1.0m/0.885m},
            {Job.Bard, 1.0m/0.746m},
            {Job.Dancer,1.0m/0.731m},
            {Job.RedMage, 1.0m/0.818m},
            {Job.Summoner, 1.0m/0.836m},
            {Job.DarkKnight, 1.0m/0.626m},
            {Job.Gunbreaker, 1.0m/0.626m},
            {Job.Warrior, 1.0m/0.614m},
            {Job.Paladin, 1.0m/0.6041m},
            {Job.Sage, 1.0m/0.524m},
            {Job.WhiteMage, 1.0m/0.524m},
            {Job.Scholar, 1.0m/0.510m},
            {Job.Astrologian, 1.0m/0.445m},
            {Job.Viper, 1.0m},
            {Job.Pictomancer, 1.0m/0.952m}, 

        };

        public static Dictionary<Job, decimal> JobGroupMultiplier = new Dictionary<Job, decimal>()
        {
            {Job.BlackMage, 1.6m},
            {Job.Samurai, 1.0m},
            {Job.Ninja, 1.19760479m},
            {Job.Monk, 1.338912134m},
            {Job.Reaper, 1.428571429m},
            {Job.Dragoon, 1.146953405m},
            {Job.Machinist, 2.069857697m},
            {Job.Bard, 1.619433198m},
            {Job.Dancer,1.982651797m},
            {Job.RedMage, 1.5m},
            {Job.Summoner, 1.211203634m},
            {Job.DarkKnight, 1.297648013m},
            {Job.Gunbreaker, 2.046035806m},
            {Job.Warrior, 1.767955801m},
            {Job.Paladin, 1.634320735m},
            {Job.Sage, 1.793721973m},
            {Job.WhiteMage, 1.744820065m},
            {Job.Scholar, 4.060913706m},
            {Job.Astrologian, 3.827751196m},
            {Job.Viper, 1.0m},
            {Job.Pictomancer, 1.1m},

        };

        // Player object functions

        public void ResetJobDependantValues(){
            EtroBiS="";
            
            BisWeaponGearId=1;
            BisHeadGearId=1;
            BisBodyGearId=1 ;
            BisHandsGearId=1;
            BisLegsGearId=1;
            BisFeetGearId=1;
            BisEarringsGearId=1;
            BisNecklaceGearId=1;
            BisBraceletsGearId=1;
            BisLeftRingGearId=1;
            BisRightRingGearId=1;

            CurWeaponGearId=1;
            CurBodyGearId=1;
            CurHeadGearId=1;
            CurHandsGearId=1;
            CurLegsGearId=1;
            CurFeetGearId=1;
            CurEarringsGearId=1;
            CurNecklaceGearId=1;
            CurBraceletsGearId=1;
            CurLeftRingGearId=1;
            CurRightRingGearId=1;
        }

        public decimal ComputePlayerGearScore(decimal a, decimal b, decimal c, decimal GroupAvgLevel, decimal NRaidBuff, DataContext context){
            if (IsAlt){
                return 999999999m;
            }
            int PlayerILevel = get_avg_item_level(context:context);

            if (GroupAvgLevel == 0){
                Console.WriteLine("GroupAvgLevel is 0");
                return 0;
            }
            

            Static playerStatic = context.Statics.Find(this.staticId);
            int staticExpectedMinLevel = 660;
            if (playerStatic != null)
            {
                if (playerStatic.Tier == Tier.SEVEN_TWO)
                {
                    staticExpectedMinLevel = 710;
                }
                else if (playerStatic.Tier == Tier.SEVEN_4)
                {
                    staticExpectedMinLevel = 740;
                }
            }

            decimal score = a * 10 * JobScoreMultiplier[Job] * (PlayerILevel/GroupAvgLevel) + b * 100 * (GroupAvgLevel-PlayerILevel)/(GroupAvgLevel-staticExpectedMinLevel) +  
                            c * NRaidBuff * JobGroupMultiplier[Job];
            //Console.WriteLine($"PlayerILevel: {PlayerILevel} PlayerId : {Id}");
            //Console.WriteLine(score.ToString());
            return score;
        }

        public Dictionary<GearType,Gear?> get_gearset_as_dict(bool useBis, DataContext context){    
            /*This function returns the gearset of the player as a dictionnary where keys are name of gear type and
              they map to the Gear object.
              bool useBis -> If set to true uses Bis gear set. If false uses current gearset.
              DataContext context -> Data context to request gear.
            */

            // TODO : Check if returned gear is null

            Dictionary<GearType,Gear?> GearDict;

            if (useBis)
            {
                GearDict= new Dictionary<GearType, Gear?>() {
                {GearType.Weapon , context.Gears.Find(BisWeaponGearId)},
                {GearType.Head , context.Gears.Find(BisHeadGearId)},
                {GearType.Body , context.Gears.Find(BisBodyGearId)},
                {GearType.Hands , context.Gears.Find(BisHandsGearId)},
                {GearType.Legs , context.Gears.Find(BisLegsGearId)},
                {GearType.Feet , context.Gears.Find(BisFeetGearId)},
                {GearType.Earrings , context.Gears.Find(BisEarringsGearId)},
                {GearType.Necklace , context.Gears.Find(BisNecklaceGearId)},
                {GearType.Bracelets , context.Gears.Find(BisBraceletsGearId)},
                {GearType.LeftRing , context.Gears.Find(BisLeftRingGearId)},
                {GearType.RightRing , context.Gears.Find(BisRightRingGearId)}
            };
            } else 
            {
                GearDict= new Dictionary<GearType, Gear?>() {
                {GearType.Weapon , context.Gears.Find(CurWeaponGearId)},
                {GearType.Head , context.Gears.Find(CurHeadGearId)},
                {GearType.Body , context.Gears.Find(CurBodyGearId)},
                {GearType.Hands , context.Gears.Find(CurHandsGearId)},
                {GearType.Legs , context.Gears.Find(CurLegsGearId)},
                {GearType.Feet , context.Gears.Find(CurFeetGearId)},
                {GearType.Earrings , context.Gears.Find(CurEarringsGearId)},
                {GearType.Necklace , context.Gears.Find(CurNecklaceGearId)},
                {GearType.Bracelets , context.Gears.Find(CurBraceletsGearId)},
                {GearType.LeftRing , context.Gears.Find(CurLeftRingGearId)},
                {GearType.RightRing , context.Gears.Find(CurRightRingGearId)}
            };
            }

            return GearDict;
        }

        public int get_avg_item_level(Dictionary<GearType,Gear?>? GearDict=null, bool UseBis=false, DataContext context=null){
            /*Returns the average item level of the player. If GearDict is given a value uses that GearDict to compute it.
            Otherwise calls get_gearset_as_dict to get it. If GearDict is not null a DataContext must be specified.
            Returns -1 if the an error occured.
            GearDict -> GearDict formatted using get_gearset_as_dict.
            UseBis -> If true computes avg ilevel for Bis. If a non null GearDict is given this value does not matter.
            */

            if (GearDict == null){
                if (context == null){return -1;}
                GearDict = get_gearset_as_dict(UseBis, context);
            }
            int TotalItemLevel = 0;
            foreach (KeyValuePair<GearType, Gear?> pair in GearDict)
            {
                if (!(pair.Value is null)) 
                    TotalItemLevel += pair.Value.GearLevel;
            }

            return TotalItemLevel/GEARSETSIZE;
        }

        public void set_lock_player(Dictionary<string, int> param, Turn turn){
            //Console.WriteLine("Setting : " + turn.ToString());
            int RESET_TIME_IN_WEEK = param["RESET_TIME_IN_WEEK"];
            if (param["BOOL_FOR_1_FIGHT"] == 0)
            {
                turn = Turn.turn_5; // Lock out of all if so.
                //Console.WriteLine("Setting to turn_5");
            }
            //Console.WriteLine("Passed changing");
            DateTime now = DateTime.Now;
            int daysUntilNextTuesday = ((int)DayOfWeek.Tuesday - (int)now.DayOfWeek + 7) % 7;
            if (daysUntilNextTuesday == 0)
                daysUntilNextTuesday = 7; // On a tuesday we want to lock for the next week.
            //Console.WriteLine("Days until next tuesday : " + daysUntilNextTuesday.ToString());
            DateTime nextTuesday = now.AddDays(daysUntilNextTuesday + 7 * RESET_TIME_IN_WEEK);
            nextTuesday = nextTuesday.Date.AddHours(4);
            switch(turn){
                case Turn.turn_0:
                    break;
                case Turn.turn_1:
                    Turn1LockedUntilDate = Turn1LockedUntilDate > nextTuesday ? Turn1LockedUntilDate : nextTuesday;
                    break;
                case Turn.turn_2:
                    Turn2LockedUntilDate = Turn2LockedUntilDate > nextTuesday ? Turn2LockedUntilDate : nextTuesday;
                    break;
                case Turn.turn_3:
                    Turn3LockedUntilDate = Turn3LockedUntilDate > nextTuesday ? Turn3LockedUntilDate : nextTuesday;
                    break;
                case Turn.turn_4:
                    Turn4LockedUntilDate = Turn4LockedUntilDate > nextTuesday ? Turn4LockedUntilDate : nextTuesday;
                    break;
                case Turn.turn_5:
                    Turn1LockedUntilDate = Turn1LockedUntilDate > nextTuesday ? Turn1LockedUntilDate : nextTuesday;
                    Turn2LockedUntilDate = Turn2LockedUntilDate > nextTuesday ? Turn2LockedUntilDate : nextTuesday;
                    Turn3LockedUntilDate = Turn3LockedUntilDate > nextTuesday ? Turn3LockedUntilDate : nextTuesday;
                    Turn4LockedUntilDate = Turn4LockedUntilDate > nextTuesday ? Turn4LockedUntilDate : nextTuesday;
                    break;
            }
            //Console.WriteLine("Updating lock status until : " + nextTuesday.ToString());
            return;
        }

        public async Task<bool> need_this_gear(GearType NewGearType, GearStage NewGearStage, DataContext context){
            // Checks if the player needs this gear to see if it is being contested.
            Gear? curGear;
            Gear? bisGear;
            switch (NewGearType){
                case GearType.Weapon:
                   curGear = await context.Gears.FindAsync(CurWeaponGearId);
                   bisGear = await context.Gears.FindAsync(BisWeaponGearId);
                    break;
                case GearType.Head:
                    curGear = await context.Gears.FindAsync(CurHeadGearId);
                    bisGear = await context.Gears.FindAsync(BisHeadGearId);
                    break;
                case GearType.Body:
                    curGear = await context.Gears.FindAsync(CurBodyGearId);
                    bisGear = await context.Gears.FindAsync(BisBodyGearId);
                    break;
                case GearType.Hands:
                    curGear = await context.Gears.FindAsync(CurHandsGearId);
                    bisGear = await context.Gears.FindAsync(BisHandsGearId);
                    break;
                case GearType.Legs:
                    curGear = await context.Gears.FindAsync(CurLegsGearId);
                    bisGear = await context.Gears.FindAsync(BisLegsGearId);
                    break;
                case GearType.Feet:
                    curGear = await context.Gears.FindAsync(CurFeetGearId);
                    bisGear = await context.Gears.FindAsync(BisFeetGearId);
                    break;
                case GearType.Earrings:
                    curGear = await context.Gears.FindAsync(CurEarringsGearId);
                    bisGear = await context.Gears.FindAsync(BisEarringsGearId);
                    break;
                case GearType.Necklace:
                    curGear = await context.Gears.FindAsync(CurNecklaceGearId);
                    bisGear = await context.Gears.FindAsync(BisNecklaceGearId);
                    break;
                case GearType.Bracelets:
                    curGear = await context.Gears.FindAsync(CurBraceletsGearId);
                    bisGear = await context.Gears.FindAsync(BisBraceletsGearId);
                    break;
                case GearType.LeftRing:
                    curGear = await context.Gears.FindAsync(CurLeftRingGearId);
                    bisGear = await context.Gears.FindAsync(BisLeftRingGearId);
                    break;
                case GearType.RightRing:
                    curGear = await context.Gears.FindAsync(CurRightRingGearId);
                    bisGear = await context.Gears.FindAsync(BisRightRingGearId);
                    break;
                default:
                    return false;
            }
            if (curGear is null || bisGear is null)
                return false;

            return bisGear.GearStage == NewGearStage && curGear.GearStage != NewGearStage;
        }

        public async Task<bool> check_if_contested(Gear NewGear, Static s, DataContext context){

            List<Players> players = s.GetPlayers(context);

            foreach (Players player in players){
                if (player.Id != Id && await player.need_this_gear(NewGear.GearType, NewGear.GearStage, context)){
                    return true;
                }
            }

            return false;
        }

        public async Task update_lock_status(Gear OldGear, Gear NewGear, DataContext context, Turn turn){
            //Console.WriteLine("Updating lock status");
            Static? s = await context.Statics.FindAsync(staticId);
            if (s is null)
                return;

            // Loading static parameter
            Dictionary<string, int> param = s.GetLockParam();

            if (param["BOOL_LOCK_PLAYERS"] == 0)
                return;

            // Case 1 - Went from preperation to tome or preperation (Do nothing for now)
            if (OldGear.GearStage == GearStage.Preparation && 
               (NewGear.GearStage == GearStage.Tomes || NewGear.GearStage == GearStage.Preparation)){
                return;
            }

            // Case 2 - Went from tome or preperation to augmented tome and we lock for that.
            if ((OldGear.GearStage == GearStage.Tomes || OldGear.GearStage == GearStage.Preparation) && NewGear.GearStage == GearStage.Upgraded_Tomes 
                  && param["LOCK_IF_TOME_AUGMENT"] == 1){
                    // If (lock if not contested) is false and we are not contested, do nothing.
                    if (param["BOOL_LOCK_IF_NOT_CONTESTED"] == 0 && !(await check_if_contested(NewGear, s, context))){
                        return;
                    }
                    set_lock_player(param,turn);
                    return;
                }

            // Case 3 - Went from anything to raid
            if (OldGear.GearStage != GearStage.Raid && NewGear.GearStage == GearStage.Raid){
                //Console.WriteLine("Raid update");
                // If (lock if not contested) is false and we are not contested, do nothing.
                    if (param["BOOL_LOCK_IF_NOT_CONTESTED"] == 0 && !(await check_if_contested(NewGear, s, context))){
                        //Console.WriteLine("Not doing aything");
                        return;
                    }
                    //Console.WriteLine("Locking");
                    set_lock_player(param,turn);
                    return;
            }


        }
    
        public async Task change_gear_piece(GearType GearToChange, bool UseBis, int NewGearId, Turn turn,bool CheckLockPlayer, bool IsFromBook, DataContext context)
        {/*Updates the id of the specified gear piece for the player..
        GearToChange -> Value of the GearType to change.
        UseBis -> If true changes the value for Bis. Else for current.
        NewGearId -> Id (in database) of new gear to change to.
        */
        int oldCurGearId = 0;

        switch (GearToChange){
            case GearType.Weapon:
                if (UseBis)
                    BisWeaponGearId = NewGearId;
                else {
                    oldCurGearId = CurWeaponGearId;
                    CurWeaponGearId = NewGearId;
                }
                break;
            case GearType.Head:
                if (UseBis)
                    BisHeadGearId = NewGearId;
                else {
                    oldCurGearId = CurHeadGearId;
                    CurHeadGearId = NewGearId;
                }
                break;
            case GearType.Body:
                if (UseBis)
                    BisBodyGearId = NewGearId;
                else {
                    oldCurGearId = CurBodyGearId;
                    CurBodyGearId = NewGearId;
                }
                break;
            case GearType.Hands:
                if (UseBis)
                    BisHandsGearId = NewGearId;
                else {
                    oldCurGearId = CurHandsGearId;
                    CurHandsGearId = NewGearId;
                }
                break;
            case GearType.Legs:
                if (UseBis)
                    BisLegsGearId = NewGearId;
                else {
                    oldCurGearId = CurLegsGearId;
                    CurLegsGearId = NewGearId;
                }
                break;
            case GearType.Feet:
                if (UseBis)
                    BisFeetGearId = NewGearId;
                else {
                    oldCurGearId = CurFeetGearId;
                    CurFeetGearId = NewGearId;
                }
                break;
            case GearType.Earrings:
                if (UseBis)
                    BisEarringsGearId = NewGearId;
                else {
                    oldCurGearId = CurEarringsGearId;
                    CurEarringsGearId = NewGearId;
                }
                break;
            case GearType.Necklace:
                if (UseBis)
                    BisNecklaceGearId = NewGearId;
                else {
                    oldCurGearId = CurNecklaceGearId;
                    CurNecklaceGearId = NewGearId;
                }
                break;
            case GearType.Bracelets:
                if (UseBis)
                    BisBraceletsGearId = NewGearId;
                else {
                    oldCurGearId = CurBraceletsGearId;
                    CurBraceletsGearId = NewGearId;
                }
                break;
            case GearType.RightRing:
                if (UseBis)
                    BisRightRingGearId = NewGearId;
                else {
                    oldCurGearId = CurRightRingGearId;
                    CurRightRingGearId = NewGearId;
                }
                break;
            case GearType.LeftRing:
                if (UseBis)
                    BisLeftRingGearId = NewGearId;
                else {
                    oldCurGearId = CurLeftRingGearId;
                    CurLeftRingGearId = NewGearId;
                }
                break;
            default:
                break;
        }

        

            Gear? newGear = await context.Gears.FindAsync(NewGearId);
            if (newGear is null)
                return;
        if (CheckLockPlayer && !UseBis){
            Gear? oldGear = await context.Gears.FindAsync(oldCurGearId);
            
            
            if (oldGear is null)
                return;
            await this.update_lock_status(oldGear, newGear, context, turn);
        
        }
        if (!UseBis)
                await this.add_gear_acquisition_timestamp(newGear, turn, IsFromBook, context);
        }

        public async Task<bool> add_gear_acquisition_timestamp(Gear newGear, Turn turn, bool IsFromBook, DataContext context){
            if (newGear is null || (newGear.GearStage != GearStage.Raid && newGear.GearStage != GearStage.Upgraded_Tomes))
                return false;
            GearAcquisitionTimestamp newTimestamp = new GearAcquisitionTimestamp(){
                GearId = newGear.Id,
                PlayerId = Id,
                Timestamp = DateOnly.FromDateTime(DateTime.Now),
                turn = turn,
                isAcquiredFromBook=IsFromBook
            };
            await context.GearAcquisitionTimestamps.AddAsync(newTimestamp);
            await context.SaveChangesAsync();
            return true;
        }
    
        public StaticDTO.PlayerInfoDTO get_player_info(DataContext context, Static Static){

            Dictionary<GearType, Gear?> CurrentGearSetDict = get_gearset_as_dict(false, context);
            Dictionary<GearType, Gear?> BisGearSetDict = get_gearset_as_dict(true, context);

            Dictionary<string, GearOptionsDTO.GearOption?> CurrentGearSetInfo = new Dictionary<string, GearOptionsDTO.GearOption?>();
            Dictionary<string, GearOptionsDTO.GearOption?> BisGearSetInfo = new Dictionary<string, GearOptionsDTO.GearOption?>();

            foreach (KeyValuePair<GearType, Gear?> pair in CurrentGearSetDict){
                CurrentGearSetInfo[pair.Key.ToString()] = !(pair.Value is null) ? new GearOptionsDTO.GearOption{
                    GearName = pair.Value.Name,
                    GearStage = pair.Value.GearStage.ToString(),
                    GearId = pair.Value.Id,
                    GearItemLevel = pair.Value.GearLevel
                } : null;
                Gear? bisPair = BisGearSetDict[pair.Key];
                BisGearSetInfo[pair.Key.ToString()] = !(bisPair is null) ? new GearOptionsDTO.GearOption{
                    GearName = bisPair.Name,
                    GearStage = bisPair.GearStage.ToString(),
                    GearId = bisPair.Id,
                    GearItemLevel = bisPair.GearLevel
                } : null;
            }

            int AverageItemLevelCurrent = get_avg_item_level(GearDict:CurrentGearSetDict);
            int AverageItemLevelBis = get_avg_item_level(GearDict:BisGearSetDict);

            Dictionary<string, GearOptionsDTO> GearOptionPerGearType = new Dictionary<string, GearOptionsDTO>();

            foreach (GearType GearType in Enum.GetValues(typeof(GearType))){
                GearOptionPerGearType[GearType.ToString()] = Gear.GetGearOptions(GearType, Job, context, Static.Tier);
            }
            List<decimal> GearInfo = Static.ComputeNumberRaidBuffsAndGroupAvgLevel(context);
            decimal NumberRaidBuffs = GearInfo[0];
            decimal TeamAverageItemLevel = GearInfo[1];

            List<decimal> ScoreParam = Static.GetGearScoreParameter();
            decimal PlayerGearScore = ComputePlayerGearScore(ScoreParam[0], ScoreParam[1], ScoreParam[2], TeamAverageItemLevel, NumberRaidBuffs, context);

            List<CostDTO> Costs = new List<CostDTO>();

            foreach(KeyValuePair<GearType, Gear?> pair in CurrentGearSetDict){
                if((pair.Value is null) || BisGearSetDict[pair.Key] is null)
                    continue;
                Costs.Add(pair.Value.GetCost(BisGearSetDict[pair.Key]));
            }

            CostDTO Cost = CostDTO.SumCost(Costs);

            return new StaticDTO.PlayerInfoDTO(){
                Id=Id,
                Name=Name,
                EtroBiS=EtroBiS,
                Job=Job.ToString(),
                Locked=Locked,
                CurrentGearSet=CurrentGearSetInfo,
                BisGearSet=BisGearSetInfo,
                GearOptionPerGearType=GearOptionPerGearType,
                AverageItemLevelBis=AverageItemLevelBis,
                AverageItemLevelCurrent=AverageItemLevelCurrent,
                PlayerGearScore=PlayerGearScore,
                Cost=Cost,
                LockedList = new List<DateTime>(){Turn1LockedUntilDate, Turn2LockedUntilDate, Turn3LockedUntilDate, Turn4LockedUntilDate},
                IsClaimed=IsClaimed,
                IsAlt=IsAlt
            };
        }

        public StaticDTO.PlayerInfoSoftDTO get_player_info_soft(DataContext context){

            Dictionary<GearType, Gear?> CurrentGearSetDict = get_gearset_as_dict(false, context);
            Dictionary<GearType, Gear?> BisGearSetDict = get_gearset_as_dict(true, context);
            int AverageItemLevelCurrent = get_avg_item_level(GearDict:CurrentGearSetDict);
            int AverageItemLevelBis = get_avg_item_level(GearDict:BisGearSetDict);
            
            List<CostDTO> Costs = new List<CostDTO>();

            foreach(KeyValuePair<GearType, Gear?> pair in CurrentGearSetDict){
                if((pair.Value is null) || BisGearSetDict[pair.Key] is null)
                    continue;
                Costs.Add(pair.Value.GetCost(BisGearSetDict[pair.Key]));
            }

            CostDTO Cost = CostDTO.SumCost(Costs);

            return new StaticDTO.PlayerInfoSoftDTO(){
                AverageItemLevelBis=AverageItemLevelBis,
                AverageItemLevelCurrent=AverageItemLevelCurrent,
                Cost=Cost,
                LockedList = new List<DateTime>(){Turn1LockedUntilDate, Turn2LockedUntilDate, Turn3LockedUntilDate, Turn4LockedUntilDate},
                IsClaimed=IsClaimed
            };
        }   
        public void remove_lock(Turn turn){
            switch(turn){
                case Turn.turn_0:
                    return;
                case Turn.turn_1:
                    Turn1LockedUntilDate =  DateTime.MinValue;
                    return;
                case Turn.turn_2:
                    Turn2LockedUntilDate =  DateTime.MinValue;
                    return;
                case Turn.turn_3:
                    Turn3LockedUntilDate =  DateTime.MinValue;
                    return;
                case Turn.turn_4:
                    Turn4LockedUntilDate =  DateTime.MinValue;
                    return;
                case Turn.turn_5:
                    Turn1LockedUntilDate =  DateTime.MinValue;
                    Turn2LockedUntilDate =  DateTime.MinValue;
                    Turn3LockedUntilDate =  DateTime.MinValue;
                    Turn4LockedUntilDate =  DateTime.MinValue;
                    return;
            }
        }
    }
    

    public enum Job
    {
    Empty = 0,
    BlackMage = 1,
    Summoner = 2,
    RedMage = 3,
    Pictomancer = 20,
    WhiteMage = 4,
    Astrologian = 5,
    Sage = 6,
    Scholar = 7,
    Ninja = 8,
    Samurai = 9,
    Reaper = 10,
    Monk = 11,
    Dragoon = 12,
    Viper = 21,
    Gunbreaker = 13,
    DarkKnight = 14,
    Paladin = 15,
    Warrior = 16,
    Machinist = 17,
    Bard = 18,
    Dancer = 19,
    
    }
}
