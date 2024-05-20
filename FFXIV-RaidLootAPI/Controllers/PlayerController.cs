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

        [HttpGet]
        public async Task<ActionResult<List<Players>>> GetAllPlayers()
        {
            List<Players> playerList = await _context.Players.ToListAsync();
            return Ok(playerList);
        }

        [HttpGet]
        public async Task<ActionResult<int>> GetAverageLevel(PlayerDTO dto)
        {
            Players? player = await _context.Players.FindAsync(dto.Id);
            if (player is null)
                return NotFound("Player is not found.");

            return player.get_avg_item_level(null,dto.UseBis,_context);
        }

        // POST

        [HttpPut]
        public async Task<ActionResult> UpdatePlayerGear(PlayerDTO dto)
        {// Changes only one of the gear type. Changes the given GearToChange to NewGearid. if Usebis set to true
         // changes the value for the bis gear set of the player.

        Players? player = await _context.Players.FindAsync(dto.Id);
        if (player is null)
            return NotFound("Player not found");
        player.change_gear_piece(dto.GearToChange,dto.UseBis,dto.NewGearId);
        _context.SaveChanges();
        return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> UpdatePlayerEtro(PlayerDTO dto)
        {// Changes only one of the gear type. Changes the given GearToChange to NewGearid. if Usebis set to true
         // changes the value for the bis gear set of the player.

        Players? player = await _context.Players.FindAsync(dto.Id);
        if (player is null)
            return NotFound("Player not found");
        player.EtroBiS = dto.NewEtro;
        _context.SaveChanges();
        return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateName(PlayerDTO dto)
        {// Changes only one of the gear type. Changes the given GearToChange to NewGearid. if Usebis set to true
         // changes the value for the bis gear set of the player.

        Players? player = await _context.Players.FindAsync(dto.Id);
        if (player is null)
            return NotFound("Player not found");
        player.Name = dto.NewName;
        _context.SaveChanges();
        return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateJob(PlayerDTO dto)
        {// Changes only one of the gear type. Changes the given GearToChange to NewGearid. if Usebis set to true
         // changes the value for the bis gear set of the player.

        Players? player = await _context.Players.FindAsync(dto.Id);
        if (player is null)
            return NotFound("Player not found");
        player.Job = dto.NewJob;
        _context.SaveChanges();
        return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateLocked(PlayerDTO dto)
        {// Changes only one of the gear type. Changes the given GearToChange to NewGearid. if Usebis set to true
         // changes the value for the bis gear set of the player.

        Players? player = await _context.Players.FindAsync(dto.Id);
        if (player is null)
            return NotFound("Player not found");
        player.Locked = dto.NewLock;
        _context.SaveChanges();
        return Ok();
        }

    }
}