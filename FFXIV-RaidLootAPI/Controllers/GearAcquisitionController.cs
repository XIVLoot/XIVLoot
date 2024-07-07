using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FFXIV_RaidLootAPI.DTO;
using System.Text.Json;
using Microsoft.AspNetCore.Cors;

namespace FFXIV_RaidLootAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]
    public class GearAcquisitionController : ControllerBase
    {

        private readonly IDbContextFactory<DataContext> _context;

        public GearAcquisitionController(IDbContextFactory<DataContext> context)
        {
            _context = context;
        }

    [HttpDelete("RemoveGearAcquisition/{id}")]
    public async Task<ActionResult> RemoveGearAcquisition(int id){
        using (var context = _context.CreateDbContext())
        {
            GearAcquisitionTimestamp? t = await context.GearAcquisitionTimestamps.FirstOrDefaultAsync(t => t.Id == id);
            if (t is null)
                return NotFound("GearAcqTimestamp not found.");
            
            context.GearAcquisitionTimestamps.Remove(t);
            await context.SaveChangesAsync();
            return Ok("");
        }
    }

    }

}