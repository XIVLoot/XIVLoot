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

        // GET

        [HttpGet]
        public async Task<ActionResult<List<Players>>> GetAllPlayers()
        {
            List<Players> playerList = await _context.Players.ToListAsync();
            return Ok(playerList);
        }

        [HttpGet]
        public async Task<ActionResult<int>> GetAverageLevel(int Id, bool UseBis)
        {
            Players? player = _context.Players.Find(Id);
            if (player is null)
                return NotFound("Player is not found.");

            return await player.get_avg_item_level(null,UseBis,_context);
        }

        // POST

        [HttpPost]
        public Task<ActionResult> UpdatePlayerGear(int Id,GearType GearToChange, int NewGearId, bool Usebis)
        {// Changes only one of the gear type. Changes the given GearToChange to NewGearid. if Usebis set to true
         // changes the value for the bis gear set of the player.

        Players? player = _context.Players.Find(Id);
        if (player is null)
            return NotFound("Player not found");
        player.change_gear_piece(GearToChange,Usebis,NewGearId);
        _context.SaveChanges();
        return Ok();
        }

        [HttpPost]
        public Task<ActionResult> UpdatePlayerEtro(int Id,string NewEtro)
        {// Changes only one of the gear type. Changes the given GearToChange to NewGearid. if Usebis set to true
         // changes the value for the bis gear set of the player.

        Players? player = _context.Players.Find(Id);
        if (player is null)
            return NotFound("Player not found");
        player.EtroBiS = NewEtro;
        _context.SaveChanges();
        return Ok();
        }

        [HttpPost]
        public Task<ActionResult> UpdateName(int Id,string NewName)
        {// Changes only one of the gear type. Changes the given GearToChange to NewGearid. if Usebis set to true
         // changes the value for the bis gear set of the player.

        Players? player = _context.Players.Find(Id);
        if (player is null)
            return NotFound("Player not found");
        player.Name = NewName;
        _context.SaveChanges();
        return Ok();
        }

        [HttpPost]
        public Task<ActionResult> UpdateJob(int Id,Job job)
        {// Changes only one of the gear type. Changes the given GearToChange to NewGearid. if Usebis set to true
         // changes the value for the bis gear set of the player.

        Players? player = _context.Players.Find(Id);
        if (player is null)
            return NotFound("Player not found");
        player.Job = job;
        _context.SaveChanges();
        return Ok();
        }

        [HttpPost]
        public Task<ActionResult> UpdateLocked(int Id,bool NewLock)
        {// Changes only one of the gear type. Changes the given GearToChange to NewGearid. if Usebis set to true
         // changes the value for the bis gear set of the player.

        Players? player = _context.Players.Find(Id);
        if (player is null)
            return NotFound("Player not found");
        player.Locked = NewLock;
        _context.SaveChanges();
        return Ok();
        }

    }
}