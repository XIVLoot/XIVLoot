using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.DTO;
using FFXIV_RaidLootAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FFXIV_RaidLootAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaticController : ControllerBase
    {
        private readonly IDbContextFactory<DataContext> _context;
        
        public StaticController(IDbContextFactory<DataContext> context)
        {
            _context = context;
            
        }
// Get All statics        
        [HttpGet(Name = "GetAllStatics")]
        public async Task<ActionResult<List<Static>>> GetAllStatics()
        {
            using (var context = _context.CreateDbContext())
            {
                List<Static> statics = await context.Statics.ToListAsync();
            
                return Ok(statics);
            }
            
        }
// Get static by uuid        
        [HttpGet("{uuid}")]
        public async Task<ActionResult<StaticDTO>> GetStaticByUUID(string uuid)
        {
            using (var context = _context.CreateDbContext())
            {
                var dbStatic = await context.Statics.FirstAsync(s => s.UUID == uuid);
                var playerList = context.Players.Where(p => p.staticId == dbStatic.Id).ToList();

                StaticDTO aStatic = new StaticDTO
                {
                    Id = dbStatic.Id,
                    Name = dbStatic.Name,
                    UUID = dbStatic.UUID,
                    Players = playerList
                };
                return Ok(aStatic);
            }
            
        }
// Add a new static        
        [HttpPost]
        public async Task<ActionResult<StaticDTO>> AddStatic(string name)
        {
            using (var context = _context.CreateDbContext())
            {
                List<Static> staticList = await context.Statics.ToListAsync();
                            string uuid = Guid.NewGuid().ToString();
                            Static newStatic = new Static
                            {
                                Name = name,
                                UUID = uuid
                            };
                            await context.Statics.AddAsync(newStatic);
                            await context.SaveChangesAsync();
                            var staticId = context.Statics.First(s => s.UUID == uuid).Id;
                            
                            //Add 8 empty players
                            // Creates a player with default gear
                            
                            List<Players> players = new List<Players>();
                            for (int i = 0; i < 8; i++)
                            {
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
                                players.Add(newPlayer);
                            }
                            await context.AddRangeAsync(players);
                            await context.SaveChangesAsync();

                            var currentStatic = await context.Statics.FirstAsync(s => s.UUID == uuid);
                            var currentPlayers = await context.Players.Where(p => p.staticId == currentStatic.Id).ToListAsync();
                            StaticDTO thisStatic = new StaticDTO
                            {
                                Id = currentStatic.Id,
                                Name = currentStatic.Name,
                                UUID = currentStatic.UUID,
                                Players = currentPlayers
                            };
                            
                            return Ok(thisStatic);
            }
            
        }
// Edit a static
        [HttpPut]
        public async Task<ActionResult<Static>> UpdateStatic(AddStaticDTO aStatic)
        {
            using (var context = _context.CreateDbContext())
            {
                var dbStatic = await context.Statics.SingleAsync(s => s.UUID == aStatic.UUID);
                if (dbStatic is null)
                    return NotFound("Static is not found.");
                dbStatic.Name = aStatic.Name;

                await context.SaveChangesAsync();
                return Ok(dbStatic);
            }
            
        }
// Remove a static
        [HttpDelete]
        public async Task<ActionResult<List<Static>>> DeleteStatic(string uuid)
        {
            using (var context = _context.CreateDbContext())
            {
                var dbStatic = await context.Statics.SingleAsync(s => s.UUID == uuid);
                if (dbStatic is null)
                    return NotFound("Static not found");
                var dbPlayers = context.Players.Where(p => p.staticId == dbStatic.Id).ToList();

                foreach (var aPlayer in dbPlayers)
                {
                    context.Players.Remove(aPlayer);
                }
                context.Statics.Remove(dbStatic);
                await context.SaveChangesAsync();
            
                return Ok(await context.Statics.ToListAsync());
            }
            
        }
        
    }
}
