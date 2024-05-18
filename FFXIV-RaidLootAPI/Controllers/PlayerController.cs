using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FFXIV_RaidLootAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly DataContext _context;

        public PlayerController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<Players>>> GetAllPlayers()
        {
            List<Players> playerList = await _context.Players.ToListAsync();
            return Ok(playerList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Players>> GetPlayById(int id)
        {
            var dbPlayer = await _context.Players.FindAsync(id);
            var dbGears = await _context.Gears.ToListAsync();
            List<Gear> gearSet = new List<Gear>();
            if (dbPlayer is null)
                return NotFound("Player not found");
            foreach (Gear gear in dbGears)
            {
                if (gear.playerId == dbPlayer.Id)
                    gearSet.Add(gear);
            }
            dbPlayer.Gears = gearSet;
            return Ok(dbPlayer);
        }
    }
}