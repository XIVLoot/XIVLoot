using ffxiRaidLootAPI.DTO;
using ffxiRaidLootAPI.Models;
using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.DTO;
using FFXIV_RaidLootAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FFXIV_RaidLootAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]
    public class PlayerTomePlanController : ControllerBase
    {
        private readonly IDbContextFactory<DataContext> _context;
        private readonly IConfiguration _configuration;
        public PlayerTomePlanController(IDbContextFactory<DataContext> context,IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPut("CreateTomePlan/{playerId}/{order}")]
        public async Task<ActionResult> CreateTomePlan(int playerId, string order)
        {
            using (var context = _context.CreateDbContext())
            {        

                PlayerTomePlan newTomePlan = new PlayerTomePlan(){
                    playerId=playerId,
                    numberStartTomes = 0,
                    numberOffsetTomes = 0,
                    gearPlanOrder = order//"Weapon;Empty;Head;Legs"
                };
                await context.PlayerTomePlans.AddAsync(newTomePlan);
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpPut("AddToTomePlan")]
        public async Task<ActionResult> AddToTomePlan(TomePlanEditDTO tomePlanEditDTO)
        {
            using (var context = _context.CreateDbContext())
            {
                PlayerTomePlan? playerTomePlan = await context.PlayerTomePlans.FirstOrDefaultAsync(p => p.playerId == tomePlanEditDTO.playerId);
                if (playerTomePlan is null)
                    return NotFound();

                if (Enum.TryParse(tomePlanEditDTO.GearToAdd, out GearType gearType))
                {
                    playerTomePlan.AddGearFromWeek(tomePlanEditDTO.weekToEdit, gearType);
                    await context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Invalid gear type.");
                }

            }
        }

        [HttpPut("RemoveFromTomePlan")]
        public async Task<ActionResult> RemoveFromTomePlan(TomePlanEditDTO tomePlanEditDTO)
        {
            using (var context = _context.CreateDbContext())
            {
                PlayerTomePlan? playerTomePlan = await context.PlayerTomePlans.FirstOrDefaultAsync(p => p.playerId == tomePlanEditDTO.playerId);
                if (playerTomePlan is null)
                    return NotFound();

                if (Enum.TryParse(tomePlanEditDTO.GearToRemove, out GearType gearType))
                {
                    playerTomePlan.RemoveGearFromWeek(tomePlanEditDTO.weekToEdit, gearType);
                    await context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Invalid gear type.");
                }

            }
        }

        [HttpPut("SetStartTomes")]
        public async Task<ActionResult> SetStartTomes(TomePlanEditDTO tomePlanEditDTO)
        {
            using (var context = _context.CreateDbContext())
            {
                PlayerTomePlan? playerTomePlan = await context.PlayerTomePlans.FirstOrDefaultAsync(p => p.playerId == tomePlanEditDTO.playerId);
                if (playerTomePlan is null)
                    return NotFound();

                try{
                    playerTomePlan.numberStartTomes = tomePlanEditDTO.numberStartTomes;
                }
                catch(Exception e){
                    playerTomePlan.numberStartTomes = 0;
                    await context.SaveChangesAsync();
                    return Ok();
                }
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpPut("SetOffsetTomes")]
        public async Task<ActionResult> SetOffsetTomes(TomePlanEditDTO tomePlanEditDTO)
        {
            using (var context = _context.CreateDbContext())
            {
                PlayerTomePlan? playerTomePlan = await context.PlayerTomePlans.FirstOrDefaultAsync(p => p.playerId == tomePlanEditDTO.playerId);
                if (playerTomePlan is null)
                    return NotFound();

                try{
                    playerTomePlan.numberOffsetTomes = tomePlanEditDTO.numberOffsetTomes;
                }
                catch(Exception e){
                    playerTomePlan.numberOffsetTomes = 0;
                    await context.SaveChangesAsync();
                    return Ok();
                }
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpPut("AddWeekToTomePlan")]
        public async Task<ActionResult> AddWeekToTomePlan(TomePlanEditDTO tomePlanEditDTO)
        {
            using (var context = _context.CreateDbContext())
            {
                PlayerTomePlan? playerTomePlan = await context.PlayerTomePlans.FirstOrDefaultAsync(p => p.playerId == tomePlanEditDTO.playerId);
                if (playerTomePlan is null)
                    return NotFound();
                playerTomePlan.gearPlanOrder = ";" + playerTomePlan.gearPlanOrder;
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpPut("RemoveWeekFromTomePlan")]
        public async Task<ActionResult> RemoveWeekFromTomePlan(TomePlanEditDTO tomePlanEditDTO)
        {
            using (var context = _context.CreateDbContext())
            {
                PlayerTomePlan? playerTomePlan = await context.PlayerTomePlans.FirstOrDefaultAsync(p => p.playerId == tomePlanEditDTO.playerId);
                if (playerTomePlan is null)
                    return NotFound();
                playerTomePlan.RemoveWeekFromGearPlan(tomePlanEditDTO.weekToEdit);
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpGet("GetPlayerTomePlan/{playerId}")]
        public async Task<ActionResult<PlayerTomePlanDto>> GetPlayerTomePlan(int playerId)
        {
            using (var context = _context.CreateDbContext())
            {
                PlayerTomePlan? playerTomePlan = await context.PlayerTomePlans.FirstOrDefaultAsync(p => p.playerId == playerId);
                Console.WriteLine("After playerTomePlan : " + playerTomePlan);
                if (playerTomePlan is null)
                    return NotFound();
                Tuple<List<GearPlanSingle>,int> tup = playerTomePlan.ComputeGearPlanInfo();
                List<GearPlanSingle> rList = tup.Item1;
                int cost = tup.Item2;
                Console.WriteLine("After rList : " + rList);
                await context.SaveChangesAsync(); // Changing if we added weeks

                return Ok(new PlayerTomePlanDto(){
                    numberWeeks = playerTomePlan.numberWeeks,
                    numberStartTomes = playerTomePlan.numberStartTomes,
                    numberOffsetTomes = playerTomePlan.numberOffsetTomes,
                    totalCost = cost,
                    gearPlanOrder = rList
                });
            }
        }
    }
}