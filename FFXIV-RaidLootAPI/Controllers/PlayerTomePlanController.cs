using System.Security.Claims;
using ffxiRaidLootAPI.DTO;
using ffxiRaidLootAPI.Models;
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
    public class PlayerTomePlanController : ControllerBase
    {
        private readonly IDbContextFactory<DataContext> _context;
        private readonly IConfiguration _configuration;
        private readonly string _jwtKey;
        public PlayerTomePlanController(IDbContextFactory<DataContext> context,IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
            _jwtKey = _configuration["JwtSettings:Key"];
        }

        async private Task<bool> UserHasClaimedPlayerFromSameStatic<T>(T user, string playerId, DataContext context) where T : IUserInterface{
            // Now checks if this user claimed a player from the static. In which case they can edit this player.
            Players? player = await context.Players.FirstOrDefaultAsync(p => p.Id == int.Parse(playerId));
            if (player is null)
                return false;

            IEnumerable<Players> validPlayers = context.Players.Where(p => p.staticId == player.staticId);
            foreach (Players playerToInspect in validPlayers){
                if (user.UserClaimedPlayer(playerToInspect.Id.ToString()))
                    return true;
            }
            return false;
        }

        async private Task<bool> UserIsAuthorized(HttpContext HttpContext, string playerId, DataContext context){
            //Console.WriteLine("Checking authorization");
            if (HttpContext.Request.Cookies.TryGetValue("jwt_xivloot", out var jwt)){
                
                string discordId = await PlayerController.GetUserDiscordIdFromJwt(jwt, _jwtKey);

                Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == discordId);
                if (user is null)
                    return false;
                if (user.UserClaimedPlayer(playerId))
                    return true;

                return await UserHasClaimedPlayerFromSameStatic<Users>(user,playerId,context);

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

                return await UserHasClaimedPlayerFromSameStatic<ApplicationUser>(user,playerId,context);
            }

            return false;
        }

        [HttpPut("CreateTomePlan/{playerId}/{order}/{doneString}")]
        public async Task<ActionResult> CreateTomePlan(int playerId, string order, string doneString)
        {
            using (var context = _context.CreateDbContext())
            {        
                if (!await UserIsAuthorized(HttpContext,playerId.ToString(),context))
                    return Unauthorized();

                PlayerTomePlan newTomePlan = new PlayerTomePlan(){
                    playerId=playerId,
                    numberStartTomes = 0,
                    numberOffsetTomes = 0,
                    gearPlanOrder = order,
                    weekDoneString = doneString

                };
                await context.PlayerTomePlans.AddAsync(newTomePlan);
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpPut("SetWeekDone")]
        public async Task<ActionResult> SetWeekDone(TomePlanEditDTO playerTomePlanDto)
        {
            using (var context = _context.CreateDbContext())
            {
                if (!await UserIsAuthorized(HttpContext,playerTomePlanDto.playerId.ToString(),context))
                    return Unauthorized();

                PlayerTomePlan? playerTomePlan = await context.PlayerTomePlans.FirstOrDefaultAsync(p => p.playerId == playerTomePlanDto.playerId);
                if (playerTomePlan is null)
                    return NotFound();
                playerTomePlan.SetWeekDone(playerTomePlanDto.weekToEdit, playerTomePlanDto.done);
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpPut("AddToTomePlan")]
        public async Task<ActionResult> AddToTomePlan(TomePlanEditDTO tomePlanEditDTO)
        {
            using (var context = _context.CreateDbContext())
            {


                if (!await UserIsAuthorized(HttpContext,tomePlanEditDTO.playerId.ToString(),context))
                    return Unauthorized();

                PlayerTomePlan? playerTomePlan = await context.PlayerTomePlans.FirstOrDefaultAsync(p => p.playerId == tomePlanEditDTO.playerId);
                if (playerTomePlan is null)
                    return NotFound();

                if (Enum.TryParse(tomePlanEditDTO.GearToAdd, out GearType gearType))
                {
                    playerTomePlan.AddGearFromWeek(tomePlanEditDTO.weekToEdit, gearType);
                    await context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Invalid gear type.");
                }

            }
        }

        [HttpPut("RemoveFromTomePlan")]
        public async Task<ActionResult> RemoveFromTomePlan(TomePlanEditDTO tomePlanEditDTO)
        {
            using (var context = _context.CreateDbContext())
            {
                if (!await UserIsAuthorized(HttpContext,tomePlanEditDTO.playerId.ToString(),context))
                    return Unauthorized();

                PlayerTomePlan? playerTomePlan = await context.PlayerTomePlans.FirstOrDefaultAsync(p => p.playerId == tomePlanEditDTO.playerId);
                if (playerTomePlan is null)
                    return NotFound();

                if (Enum.TryParse(tomePlanEditDTO.GearToRemove, out GearType gearType))
                {
                    playerTomePlan.RemoveGearFromWeek(tomePlanEditDTO.weekToEdit, gearType);
                    await context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Invalid gear type.");
                }

            }
        }

        [HttpPut("SetStartTomes")]
        public async Task<ActionResult> SetStartTomes(TomePlanEditDTO tomePlanEditDTO)
        {
            using (var context = _context.CreateDbContext())
            {

                if (!await UserIsAuthorized(HttpContext,tomePlanEditDTO.playerId.ToString(),context))
                    return Unauthorized();

                PlayerTomePlan? playerTomePlan = await context.PlayerTomePlans.FirstOrDefaultAsync(p => p.playerId == tomePlanEditDTO.playerId);
                if (playerTomePlan is null)
                    return NotFound();

                try{
                    playerTomePlan.numberStartTomes = tomePlanEditDTO.numberStartTomes;
                }
                catch(Exception e){
                    playerTomePlan.numberStartTomes = 0;
                    await context.SaveChangesAsync();
                    return Ok();
                }
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpPut("SetOffsetTomes")]
        public async Task<ActionResult> SetOffsetTomes(TomePlanEditDTO tomePlanEditDTO)
        {
            using (var context = _context.CreateDbContext())
            {
                if (!await UserIsAuthorized(HttpContext,tomePlanEditDTO.playerId.ToString(),context))
                    return Unauthorized();

                PlayerTomePlan? playerTomePlan = await context.PlayerTomePlans.FirstOrDefaultAsync(p => p.playerId == tomePlanEditDTO.playerId);
                if (playerTomePlan is null)
                    return NotFound();

                try{
                    playerTomePlan.numberOffsetTomes = tomePlanEditDTO.numberOffsetTomes;
                }
                catch(Exception e){
                    playerTomePlan.numberOffsetTomes = 0;
                    await context.SaveChangesAsync();
                    return Ok();
                }
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpPut("AddWeekToTomePlan")]
        public async Task<ActionResult> AddWeekToTomePlan(TomePlanEditDTO tomePlanEditDTO)
        {
            using (var context = _context.CreateDbContext())
            {
                if (!await UserIsAuthorized(HttpContext,tomePlanEditDTO.playerId.ToString(),context))
                    return Unauthorized();

                PlayerTomePlan? playerTomePlan = await context.PlayerTomePlans.FirstOrDefaultAsync(p => p.playerId == tomePlanEditDTO.playerId);
                if (playerTomePlan is null)
                    return NotFound();
                if (tomePlanEditDTO.weekToEdit == 0)
                {
                playerTomePlan.gearPlanOrder = ";" + playerTomePlan.gearPlanOrder;
                playerTomePlan.weekDoneString = "0;" + playerTomePlan.weekDoneString;
                } else if(tomePlanEditDTO.weekToEdit == -1){
                    playerTomePlan.gearPlanOrder = playerTomePlan.gearPlanOrder + ";";
                    playerTomePlan.weekDoneString = playerTomePlan.weekDoneString + ";0";
                }

                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpPut("RemoveWeekFromTomePlan")]
        public async Task<ActionResult> RemoveWeekFromTomePlan(TomePlanEditDTO tomePlanEditDTO)
        {
            using (var context = _context.CreateDbContext())
            {
                if (!await UserIsAuthorized(HttpContext,tomePlanEditDTO.playerId.ToString(),context))
                    return Unauthorized();

                PlayerTomePlan? playerTomePlan = await context.PlayerTomePlans.FirstOrDefaultAsync(p => p.playerId == tomePlanEditDTO.playerId);
                if (playerTomePlan is null)
                    return NotFound();
                playerTomePlan.RemoveWeekFromGearPlan(tomePlanEditDTO.weekToEdit);
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpGet("GetPlayerTomePlan/{playerId}")]
        public async Task<ActionResult<PlayerTomePlanDto>> GetPlayerTomePlan(int playerId)
        {
            using (var context = _context.CreateDbContext())
            {
                PlayerTomePlan? playerTomePlan = await context.PlayerTomePlans.FirstOrDefaultAsync(p => p.playerId == playerId);
                Console.WriteLine("After playerTomePlan : " + playerTomePlan);
                if (playerTomePlan is null)
                    return NotFound();
                Tuple<List<GearPlanSingle>,int> tup = playerTomePlan.ComputeGearPlanInfo();
                List<GearPlanSingle> rList = tup.Item1;
                int cost = tup.Item2;
                Console.WriteLine("After rList : " + rList);
                await context.SaveChangesAsync(); // Changing if we added weeks

                return Ok(new PlayerTomePlanDto(){
                    numberWeeks = playerTomePlan.numberWeeks,
                    numberStartTomes = playerTomePlan.numberStartTomes,
                    numberOffsetTomes = playerTomePlan.numberOffsetTomes,
                    totalCost = cost,
                    gearPlanOrder = rList,
                    weekDone = playerTomePlan.GetWeekDoneList()
                });
            }
        }
    }
}