using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.User;
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
            Users newuser = new Users(){
                user_discord_id=user_discord_id,
                user_saved_static=""
            };
            await context.Users.AddAsync(newuser);
            await context.SaveChangesAsync();
            return Ok(newuser);
            }
        }

        [HttpGet("GetUserSavedStatic/{user_discord_id}")]
        public async Task<IActionResult> GetUserSavedStatic(string user_discord_id)
        {
            using (var context = _context.CreateDbContext())
            {
            Users? user = await context.Users.FirstOrDefaultAsync(u => u.user_discord_id == user_discord_id);
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
            Users? user = await context.Users.FirstOrDefaultAsync(u => u.user_discord_id == user_discord_id);
            if (user is null){
                return NotFound("User not found");
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
            Users? user = await context.Users.FirstOrDefaultAsync(u => u.user_discord_id == user_discord_id);
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
    }
}