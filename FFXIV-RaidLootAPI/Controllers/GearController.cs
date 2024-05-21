using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FFXIV_RaidLootAPI.DTO;

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
        [HttpGet("GetGearOption/{Job}/{GearType}")]
        public async Task<ActionResult<GearOptionsDTO>> GetGearOption(Job Job, GearType GearType)
        {
            /*Returns a list of GearOptionsDTO which is a list of GearOption. Each gear options
              has the gear name, the gear ilevel and the gear stage (raid/augmented/crafted/tome)
              Job -> Job to request the gear for 
              GearType -> What gear piece to request (ie. Ring, Weapon, etc.)
            */
        using (var context = _context.CreateDbContext())
        {
            List<GearOptionsDTO.GearOption> OptionList = new List<GearOptionsDTO.GearOption>();
            if (GearType == GearType.Weapon)
            {
                IEnumerable<Gear> GearIterFromDb = context.Gears.Where(g => g.GearWeaponCategory == Job && g.GearCategory == GearCategory.Weapon);
                foreach (Gear gear in GearIterFromDb)
                {
                    OptionList.Add(new GearOptionsDTO.GearOption()
                    {
                        GearName = gear.Name,
                        GearItemLevel = gear.GearLevel,
                        GearStage = gear.GearStage.ToString(),
                        GearId = gear.Id
                    });
                }
            }
            else
            {
                GearCategory GearToChooseFrom = Gear.JOB_TO_GEAR_CATEGORY_MAP[Job][(int) GearType >=7 ? 1 : 0];
                // Left side is index 0 right side is index 1
                IEnumerable<Gear> GearIterFromDb = context.Gears.Where(g => g.GearCategory == GearToChooseFrom && g.GearType == GearType);
                foreach (Gear gear in GearIterFromDb)
                {
                    OptionList.Add(new GearOptionsDTO.GearOption()
                    {
                        GearName = gear.Name,
                        GearItemLevel = gear.GearLevel,
                        GearStage = gear.GearStage.ToString(),
                        GearId = gear.Id
                    });
                }

            }
            return Ok(new GearOptionsDTO() 
            {
                GearOptionList=OptionList
            });
        }
        }
    }
}
