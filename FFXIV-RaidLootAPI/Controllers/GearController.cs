using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FFXIV_RaidLootAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GearController : ControllerBase
    {
        private readonly IDbContextFactory<DataContext> _context;

        public GearController(IDbContextFactory<DataContext> context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Gear>>> GetAllGear()
        {
            using (var context = _context.CreateDbContext())
            {
                List<Gear> gearList = await context.Gears.ToListAsync();
                return Ok(gearList);
            }
            
        }
    }
}
