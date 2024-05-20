using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.DTO;
using FFXIV_RaidLootAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FFXIV_RaidLootAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaticController : ControllerBase
    {
        private readonly DataContext _context;
        public StaticController(DataContext context)
        {
            _context = context;
        }
        [HttpGet(Name = "GetAllStatics")]
        public async Task<ActionResult<List<Static>>> GetAllStatics()
        {
            List<Static> statics = await _context.Statics.ToListAsync();
            
            return Ok(statics);
        }
// Add a new static        
        [HttpPost]
        public async Task<ActionResult<List<Static>>> AddStatic(StaticDTO aStatic)
        {
            List<Static> staticList = await _context.Statics.ToListAsync();
            string uuid = Guid.NewGuid().ToString();
            Static newStatic = new Static
            {
                Name = aStatic.Name,
                UUID = uuid
            };
            _context.Statics.Add(newStatic);
            await _context.SaveChangesAsync();
            
            //Add 8 empty players
            Players newPlayer = new Players
            {
                Name = "Enter the new name here",
                Locked = false,
                staticId = _context.Statics.Single(s => s.UUID == uuid).Id
            };
            for (int i = 0; i < 7; i++)
            {
                _context.Players.Add(newPlayer);
            }

            await _context.SaveChangesAsync();
            
            return Ok(await _context.Statics.ToListAsync());
        }
// Edit a static
        [HttpPut]
        public async Task<ActionResult<Static>> UpdateStatic(StaticDTO aStatic)
        {
            var dbStatic = await _context.Statics.SingleAsync(s => s.UUID == aStatic.UUID);
            if (dbStatic is null)
                return NotFound("Static is not found.");
            dbStatic.Name = aStatic.Name;

            await _context.SaveChangesAsync();
            return Ok(dbStatic);
        }
// Remove a static
        [HttpDelete]
        public async Task<ActionResult<List<Static>>> DeleteStatic(string uuid)
        {
            var dbStatic = await _context.Statics.SingleAsync(s => s.UUID == uuid);
            if (dbStatic is null)
                return NotFound("Static not found");

            _context.Statics.Remove(dbStatic);
            await _context.SaveChangesAsync();
            
            return Ok(await _context.Statics.ToListAsync());
        }
        
    }
}
