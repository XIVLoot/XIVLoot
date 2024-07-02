using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.Models;
using FFXIV_RaidLootAPI.User;
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

        public UserController(IDbContextFactory<DataContext> context)
        {
            _context = context;
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
        public async Task<IActionResult> GetUserSavedStatic(string user_discord_id)
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
        public async Task<IActionResult> AddStaticToUserSaved(string user_discord_id, string static_uuid)
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
        public async Task<IActionResult> RemoveStaticToUserSaved(string user_discord_id, string static_uuid)
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
    /*
        [HttpPost("SetUserName/{new_name}")]
        public async Task<IActionResult> GetEmail(string new_name)
        {
            if (!Request.Cookies.TryGetValue("Id", out var user_id))
            {
                return BadRequest("User ID cookie not found");
            }

            using (var context = _context.CreateDbContext())
            {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == user_id);
            if (user is null)
                return NotFound("USer not found");
            user.user_displayed_name = new_name;
            await context.SaveChangesAsync();
            return Ok();
            }
        }

        [HttpGet("GetUserInfo"), Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            using (var context = _context.CreateDbContext())
            {
            ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == "0");
            if (user is null){
                return NotFound("User not found");
            }
            string static_list_as_string = user.user_saved_static;
            List<string> uuidList = static_list_as_string.Split(';').ToList();
            return Ok(new {staticList = uuidList, userName = user.user_displayed_name});
            }
        }
        */
    }
}