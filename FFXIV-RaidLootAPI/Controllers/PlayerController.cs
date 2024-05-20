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

        [HttpGet("GetCost/{Id}")]
        public async Task<ActionResult<CostDTO>> GetTotalCost(int Id)
        {
            Players? player = await _context.Players.FindAsync(Id);
            if (player is null)
                return NotFound("Player is not found");

            Dictionary<string, Gear?> bisDict = player.get_gearset_as_dict(false,_context);
            Dictionary<string, Gear?> curDict = player.get_gearset_as_dict(false,_context);
            List<CostDTO> ListCostDTO = new List<CostDTO>(curDict.Count);
            int i = 0;

            foreach (KeyValuePair<string, Gear?> pair in curDict)
            {   
                if (!(pair.Value is null) && !(bisDict[pair.Key] is null))
                {
                ListCostDTO.Insert(i,pair.Value.GetCost(bisDict[pair.Key]));
                }
                i+=1;
            }

            return CostDTO.SumCost(ListCostDTO);

        }



        // POST

                // Create a player function

        [HttpPost]

        public async Task<ActionResult<Players>> CreateNewPlayer(int staticId){
            // Creates a player with default gear
            Players newPlayer = new Players 
            {
                Locked=false,
                staticId=staticId,
                Job=Job.BlackMage,
                BisWeaponGearId=1,
                CurWeaponGearId=1,
                BisHeadGearId=2,
                CurHeadGearId=2,
                BisBodyGearId=3,
                CurBodyGearId=3,
                BisHandsGearId=4,
                CurHandsGearId=4,
                BisLegsGearId=5,
                CurLegsGearId=5,
                BisFeetGearId=6,
                CurFeetGearId=6,
                BisEarringsGearId=7,
                CurEarringsGearId=7,
                BisNecklaceGearId=8,
                CurNecklaceGearId=8,
                BisBraceletsGearId=9,
                CurBraceletsGearId=9,
                BisRightRingGearId=10,
                CurRightRingGearId=10,
                BisLeftRingGearId=11,
                CurLeftRingGearId=11,
            };
            _context.Players.Add(newPlayer);
            _context.SaveChanges();
            return Ok(newPlayer);
        }


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