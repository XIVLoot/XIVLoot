using ffxiRaidLootAPI.DTO;
using ffxiRaidLootAPI.Models;
using FFXIV_RaidLootAPI.Data;
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

        [HttpPut("CreateTomePlan/{playerId}")]
        public async Task<ActionResult> CreateTomePlan(int playerId)
        {
            using (var context = _context.CreateDbContext())
            {        

                PlayerTomePlan newTomePlan = new PlayerTomePlan(){
                    playerId=playerId,
                    numberStartTomes = 0,
                    numberOffsetTomes = 0,
                    gearPlanOrder = "Earrings;Weapon;Empty;Head;Legs;"
                };
                await context.PlayerTomePlans.AddAsync(newTomePlan);
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

                List<GearPlanSingle> rList = playerTomePlan.ComputeGearPlanInfo();
                Console.WriteLine("After rList : " + rList);

                return Ok(new PlayerTomePlanDto(){
                    numberWeeks = playerTomePlan.numberWeeks,
                    numberStartTomes = playerTomePlan.numberStartTomes,
                    numberOffsetTomes = playerTomePlan.numberOffsetTomes,
                    gearPlanOrder = rList
                });
            }
        }
    }
}