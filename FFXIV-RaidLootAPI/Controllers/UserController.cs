using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.DTO;
using FFXIV_RaidLootAPI.Models;
using FFXIV_RaidLootAPI.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FFXIV_RaidLootAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]
    public class UserController : ControllerBase
    {
        private readonly IDbContextFactory<DataContext> _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(IDbContextFactory<DataContext> context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPut("AddUserDiscordId/{user_discord_id}")]
        public async Task<IActionResult> AddUserDiscordId(string user_discord_id)
        {
            using (var context = _context.CreateDbContext())
            {


            Users? checkUser = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == user_discord_id);

            if (!(checkUser is null)){
                Console.WriteLine("User already exists");
                return Ok(new Users(){
                user_discord_id=user_discord_id,
                user_saved_static=checkUser.user_saved_static
            });
            }

            Users newuser = new Users(){
                user_discord_id=user_discord_id,
                user_saved_static=""
            };
            await context.User.AddAsync(newuser);
            await context.SaveChangesAsync();
            return Ok(newuser);
            }
        }

        [HttpGet("GetUserSavedStatic/{user_discord_id}")]
        public async Task<IActionResult> GetUserSavedStaticDiscord(string user_discord_id)
        {
            using (var context = _context.CreateDbContext())
            {
            Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == user_discord_id);
            if (user is null){
                return NotFound("User not found");
            }
            string static_list_as_string = user.user_saved_static;
            List<string> uuidList = static_list_as_string.Split(';').ToList();
            return Ok(uuidList);
            }
        }

        [HttpPut("AddStaticToUserSaved/{user_discord_id}/{static_uuid}")]
        public async Task<IActionResult> AddStaticToUserSavedDiscord(string user_discord_id, string static_uuid)
        {
            using (var context = _context.CreateDbContext())
            {
            Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == user_discord_id);
            if (user is null){
                return NotFound("User not found");
            }

            string static_list_as_string = user.user_saved_static;
            List<string> uuidList = static_list_as_string.Split(';').ToList();

            if (uuidList.Contains(static_uuid)){
                return Ok("Static already saved");
            }

            user.user_saved_static += static_uuid + ";";
            await context.SaveChangesAsync();
            return Ok(user);
            }
        }

        [HttpPut("RemoveStaticToUserSaved/{user_discord_id}/{static_uuid}")]
        public async Task<IActionResult> RemoveStaticToUserSavedDiscord(string user_discord_id, string static_uuid)
        {
            using (var context = _context.CreateDbContext())
            {
            Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == user_discord_id);
            if (user is null){
                return NotFound("User not found");
            }
            string static_list_as_string = user.user_saved_static;
            List<string> uuidList = static_list_as_string.Split(';').ToList();
            uuidList.Remove(static_uuid);
            user.user_saved_static = String.Join(";", uuidList);
            await context.SaveChangesAsync();
            return Ok(user);
            }
        }

        [HttpGet("GetUserEmail")]
        [Authorize]
        public async Task<IActionResult> GetUserEmail(){
            using (var context = _context.CreateDbContext())
            {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return NotFound("");

            return Ok(user.Email);
            }
        }

        [HttpGet("IsLoggedIn")]
        [Authorize]
        public IActionResult IsUserLoggedIn()
        {
            return Ok(true);
        }

        [HttpGet("SetUsername/{newName}")]
        [Authorize]
        public async Task<IActionResult> Setusername(string newName){
            using (var context = _context.CreateDbContext())
            {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return NotFound("");

            user.user_displayed_name = newName;
            await context.SaveChangesAsync();
            return Ok();
            }
        }

        [HttpGet("GetUsernameDefault")]
        [Authorize]
        public async Task<IActionResult> GetUsername(){
            using (var context = _context.CreateDbContext())
            {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            Console.WriteLine("HEYYYYYYYY  : " + userId.ToString());

            ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return NotFound("");

            return Ok(user.user_displayed_name);
            }
        }

        [HttpGet("GetUserSavedStatic")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            using (var context = _context.CreateDbContext())
            {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return NotFound("");

            string static_list_as_string = user.user_saved_static;
            List<string> uuidList = static_list_as_string.Split(';').ToList();
            return Ok(uuidList);
            }
        }

        [HttpPut("RemoveStaticToUserSaved/{static_uuid}")]
        [Authorize]
        public async Task<IActionResult> RemoveStaticToUserSaved(string static_uuid)
        {
            using (var context = _context.CreateDbContext())
            {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return NotFound("");
            
            string static_list_as_string = user.user_saved_static;
            List<string> uuidList = static_list_as_string.Split(';').ToList();
            uuidList.Remove(static_uuid);
            user.user_saved_static = String.Join(";", uuidList);
            await context.SaveChangesAsync();
            return Ok();
            }
        }

        [HttpPut("AddStaticToUserSaved/{static_uuid}")]
        public async Task<IActionResult> AddStaticToUserSaved(string static_uuid)
        {
            using (var context = _context.CreateDbContext())
            {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return NotFound("");

            string static_list_as_string = user.user_saved_static;
            List<string> uuidList = static_list_as_string.Split(';').ToList();

            if (uuidList.Contains(static_uuid)){
                return Ok("Static already saved");
            }

            user.user_saved_static += static_uuid + ";";
            await context.SaveChangesAsync();
            return Ok();
            }
        }

        [HttpGet("IsPlayerClaimedByUserDiscord/{discord_id}/{playerId}")]
        public async Task<IActionResult> IsPlayerClaimedByUserDiscord(string discord_id, string playerId){
            using (var context = _context.CreateDbContext())
            {
                Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == discord_id);
                if (user is null)
                    return NotFound();
                
                return Ok(user.UserClaimedPlayer(playerId));
            }
        }

        [HttpGet("IsPlayerClaimedByUserDefault/{playerId}")]
        public async Task<IActionResult> IsPlayerClaimedByUserDefault(string playerId){
            using (var context = _context.CreateDbContext())
            {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return NotFound("User not found");
                
                return Ok(user.UserClaimedPlayer(playerId));
            }
        }

        [HttpPut("ClaimPlayerDefault/{playerId}")]
        public async Task<IActionResult> ClaimPlayerDefault(int playerId)
        {
            using (var context = _context.CreateDbContext())
            {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return NotFound("User not found");
            //Console.WriteLine("CLAIMING DEFAULT FOUND USER: ");
            Players? player = await context.Players.FirstOrDefaultAsync(p => p.Id == playerId);
            if (player is null || player.IsClaimed)
                return NotFound("");

            //Console.WriteLine("CLAIMING DEFAULT FOUND PLAYER: ");

            player.IsClaimed = true;
            user.user_claimed_playerId += playerId.ToString()+";";
            Console.WriteLine("ADDED :" + user.user_claimed_playerId);
            await context.SaveChangesAsync();
            return Ok();
            }
        }

        [HttpPut("UnclaimPlayerDefault/{playerId}")]
        public async Task<IActionResult> UnclaimPlayerDefault(int playerId)
        {
            using (var context = _context.CreateDbContext())
            {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return NotFound("User not found");
            //Console.WriteLine("CLAIMING DEFAULT FOUND USER: ");
            Players? player = await context.Players.FirstOrDefaultAsync(p => p.Id == playerId);
            if (player is null)
                return NotFound("Player not found.");

            //Console.WriteLine("CLAIMING DEFAULT FOUND PLAYER: ");

            player.IsClaimed = false;
            user.removePlayerClaim(playerId.ToString());
            await context.SaveChangesAsync();
            return Ok();
            }
        }

        [HttpGet("GetAllClaimedPlayerDefault")]
        public async Task<ActionResult<List<ClaimedPlayerInfoDTO>>> GetAllClaimedPlayerDefault()
        {
            using (var context = _context.CreateDbContext())
            {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return NotFound();

            List<ClaimedPlayerInfoDTO> rList = new List<ClaimedPlayerInfoDTO>();

            foreach (string pId in user.getAllClaimedPlayerId()){
                if (pId == ";" || pId == "")
                    continue;
                Players? player = await context.Players.FirstOrDefaultAsync(p => p.Id ==  int.Parse(pId));

                if (!(player is null)){
                    Static? dBStatic = await context.Statics.FirstOrDefaultAsync(s => s.Id == player.staticId);
                    if (!(dBStatic is null)){

                        Dictionary<GearType, Gear?> CurrentGearSetDict = player.get_gearset_as_dict(false, context);
                        Dictionary<GearType, Gear?> BisGearSetDict = player.get_gearset_as_dict(true, context);

                        List<CostDTO> Costs = new List<CostDTO>();

                        foreach(KeyValuePair<GearType, Gear?> pair in CurrentGearSetDict){
                            if((pair.Value is null) || BisGearSetDict[pair.Key] is null)
                                continue;
                            Costs.Add(pair.Value.GetCost(BisGearSetDict[pair.Key]!));
                        }

                        CostDTO Cost = CostDTO.SumCost(Costs);

                        rList.Add(new ClaimedPlayerInfoDTO(){
                            Name=player.Name,
                            Job = player.Job.ToString(),
                            pId = player.Id,
                            StaticName=dBStatic.Name,
                            StaticUUID=dBStatic.UUID,
                            CurrentAverageItemLevel=player.get_avg_item_level(GearDict:CurrentGearSetDict),
                            BisAverageItemLevel=player.get_avg_item_level(GearDict:BisGearSetDict),
                            Cost=Cost
                        });
                    }
                }
            }

            return Ok(rList);
            }
        }

        [HttpPut("ClaimPlayerDiscord/{discordId}/{playerId}")]
        public async Task<IActionResult> ClaimPlayerDiscord(string discordId,int playerId)
        {
            using (var context = _context.CreateDbContext())
            {

            Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == discordId);
            if (user is null)
                return NotFound("");


            Players? player = await context.Players.FirstOrDefaultAsync(p => p.Id == playerId);
            if (player is null || player.IsClaimed)
                return NotFound("");
            player.IsClaimed = true;
            user.user_claimed_playerId += playerId.ToString()+";";
            Console.WriteLine(user.user_claimed_playerId);
            await context.SaveChangesAsync();
            return Ok();
            }
        }

        [HttpPut("UnclaimPlayerDiscord/{discordId}/{playerId}")]
        public async Task<IActionResult> UnclaimPlayerDiscord(string discordId,int playerId)
        {
            using (var context = _context.CreateDbContext())
            {

            Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == discordId);
            if (user is null)
                return NotFound("");


            Players? player = await context.Players.FirstOrDefaultAsync(p => p.Id == playerId);
            if (player is null)
                return NotFound("");

            player.IsClaimed = false;
            user.removePlayerClaim(playerId.ToString());
            await context.SaveChangesAsync();
            return Ok();
            }
        }

        [HttpGet("GetAllClaimedPlayerDiscord/{discordId}")]
        public async Task<ActionResult<List<ClaimedPlayerInfoDTO>>> GetAllClaimedPlayerDiscord(string discordId)
        {
            using (var context = _context.CreateDbContext())
            {
            Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == discordId);
            if (user is null)
                return NotFound("");

            List<ClaimedPlayerInfoDTO> rList = new List<ClaimedPlayerInfoDTO>();

            foreach (string pId in user.getAllClaimedPlayerId()){
                if (pId == ";" || pId == "")
                    continue;
                int playerId = int.Parse(pId);
                
                Players? player = await context.Players.FirstOrDefaultAsync(p => p.Id ==  playerId);

                if (!(player is null)){
                    Static? dBStatic = await context.Statics.FirstOrDefaultAsync(s => s.Id == player.staticId);
                    if (!(dBStatic is null)){

                        Dictionary<GearType, Gear?> CurrentGearSetDict = player.get_gearset_as_dict(false, context);
                        Dictionary<GearType, Gear?> BisGearSetDict = player.get_gearset_as_dict(true, context);

                        List<CostDTO> Costs = new List<CostDTO>();

                        foreach(KeyValuePair<GearType, Gear?> pair in CurrentGearSetDict){
                            if((pair.Value is null) || BisGearSetDict[pair.Key] is null)
                                continue;
                            Costs.Add(pair.Value.GetCost(BisGearSetDict[pair.Key]!));
                        }

                        CostDTO Cost = CostDTO.SumCost(Costs);

                        rList.Add(new ClaimedPlayerInfoDTO(){
                            Name=player.Name,
                            Job = player.Job.ToString(),
                            pId = player.Id,
                            StaticName=dBStatic.Name,
                            StaticUUID=dBStatic.UUID,
                            CurrentAverageItemLevel=player.get_avg_item_level(GearDict:CurrentGearSetDict),
                            BisAverageItemLevel=player.get_avg_item_level(GearDict:BisGearSetDict),
                            Cost=Cost
                        });
                    }
                }
            }

            return Ok(rList);
            }
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            return Ok();
        }
        
    }
}