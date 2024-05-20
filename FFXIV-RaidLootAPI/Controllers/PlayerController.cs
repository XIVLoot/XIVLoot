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
        private readonly DataContext _context;

        public PlayerController(DataContext context)
        {
            _context = context;
        }
        // GET

        [HttpGet("{Id}/{UseBis}")]
        public async Task<ActionResult<int>> GetAverageLevel(int Id, bool UseBis)
        {
            Players? player = await _context.Players.FindAsync(Id);
            if (player is null)
                return NotFound("Player is not found.");

            int AvgLvl = player.get_avg_item_level(null,UseBis,_context);
            if (AvgLvl == -1){return NotFound("A context was not provided when it was needed");}
            return AvgLvl;
        }

        // POST

        [HttpPut("GearToChange")]
        public async Task<ActionResult> UpdatePlayerGear(PlayerDTO dto)
        {

        Players? player = await _context.Players.FindAsync(dto.Id);
        if (player is null)
            return NotFound("Player not found");
        player.change_gear_piece(dto.GearToChange,dto.UseBis,dto.NewGearId);
        _context.SaveChanges();
        return Ok();
        }

        [HttpPut("NewEtro")]
        public async Task<ActionResult> UpdatePlayerEtro(PlayerDTO dto)
        {

        Players? player = await _context.Players.FindAsync(dto.Id);
        if (player is null)
            return NotFound("Player not found");
        player.EtroBiS = dto.NewEtro;
        _context.SaveChanges();
        return Ok();
        }

        [HttpPut("NewName")]
        public async Task<ActionResult> UpdateName(PlayerDTO dto)
        {

        Players? player = await _context.Players.FindAsync(dto.Id);
        if (player is null)
            return NotFound("Player not found");
        player.Name = dto.NewName;
        _context.SaveChanges();
        return Ok();
        }

        [HttpPut("NewJob")]
        public async Task<ActionResult> UpdateJob(PlayerDTO dto)
        {

        Players? player = await _context.Players.FindAsync(dto.Id);
        if (player is null)
            return NotFound("Player not found");
        player.Job = dto.NewJob;
        _context.SaveChanges();
        return Ok();
        }

        [HttpPut("NewLock")]
        public async Task<ActionResult> UpdateLocked(PlayerDTO dto)
        {

        Players? player = await _context.Players.FindAsync(dto.Id);
        if (player is null)
            return NotFound("Player not found");
        player.Locked = dto.NewLock;
        _context.SaveChanges();
        return Ok();
        }

    }
}