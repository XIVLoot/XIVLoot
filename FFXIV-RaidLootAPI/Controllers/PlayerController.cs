using System.Text.Json;
using Azure.Identity;
using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.DTO;
using FFXIV_RaidLootAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FFXIV_RaidLootAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IDbContextFactory<DataContext> _context;
        private static readonly List<string> ETRO_GEAR_NAME = new List<string> 
        {           
            "weapon",
            "head",
            "body",
            "hands",
            "legs",
            "feet",
            //"offHand", TODO CONSIDER HAVING IT
            "ears",
            "neck",
            "wrists",
            "fingerL",
            "fingerR"
                    };

        public PlayerController(IDbContextFactory<DataContext> context)
        {
            _context = context;
        }
        // GET

        [HttpGet("ResetJobDependantValues/{Id}")]
        public async Task<ActionResult> ResetJobDependantValues(int Id){
            using (var context = _context.CreateDbContext())
            {
                Players? player = await context.Players.FindAsync(Id);
                if (player is null)
                    return NotFound("Player is not found.");
                player.ResetJobDependantValues();
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpGet("{Id}/{UseBis}")]
        public async Task<ActionResult<int>> GetAverageLevel(int Id, bool UseBis)
        {
            using (var context = _context.CreateDbContext())
            {
                Players? player = await context.Players.FindAsync(Id);
                if (player is null)
                    return NotFound("Player is not found.");

                int AvgLvl = player.get_avg_item_level(null,UseBis,context);
                if (AvgLvl == -1){return NotFound("A context was not provided when it was needed");}
                return AvgLvl;
            }
        }

        [HttpGet("GetGearList/{UseBis}")]
        public async Task<ActionResult<GetGearListDTO>> GetGearList(int Id, bool UseBis)
        {
        using (var context = _context.CreateDbContext())
        {
            GetGearListDTO ListGearDTO = new GetGearListDTO();
            Players? player = context.Players.Find(Id);
            if (player is null)
                return NotFound("Player not found.");

            int i = 0;
            foreach (KeyValuePair<GearType,Gear?> pair in player.get_gearset_as_dict(UseBis, context))
            {
                if (pair.Value is null) continue;
                ListGearDTO.ListGearName.Insert(i,pair.Value.Name);
                i+=1;
            }

            return Ok(ListGearDTO);
        }
        }

        [HttpGet("GetSingletonPlayerInfo/{uuid}/{id}")]
        public async Task<ActionResult<PlayerDTO>> GetSingletonPlayerInfo(string uuid, int id){
            using (var context = _context.CreateDbContext())
            {
                Players? player = await context.Players.FindAsync(id);
                if (player is null)
                    return NotFound("Player not found.");
                var dbStatic = await context.Statics.FirstAsync(s => s.UUID == uuid);

                return Ok(player.get_player_info(context,dbStatic));

            }
        }

        // POST

        [HttpPost("ResetJobDependantInfo")]
        public async Task<ActionResult> ResetJobDependantInfo(PlayerDTO dto){
            using (var context = _context.CreateDbContext())
            {
                Players? player = await context.Players.FindAsync(dto.Id);
                if (player is null)
                    return NotFound("Player not found.");
                player.ResetJobDependantValues();
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpPut("GearToChange")]
        public async Task<ActionResult> UpdatePlayerGear(PlayerDTO dto)
        {
        using (var context = _context.CreateDbContext())
        {
            Players? player = await context.Players.FindAsync(dto.Id);
            if (player is null)
                return NotFound("Player not found");
            await player.change_gear_piece(dto.GearToChange, dto.UseBis, dto.NewGearId, Turn.turn_1, context);

            await context.SaveChangesAsync();
            return Ok();
        }
        }

        [HttpPut("RemovePlayerLock/{turn}")]
        public async Task<ActionResult> RemovePlayerLock(PlayerDTO dto, Turn turn)
        {
        using (var context = _context.CreateDbContext())
        {
            Players? player = await context.Players.FindAsync(dto.Id);
            if (player is null)
                return NotFound("Player not found");
            player.remove_lock(turn);

            await context.SaveChangesAsync();
            return Ok();
        }
        }

        [HttpPut("NewEtro")]
        public async Task<ActionResult> UpdatePlayerEtro(PlayerDTO dto)
        {
        using (var context = _context.CreateDbContext())
        {
            Players? player = await context.Players.FindAsync(dto.Id);
            if (player is null)
                return NotFound("Player not found");
            if (dto.UseBis)
                player.EtroBiS = dto.NewEtro;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://etro.gg");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_API_KEY");
                List<int> ListIdGears = new List<int> {0,0,0,0,0,0,0,0,0,0,0};
                int IdLeftRing = 0;
                int IdRightRing = 0;
                    try
                    {
                        // Make the GET request
                        HttpResponseMessage response = await client.GetAsync("/api/gearsets/"+dto.NewEtro); // Replace 'endpoint' with the actual endpoint you want to access

                        // Ensure the request was successful
                        response.EnsureSuccessStatusCode();

                        // Read and process the response content
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Dictionary<string, object>? responseData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

                        if (responseData is null)
                            return NotFound("Could not find etro gearset.");

                        for (int i=0;i<ETRO_GEAR_NAME.Count;i++){
                            if (!(responseData[ETRO_GEAR_NAME[i]] is null))
                                ListIdGears[i] = Convert.ToInt32(responseData[ETRO_GEAR_NAME[i]].ToString());
                            if (ETRO_GEAR_NAME[i] == "fingerL")
                                IdLeftRing = Convert.ToInt32(responseData[ETRO_GEAR_NAME[i]].ToString());
                            else if (ETRO_GEAR_NAME[i] == "fingerR")
                                IdRightRing = Convert.ToInt32(responseData[ETRO_GEAR_NAME[i]].ToString());
                        }
                    }
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine("Request error: " + e.Message);
                        return NotFound("Could not find etro gearset.");
                    }

                    bool AllGearPieceFound = true;

                    // Assume the the order as ETRO_GEAR_NAME

                    for (int i = 0;i<ListIdGears.Count;i++)
                    {
                        Gear? gear;
                        if (ListIdGears[i] == IdRightRing)
                            gear = context.Gears.FirstOrDefault(g => g.EtroGearId == ListIdGears[i] && g.GearType == GearType.RightRing);
                        else if (ListIdGears[i] == IdLeftRing)
                            gear = context.Gears.FirstOrDefault(g => g.EtroGearId == ListIdGears[i] && g.GearType == GearType.LeftRing);
                        else
                            gear = context.Gears.FirstOrDefault(g => g.EtroGearId == ListIdGears[i]);

                        int id = 1;
                        if (gear is null)
                            {
                            AllGearPieceFound=false;
                            continue;
                            }
                        else 
                            id = gear.Id;
                        if (dto.UseBis)
                        {
                            switch (gear.GearType)
                            {
                                case GearType.Weapon:
                                    player.BisWeaponGearId = id;break;
                                case GearType.Head:
                                    player.BisHeadGearId = id;break;
                                case GearType.Body:
                                    player.BisBodyGearId = id;break;
                                case GearType.Hands:
                                    player.BisHandsGearId = id;break;
                                case GearType.Legs:
                                    player.BisLegsGearId = id;break;
                                case GearType.Feet:
                                    player.BisFeetGearId = id;break;
                                case GearType.Earrings:
                                    player.BisEarringsGearId = id;break;
                                case GearType.Necklace:
                                    player.BisNecklaceGearId = id;break;
                                case GearType.Bracelets:
                                    player.BisBraceletsGearId = id;break;
                                case GearType.RightRing:
                                    player.BisRightRingGearId = id;break;
                                case GearType.LeftRing:
                                    player.BisLeftRingGearId = id;break;
                            }
                        } else
                        {
                            switch (gear.GearType)
                            {
                                case GearType.Weapon:
                                    player.CurWeaponGearId = id;break;
                                case GearType.Head:
                                    player.CurHeadGearId = id;break;
                                case GearType.Body:
                                    player.CurBodyGearId = id;break;
                                case GearType.Hands:
                                    player.CurHandsGearId = id;break;
                                case GearType.Legs:
                                    player.CurLegsGearId = id;break;
                                case GearType.Feet:
                                    player.CurFeetGearId = id;break;
                                case GearType.Earrings:
                                    player.CurEarringsGearId = id;break;
                                case GearType.Necklace:
                                    player.CurNecklaceGearId = id;break;
                                case GearType.Bracelets:
                                    player.CurBraceletsGearId = id;break;
                                case GearType.RightRing:
                                    player.CurRightRingGearId = id;break;
                                case GearType.LeftRing:
                                    player.CurLeftRingGearId = id;break;
                            }
                        }
                    }
                await context.SaveChangesAsync();
                return AllGearPieceFound ? Ok() : Ok("Could not find at least one gear piece.");
            }
        
        }
        }


        [HttpPut("NewName")]
        public async Task<ActionResult> UpdateName(PlayerDTO dto)
        {
        using (var context = _context.CreateDbContext())
        {
            Players? player = await context.Players.FindAsync(dto.Id);
            if (player is null)
                return NotFound("Player not found");
            player.Name = dto.NewName;
            context.SaveChanges();
            return Ok();
        }
        }

        [HttpPut("NewJob")]
            public async Task<ActionResult> UpdateJob(PlayerDTO dto)
            {
            using (var context = _context.CreateDbContext())
            {
                Players? player = await context.Players.FindAsync(dto.Id);
                if (player is null)
                    return NotFound("Player not found");
                player.Job = dto.NewJob;
                context.SaveChanges();
                return Ok();
            }
            }
        
        

        [HttpPut("NewLock")]
        public async Task<ActionResult> UpdateLocked(PlayerDTO dto)
        {
        using (var context = _context.CreateDbContext())
        {
            Players? player = await context.Players.FindAsync(dto.Id);
            if (player is null)
                return NotFound("Player not found");
            player.Locked = dto.NewLock;
            context.SaveChanges();
            return Ok();
        }
        }

    }
}