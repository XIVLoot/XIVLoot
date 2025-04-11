using System.Security.Claims;
using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.DTO;
using FFXIV_RaidLootAPI.Models;
using FFXIV_RaidLootAPI.User;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FFXIV_RaidLootAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]
    public class StaticController : ControllerBase
    {
        private readonly IDbContextFactory<DataContext> _context;

        private readonly string _jwtKey;
        private readonly IConfiguration _configuration;
        
        public StaticController(IDbContextFactory<DataContext> context,IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
            _jwtKey = _configuration["JwtSettings:Key"]!;
            
        }

        async private Task<bool> UserHasClaimedPlayerFromSameStatic<T>(T user, string playerId, DataContext context, int staticId) where T : IUserInterface{
            // Now checks if this user claimed a player from the static. In which case they can edit this player.

            Players? player = await context.Players.FirstOrDefaultAsync(p => p.Id == int.Parse(playerId));
            if (player is null && staticId == 0)
                return false;
            if (player != null)
                staticId = player.staticId;
            IEnumerable<Players> validPlayers = context.Players.Where(p => p.staticId == staticId);
            foreach (Players playerToInspect in validPlayers){
                if (user.UserClaimedPlayer(playerToInspect.Id.ToString()))
                    return true;
            }
            return false;
        }

        async private Task<bool> UserIsAuthorized(HttpContext HttpContext, string playerId, DataContext context, int staticId){
            Console.WriteLine("Checking authorization");
            if (HttpContext.Request.Cookies.TryGetValue("jwt_xivloot", out var jwt)){

                string discordId = await PlayerController.GetUserDiscordIdFromJwt(jwt, _jwtKey);

                Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == discordId);
                if (user is null)
                    return false;
                if (user.UserClaimedPlayer(playerId))
                    return true;

                return await UserHasClaimedPlayerFromSameStatic<Users>(user,playerId,context, staticId);

            } 
            else if (!(User is null)){
                //Console.WriteLine("DEFAUTL CONNECTED");
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
                var userId = userIdClaim?.Value;

                Console.WriteLine("USER ID FROM EMAIL IS : " + userId);

                ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user is null)
                    return false;

                bool thisUserClaimed = user.UserClaimedPlayer(playerId);
                if (thisUserClaimed)
                    return true;

                return await UserHasClaimedPlayerFromSameStatic<ApplicationUser>(user,playerId,context, staticId);
            }

            return false;
        }


        [HttpGet("UserIsOwner/{uuid}")]
        public async Task<IActionResult> CheckUserOwnsStatic(string uuid){
            using (var context = _context.CreateDbContext())
            {

                Static? dbStatic = await context.Statics.FirstOrDefaultAsync(s => s.UUID == uuid);
                if (dbStatic is null)
                    return Ok(false);

                if(HttpContext.Request.Cookies.TryGetValue("jwt_xivloot", out var jwt)) // Discord user
                {
                    string discordId = await PlayerController.GetUserDiscordIdFromJwt(jwt, _jwtKey);

                    Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == discordId);
                    if (!(user is null))
                        return Ok(user.Id.ToString() == dbStatic.ownerIdString);
                } else if (!(User is null)) // Email user. Note - email user id are uuid while discord userid are integer id
                {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                    return Ok(userIdClaim.Value == dbStatic.ownerIdString);
                }
                return Ok(false);
            }
        }

        [HttpPut("UnclaimStaticOwnerShip/{uuid}")]
        public async  Task<IActionResult> UnclaimStaticOwnership(string uuid){
            using (var context = _context.CreateDbContext())
            {
                Static? dbStatic = await context.Statics.FirstOrDefaultAsync(s => s.UUID == uuid);
                if (dbStatic is null)
                    return NotFound(false);

                if (dbStatic.ownerIdString == "")
                    return Unauthorized(false);

                // Check if can actually do it in case the user was evil):
                if(HttpContext.Request.Cookies.TryGetValue("jwt_xivloot", out var jwt)) // Discord user
                {
                    string discordId = await PlayerController.GetUserDiscordIdFromJwt(jwt, _jwtKey);

                    Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == discordId);
                    if (user is null)
                        return NotFound();

                    if (user.Id.ToString() == dbStatic.ownerIdString)
                        dbStatic.ownerIdString = "";

                } 
                else if (!(User is null)) // Email user. Note - email user id are uuid while discord userid are integer id
                    {
                    var claimsIdentity = User.Identity as ClaimsIdentity;
                    var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

                    if (userIdClaim is null)
                        return Unauthorized(false);
                    ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userIdClaim.Value);
                    if (user is null)
                        return NotFound(false);

                    if (user.Id == dbStatic.ownerIdString)
                        dbStatic.ownerIdString = "";
                    } 
                else
                    return Unauthorized(false); // Has to be a logged in user

                await context.SaveChangesAsync();
                return Ok(true);
            }
        }
        
        [HttpPut("ClaimStaticOwnerShip/{uuid}")]
        public async  Task<IActionResult> ClaimStaticOwnership(string uuid){
            using (var context = _context.CreateDbContext())
            {
                Static? dbStatic = await context.Statics.FirstOrDefaultAsync(s => s.UUID == uuid);
                if (dbStatic is null)
                    return NotFound(false);

                if (dbStatic.ownerIdString != "")
                    return Unauthorized(false);

                // Check if can actually do it in case the user was evil):
                if(HttpContext.Request.Cookies.TryGetValue("jwt_xivloot", out var jwt)) // Discord user
                {
                    string discordId = await PlayerController.GetUserDiscordIdFromJwt(jwt, _jwtKey);

                    Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == discordId);
                    if (user is null)
                        return Unauthorized(false);

                    IEnumerable<Players> playerInStatic = context.Players.Where(p => p.staticId == dbStatic.Id);

                    foreach (string pId in user.getAllClaimedPlayerId()){
                        foreach (Players player in playerInStatic){
                            if (player.Id.ToString() == pId)
                            {
                                dbStatic.ownerIdString = user.Id.ToString();
                                await context.SaveChangesAsync();
                                 return Ok(true);
                            }
                        }
                    }
                } 
                else if (!(User is null)) // Email user. Note - email user id are uuid while discord userid are integer id
                    {
                    var claimsIdentity = User.Identity as ClaimsIdentity;
                    var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

                    if (userIdClaim is null)
                        return Unauthorized(false);
                    ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userIdClaim.Value);
                    if (user is null)
                        return NotFound(false);

                    IEnumerable<Players> playerInStatic = context.Players.Where(p => p.staticId == dbStatic.Id);

                    

                    foreach (string pId in user.getAllClaimedPlayerId()){
                        Console.WriteLine("User claimed player : " + pId);
                        foreach (Players player in playerInStatic){
                            Console.WriteLine("User claimed player : " + player.Id.ToString());
                            if (player.Id.ToString() == pId)
                            {
                                dbStatic.ownerIdString = userIdClaim.Value;
                                await context.SaveChangesAsync();
                                return Ok(true);
                            }
                        }
                    }
                    } 
                else
                    return Unauthorized(false); // Has to be a logged in user

                return Ok(false);
            }
        }

        [HttpGet("GetOwnerName/{uuid}")]
        public async Task<IActionResult> GetOwnerName(string uuid){
            using (var context = _context.CreateDbContext())
            {
                Static? dbStatic = await context.Statics.FirstOrDefaultAsync(s => s.UUID == uuid);
                if (dbStatic is null)
                    return NotFound();
                
                if (dbStatic.ownerIdString == "")
                    return Ok(""); // Means no owner

                if (Guid.TryParse(dbStatic.ownerIdString,out _)){
                    // is uuid so email account
                    ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == dbStatic.ownerIdString);
                    if (user is null)
                        return NotFound();

                    IEnumerable<Players> playerInStatic = context.Players.Where(p => p.staticId == dbStatic.Id);

                    foreach (string pId in user.getAllClaimedPlayerId()){
                        foreach (Players player in playerInStatic){
                            if (player.Id.ToString() == pId)
                                return Ok(player.Name);
                        }
                    }

                    return Ok("CLAIM A PLAYER");
                } 
                else{
                // else is a discord user
                    Users? user = await context.User.FirstOrDefaultAsync(u => u.Id.ToString() == dbStatic.ownerIdString);
                    if (user is null)
                        return NotFound();

                    IEnumerable<Players> playerInStatic = context.Players.Where(p => p.staticId == dbStatic.Id);

                    foreach (string pId in user.getAllClaimedPlayerId()){
                        foreach (Players player in playerInStatic){
                            if (player.Id.ToString() == pId)
                                return Ok(player.Name);
                        }
                    }
                    return Ok("CLAIM A PLAYER");
                }
            }
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

        [HttpGet("GetItemNeedForPlayers/{uuid}")]
        public async Task<ActionResult<ItemBreakdownDTO>> GetItemNeedForPlayers(string uuid)
        {
            using (var context = _context.CreateDbContext())
            {
                var dbStatic = await context.Statics.FirstAsync(s => s.UUID == uuid);
                if (dbStatic is null)
                    return NotFound("Static not found");

                List<Players> players = await context.Players.Where(p => p.staticId == dbStatic.Id).ToListAsync();
                ItemBreakdownDTO itemBreakdown = new ItemBreakdownDTO();
                foreach (GearType type in Enum.GetValues(typeof(GearType)))
                {
                    if (type == GearType.Empty)
                        continue;

                    foreach (Players player in players)
                    {
                        if (player is null)
                            continue;
                        
                        Turn turn = Turn.turn_0;

                        // Need raid?
                        if (await player.need_this_gear(type, GearStage.Raid, context))
                        {
                            switch(type){
                                case GearType.Weapon:
                                    turn = Turn.turn_4;
                                    break;
                                case GearType.Legs:
                                case GearType.Body:
                                    turn = Turn.turn_3;
                                    break;
                                case GearType.Head:
                                case GearType.Hands:
                                case GearType.Feet:
                                    turn = Turn.turn_2;
                                    break;
                                default:
                                    turn = Turn.turn_1;
                                    break;
                            }

                            /*if (turn == Turn.turn_2){
                                // Everything raid from 2 can drop in 3 so also add to turn_3.
                                itemBreakdown.ItemBreakdown[Enum.GetName(typeof(Turn), Turn.turn_2)!][type.ToString()].Add(new ItemBreakdownDTO.PlayerInfoItemBreakdown(){
                                    Name=player.Name,
                                    NeedThisGearType=true,
                                    playerId=player.Id
                                });
                                itemBreakdown.ItemBreakdown[Enum.GetName(typeof(Turn), Turn.turn_3)!][type.ToString()].Add(new ItemBreakdownDTO.PlayerInfoItemBreakdown(){
                                    Name=player.Name,
                                    NeedThisGearType=true,
                                    playerId=player.Id
                                });
                            } else */
                            if(turn == Turn.turn_1 && (type == GearType.RightRing || type == GearType.LeftRing)){
                                if(!itemBreakdown.PlayerAlreadyNeed(player.Id, Turn.turn_1, "Ring")){
                                    itemBreakdown.ItemBreakdown[Enum.GetName(typeof(Turn), turn)!]["Ring"].Add(new ItemBreakdownDTO.PlayerInfoItemBreakdown(){
                                        Name=player.Name,
                                        NeedThisGearType=true,
                                        playerId=player.Id
                                    });
                                }
                            }
                            else{
                                itemBreakdown.ItemBreakdown[Enum.GetName(typeof(Turn), turn)!][type.ToString()].Add(new ItemBreakdownDTO.PlayerInfoItemBreakdown(){
                                    Name=player.Name,
                                    NeedThisGearType=true,
                                    playerId=player.Id
                                });
                            }
                            turn = Turn.turn_0; // Reset
                        }

                        

                        // Need tome augment
                        if (await player.need_this_gear(type, GearStage.Upgraded_Tomes, context))
                        {
                            switch(type){
                                case GearType.Weapon:
                                    turn = Turn.turn_4;
                                    break;
                                case GearType.Legs:
                                case GearType.Body:
                                case GearType.Feet:
                                case GearType.Hands:
                                case GearType.Head:
                                    turn = Turn.turn_3;
                                    break;
                                default:
                                    turn = Turn.turn_2;
                                    break;
                            }

                            //if (turn == Turn.turn_4)// TODO SUPPORT WEAPON UPGRADE
                            //    continue;
                            if (turn == Turn.turn_2){
                                // Check if the player already needs a shine. If they do we add to the counter
                                if(!itemBreakdown.PlayerAlreadyNeed(player.Id, Turn.turn_2, "Shine")){
                                    itemBreakdown.ItemBreakdown[Enum.GetName(typeof(Turn), Turn.turn_2)!]["Shine"].Add(new ItemBreakdownDTO.PlayerInfoItemBreakdown(){
                                        Name=player.Name,
                                        NeedThisGearType=true,
                                        playerId=player.Id
                                    });
                                }
                            } else if (turn == Turn.turn_3){
                                if(!itemBreakdown.PlayerAlreadyNeed(player.Id, Turn.turn_3, "Twine")){
                                    itemBreakdown.ItemBreakdown[Enum.GetName(typeof(Turn), turn)!]["Twine"].Add(new ItemBreakdownDTO.PlayerInfoItemBreakdown(){
                                        Name=player.Name,
                                        NeedThisGearType=true,
                                        playerId=player.Id
                                    });
                                }
                            }
                        }
                    }

                }
                return Ok(itemBreakdown);
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
        public async Task<ActionResult<List<decimal>>> SetPGSParam(string uuid, decimal a, decimal b, decimal c)
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
                ////Console.WriteLine(PossibleStartDate.ToString() + " -- ARRAY");

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

                // Add non all players first.
                foreach (Players player in playerList.Where(p => p.IsAlt == false))
                {
                    StaticDTO.PlayerInfoDTO info = player.get_player_info(context,dbStatic);
                    PlayersInfoList.Add(info);
                }

                // Add altplayers second so they appear at the end of the list.
                foreach (Players player in playerList.Where(p => p.IsAlt))
                {
                    StaticDTO.PlayerInfoDTO info = player.get_player_info(context,dbStatic);
                    PlayersInfoList.Add(info);
                }

                StaticDTO aStatic = new StaticDTO
                {
                    Id = dbStatic.Id,
                    Name = dbStatic.Name,
                    UUID = dbStatic.UUID,
                    LockParam=dbStatic.GetLockParam(),
                    PlayersInfoList = PlayersInfoList,
                    Tier = dbStatic.Tier
                };

                return Ok(aStatic);
            }
            
        }

//Add Player to static which will be flagged as alt
        [HttpPut("AddNewPlayerToStatic/{uuid}")]
        public async Task<IActionResult> AddNewPlayerToStatic(string uuid){
            using (var context = _context.CreateDbContext()){
                var dbStatic = await context.Statics.FirstAsync(s => s.UUID == uuid);
                if (dbStatic is null)
                    return NotFound("Static not found");

                Players player = new Players(){
                        Locked=false,
                        staticId=dbStatic.Id,
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
                        IsAlt=true
                };

                bool isAuthorized = await UserIsAuthorized(HttpContext, "0", context, dbStatic.Id); // Must be in the static to add a player.
                if(!isAuthorized)
                    return Unauthorized("Not Authorized");

                await context.Players.AddAsync(player);
                await context.SaveChangesAsync();
                return Ok(player.get_player_info(context,dbStatic));
            }
        }

// Add a new static        
        [HttpPut("CreateNewStatic/{name}/{tier}")]
        public async Task<ActionResult<string>> AddStatic(string name, int tier)
        {
            using (var context = _context.CreateDbContext())
            {
                string userId = string.Empty;
                // Getting user info if any to add as creator.

                if(HttpContext.Request.Cookies.TryGetValue("jwt_xivloot", out var jwt)) // Discord user
                {
                    string discordId = await PlayerController.GetUserDiscordIdFromJwt(jwt, _jwtKey);

                    Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == discordId);
                    if (!(user is null))
                        userId = user.Id.ToString();
                } else if (!(User is null)) // Email user. Note - email user id are uuid while discord userid are integer id
                {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                    userId = userIdClaim.Value;
                }

                List<Static> staticList = await context.Statics.ToListAsync();
                string uuid = Guid.NewGuid().ToString();
                Static newStatic = new Static
                {
                    Name = name,
                    UUID = uuid,
                    ownerIdString=userId,
                    Tier = (Tier)tier
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
                        IsAlt=false
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
