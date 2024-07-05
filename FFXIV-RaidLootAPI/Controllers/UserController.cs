using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.Models;
using FFXIV_RaidLootAPI.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FFXIV_RaidLootAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [Authorize]
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

        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            return Ok();
        }
        
    }
}