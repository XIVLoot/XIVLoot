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

        [HttpGet("GetPlayerTomePlan/{playerId}")]
        public async Task<ActionResult<PlayerTomePlanDto>> GetPlayerTomePlan(int playerId)
        {
            PlayerTomePlan? playerTomePlan = await _context.PlayerTomePlans.FindAsync(playerId);
            if (playerTomePlan is null)
                return NotFound();

            return Ok(new PlayerTomePlanDto(){
                numberWeeks = playerTomePlan.numberWeeks,
                numberStartTomes = playerTomePlan.numberStartTomes,
                numberOffsetTomes = playerTomePlan.numberOffsetTomes,
                gearPlanOrder = playerTomePlan.GetGearPlanOrder()
            });
        }
    }
}