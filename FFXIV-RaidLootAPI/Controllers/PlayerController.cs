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
        
        
    }
}