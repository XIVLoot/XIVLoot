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
        private readonly DataContext _context;

        public GearController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Gear>>> GetAllGear()
        {
            List<Gear> gearList = await _context.Gears.ToListAsync();
            return Ok(gearList);
        }
    }
}
