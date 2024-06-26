using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.DTO;
using FFXIV_RaidLootAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FFXIV_RaidLootAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaticController : ControllerBase
    {
        private readonly IDbContextFactory<DataContext> _context;
        
        public StaticController(IDbContextFactory<DataContext> context)
        {
            _context = context;
            
        }
// Get All statics        
        [HttpGet(Name = "GetAllStatics")]
        public async Task<ActionResult<List<Static>>> GetAllStatics()
        {
            using (var context = _context.CreateDbContext())
            {
                List<Static> statics = await context.Statics.ToListAsync();
            
                return Ok(statics);
            }
            
        }


// Get only static name
        [HttpGet("GetOnlyStaticName/{uuid}")]
        public async Task<ActionResult<List<Static>>> GetOnlyStaticName(string uuid)
        {
            using (var context = _context.CreateDbContext())
            {
                return Ok(context.Statics.First(s => s.UUID == uuid).Name);
            }
        }


        [HttpGet("GetPGSParam/{uuid}")]
        public async Task<ActionResult<List<decimal>>> GetPGSParam(string uuid)
        {
            using (var context = _context.CreateDbContext())
            {   
                Static? s = await context.Statics.FirstOrDefaultAsync(u => u.UUID == uuid);
                if (s is null)
                    return NotFound("Static not found");
                
                return Ok(new List<decimal>() {s.GearScoreA, s.GearScoreB, s.GearScoreC});
            }
        }
        /*
        - BOOL_LOCK_PLAYERS; (FALSE)
        - BOOL_LOCK_IF_NOT_CONTESTED; (TRUE)
        - RESET_TIME_IN_WEEK; (1)
        - BOOL_FOR_1_FIGHT; (FALSE)
        - INT_NUMBER_OF_PIECES_UNTIL_LOCK; (1)
        - LOCK_IF_TOME_AUGMENT; (FALSE)
        - BOOL_IF_ROLE_CHANGES_NUMBER_PIECES; (FALSE)
        - DPS_NUMBER;TANK_NUMBER;HEALER_NUMBER (1),(1),(1)
        */
        [HttpPut("UpdateLockParam/{uuid}")]
        public async Task<ActionResult<List<decimal>>> UpdateLockParam(string uuid, ParamUpdateDTO NewParam)
        {
            using (var context = _context.CreateDbContext())
            {   
                Static? s = await context.Statics.FirstOrDefaultAsync(u => u.UUID == uuid);
                if (s is null)
                    return NotFound("Static not found");
                
                s.LOCK_PARAM = Static.DictParamToString(NewParam.DTOAsDict());
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpPut("SetPGSParam/{uuid}/{a}/{b}/{c}")]
        public async Task<ActionResult<List<decimal>>> GetPGSParam(string uuid, decimal a, decimal b, decimal c)
        {
            using (var context = _context.CreateDbContext())
            {   
                Static? s = await context.Statics.FirstOrDefaultAsync(u => u.UUID == uuid);
                if (s is null)
                    return NotFound("Static not found");
                
                s.GearScoreA = a;
                s.GearScoreB = b;
                s.GearScoreC = c;
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpGet("GetAllTimestampOfStatic/{uuid}")]
        public async Task<ActionResult<GearAcquisitionDTO>> GetAllTimestampOfStatic(string uuid){
            using (var context = _context.CreateDbContext()){
                var dbStatic = await context.Statics.FirstAsync(s => s.UUID == uuid);
                if (dbStatic is null)
                    return NotFound("Static not found");
                return Ok(new GearAcquisitionDTO(){
                    info=dbStatic.GetAllTimestampOfStatic(DateOnly.FromDateTime(DateTime.MinValue),context)
                });
            }
        }

        [HttpGet("GetGearAcquisitionForPastWeeksPerTurn/{uuid}/{numberWeek}")]
        public async Task<ActionResult<GearAcquisitionDTO>> GetGearAcquisitionForPastWeeksPerTurn(string uuid, int numberWeek){
            using (var context = _context.CreateDbContext()){
                var dbStatic = await context.Statics.FirstAsync(s => s.UUID == uuid);
                if (dbStatic is null)
                    return NotFound("Static not found");

                int DaysBeforeLastTuesday = ((int)DateTime.Now.DayOfWeek - (int)DayOfWeek.Tuesday + 7)%7;

                DateOnly FirstTuesdayToConsider = DateOnly.FromDateTime(DateTime.Now.AddDays(-1*DaysBeforeLastTuesday).AddDays(-7 * (numberWeek-1)));

                Dictionary<DateOnly, List<GearAcquisitionDTO.GearAcqInfo>> info = dbStatic.GetAllTimestampOfStatic(FirstTuesdayToConsider, context);

                DateOnly CounterDate = FirstTuesdayToConsider;

                Dictionary<DateOnly, List<GearAcquisitionDTO.GearAcqInfo>> response = new Dictionary<DateOnly, List<GearAcquisitionDTO.GearAcqInfo>>();

                while (CounterDate <= DateOnly.FromDateTime(DateTime.Now)){
                    response[CounterDate] = new List<GearAcquisitionDTO.GearAcqInfo>();
                    CounterDate = CounterDate.AddDays(7);
                }

                List<DateOnly> PossibleStartDate = response.Keys.ToList();
                //Console.WriteLine(PossibleStartDate.ToString() + " -- ARRAY");

                foreach (KeyValuePair<DateOnly, List<GearAcquisitionDTO.GearAcqInfo>> pair in info){
                    DateOnly KeyDate = DateOnly.FromDateTime(DateTime.Now);
                    for (int i = 0;i<PossibleStartDate.Count;i++){
                        if (i == PossibleStartDate.Count-1 || (pair.Key >= PossibleStartDate[i] && pair.Key < PossibleStartDate[i+1])){
                            KeyDate = PossibleStartDate[i];
                            break;
                        }
                    }

                    foreach (GearAcquisitionDTO.GearAcqInfo p in pair.Value){
                        response[KeyDate].Add(p);
                    }   
                }

                return Ok(new GearAcquisitionDTO(){
                    info=response
                });

            }
        }

        [HttpGet("PlayerGearScore/{uuid}")]
        public async Task<ActionResult<List<PlayerGearScoreDTO>>> GetPlayerGearScore(string uuid)
        {   
            try{
            using (var context = _context.CreateDbContext())
            {
                var dbStatic = await context.Statics.FirstAsync(s => s.UUID == uuid);
                List<Tuple<int, decimal>> PlayerGearScoreList = dbStatic.ComputePlayerGearScore(context);

                PlayerGearScoreDTO r = new PlayerGearScoreDTO();

                foreach (Tuple<int, decimal> val in PlayerGearScoreList){
                    r.PlayerGearScoreList.Add(new PlayerGearScoreDTO.PlayerGearScoreDTOInside {
                        id = val.Item1,
                        score = val.Item2
                    });
                }

                return Ok(r);


            }
            }
            catch (Exception e){
                return NotFound(e.Message);
            }
        }
// Get static by uuid        
        [HttpGet("{uuid}")]
        public async Task<ActionResult<StaticDTO>> GetStaticByUUID(string uuid)
        {
            using (var context = _context.CreateDbContext())
            {
                var dbStatic = await context.Statics.FirstOrDefaultAsync(s => s.UUID == uuid);
                if (dbStatic is null){
                    return NotFound("Static not found");
                }
                var playerList = context.Players.Where(p => p.staticId == dbStatic.Id).ToList();
                List<StaticDTO.PlayerInfoDTO> PlayersInfoList = new List<StaticDTO.PlayerInfoDTO>();

                foreach (Players player in playerList){
                    StaticDTO.PlayerInfoDTO info = player.get_player_info(context,dbStatic);
                    PlayersInfoList.Add(info);
                }

                StaticDTO aStatic = new StaticDTO
                {
                    Id = dbStatic.Id,
                    Name = dbStatic.Name,
                    UUID = dbStatic.UUID,
                    LockParam=dbStatic.GetLockParam(),
                    PlayersInfoList = PlayersInfoList
                };

                return Ok(aStatic);
            }
            
        }
// Add a new static        
        [HttpPut("CreateNewStatic/{name}")]
        public async Task<ActionResult<string>> AddStatic(string name)
        {
            using (var context = _context.CreateDbContext())
            {
                List<Static> staticList = await context.Statics.ToListAsync();
                string uuid = Guid.NewGuid().ToString();
                Static newStatic = new Static
                {
                    Name = name,
                    UUID = uuid
                };
                await context.Statics.AddAsync(newStatic);
                await context.SaveChangesAsync();
                var staticId = context.Statics.First(s => s.UUID == uuid).Id;
                
                //Add 8 empty players
                // Creates a player with default gear
                
                List<Players> players = new List<Players>();
                for (int i = 0; i < 8; i++)
                {
                    Players newPlayer = new Players 
                    {
                        Locked=false,
                        staticId=staticId,
                        Job=Job.BlackMage,
                        BisWeaponGearId=1,
                        CurWeaponGearId=1,
                        BisHeadGearId=1,
                        CurHeadGearId=1,
                        BisBodyGearId=1,
                        CurBodyGearId=1,
                        BisHandsGearId=1,
                        CurHandsGearId=1,
                        BisLegsGearId=1,
                        CurLegsGearId=1,
                        BisFeetGearId=1,
                        CurFeetGearId=1,
                        BisEarringsGearId=1,
                        CurEarringsGearId=1,
                        BisNecklaceGearId=1,
                        CurNecklaceGearId=1,
                        BisBraceletsGearId=1,
                        CurBraceletsGearId=1,
                        BisRightRingGearId=1,
                        CurRightRingGearId=1,
                        BisLeftRingGearId=1,
                        CurLeftRingGearId=1,
                    };
                    players.Add(newPlayer);
                }
                await context.AddRangeAsync(players);
                await context.SaveChangesAsync();
                return Ok(uuid);
            }
            
        }
// Edit a static
        [HttpPut]
        public async Task<ActionResult<Static>> UpdateStatic(AddStaticDTO aStatic)
        {
            using (var context = _context.CreateDbContext())
            {
                var dbStatic = await context.Statics.SingleAsync(s => s.UUID == aStatic.UUID);
                if (dbStatic is null)
                    return NotFound("Static is not found.");
                dbStatic.Name = aStatic.Name;

                await context.SaveChangesAsync();
                return Ok(dbStatic);
            }
            
        }
// Remove a static
        [HttpDelete]
        public async Task<ActionResult<List<Static>>> DeleteStatic(string uuid)
        {
            using (var context = _context.CreateDbContext())
            {
                var dbStatic = await context.Statics.SingleAsync(s => s.UUID == uuid);
                if (dbStatic is null)
                    return NotFound("Static not found");
                var dbPlayers = context.Players.Where(p => p.staticId == dbStatic.Id).ToList();

                foreach (var aPlayer in dbPlayers)
                {
                    context.Players.Remove(aPlayer);
                }
                context.Statics.Remove(dbStatic);
                await context.SaveChangesAsync();
            
                return Ok(await context.Statics.ToListAsync());
            }
            
        }
        
    }
}
