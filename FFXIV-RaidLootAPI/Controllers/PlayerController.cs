using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.DTO;
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
        private readonly IDbContextFactory<DataContext> _context;

        public PlayerController(IDbContextFactory<DataContext> context)
        {
            _context = context;
        }
        // GET

        [HttpGet("{Id}/{UseBis}")]
        public async Task<ActionResult<int>> GetAverageLevel(int Id, bool UseBis)
        {
            using (var context = _context.CreateDbContext())
            {
                Players? player = await context.Players.FindAsync(Id);
                if (player is null)
                    return NotFound("Player is not found.");

                int AvgLvl = player.get_avg_item_level(null,UseBis,context);
                if (AvgLvl == -1){return NotFound("A context was not provided when it was needed");}
                return AvgLvl;
            }
            
        }

        // POST

        [HttpPut("GearToChange")]
        public async Task<ActionResult> UpdatePlayerGear(PlayerDTO dto)
        {
            using (var context = _context.CreateDbContext())
            {
                Players? player = await context.Players.FindAsync(dto.Id);
                if (player is null)
                    return NotFound("Player not found");
                player.change_gear_piece(dto.GearToChange, dto.UseBis, dto.NewGearId);
                context.SaveChanges();
                return Ok();
            }
        }

        [HttpPut("NewEtro")]
            public async Task<ActionResult> UpdatePlayerEtro(PlayerDTO dto)
            {
                using (var context = _context.CreateDbContext())
                {
                    Players? player = await context.Players.FindAsync(dto.Id);
                    if (player is null)
                        return NotFound("Player not found");
                    player.EtroBiS = dto.NewEtro;
                    context.SaveChanges();
                    return Ok();
                }
            }



        [HttpPut("NewName")]
        public async Task<ActionResult> UpdateName(PlayerDTO dto)
        {
            using (var context = _context.CreateDbContext())
            {
                Players? player = await context.Players.FindAsync(dto.Id);
                if (player is null)
                    return NotFound("Player not found");
                player.Name = dto.NewName;
                context.SaveChanges();
                return Ok();
            }
        }

        [HttpPut("NewJob")]
            public async Task<ActionResult> UpdateJob(PlayerDTO dto)
            {
                using (var context = _context.CreateDbContext())
                {
                    Players? player = await context.Players.FindAsync(dto.Id);
                    if (player is null)
                        return NotFound("Player not found");
                    player.Job = dto.NewJob;
                    context.SaveChanges();
                    return Ok();
                }
                
            }
        
        

        [HttpPut("NewLock")]
        public async Task<ActionResult> UpdateLocked(PlayerDTO dto)
        {

            using (var context = _context.CreateDbContext())
            {
                Players? player = await context.Players.FindAsync(dto.Id);
                if (player is null)
                    return NotFound("Player not found");
                player.Locked = dto.NewLock;
                context.SaveChanges();
                return Ok();
            }
        }

    }
}