using System.Text.Json;
using Azure.Identity;
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
        private static readonly List<string> ETRO_GEAR_NAME = new List<string> 
        {           
            "weapon",
            "head",
            "body",
            "hands",
            "legs",
            "feet",
            "offHand",
            "ears",
            "neck",
            "wrists",
            "fingerL",
            "fingerR"
                    };

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

            Dictionary<string, Gear?> bisDict = player.get_gearset_as_dict(true,_context);
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

        [HttpGet("GetGearList/{UseBis}")]
        public async Task<ActionResult<GetGearListDTO>> GetGearList(int Id, bool UseBis)
        {
        GetGearListDTO ListGearDTO = new GetGearListDTO();
        Players? player = _context.Players.Find(Id);
        if (player is null)
            return NotFound("Player not found.");

        int i = 0;
        foreach (KeyValuePair<string,Gear?> pair in player.get_gearset_as_dict(UseBis, _context))
        {
            if (pair.Value is null) continue;
            ListGearDTO.ListGearName.Insert(i,pair.Value.Name);
            i+=1;
        }

        return Ok(ListGearDTO);

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

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://etro.gg");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_API_KEY");
                List<int> ListIdGears = new List<int> {0,0,0,0,0,0,0,0,0,0,0,0};
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

                        for (int i=0;i<ETRO_GEAR_NAME.Count;i++)
                            if (!(responseData[ETRO_GEAR_NAME[i]] is null))
                                ListIdGears[i] = Convert.ToInt32(responseData[ETRO_GEAR_NAME[i]].ToString());
                    }
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine("Request error: " + e.Message);
                        return NotFound("Could not find etro gearset.");
                    }

                    foreach (int GearId in ListIdGears)
                    {   
                        string GearILevel;
                        string GearName;
                        if (GearId == 0) 
                        {
                            GearILevel = "665";
                            GearName = "Relic Weapon";
                        }
                        else
                        {
                            try
                            {
                                // Make the GET request
                                HttpResponseMessage response = await client.GetAsync("/api/equipment/"+GearId.ToString()+"/");
                                Console.WriteLine("/api/equipment/"+GearId.ToString()+"/");
                                // Ensure the request was successful
                                response.EnsureSuccessStatusCode();

                                // Read and process the response content
                                string responseBody = await response.Content.ReadAsStringAsync();
                                Dictionary<string, object>? responseData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

                                if (responseData is null)
                                    return NotFound("Could not find gear.");

                                GearILevel = responseData["itemLevel"].ToString();
                                GearName = responseData["name"].ToString();
                                
                            }
                            catch (HttpRequestException e)
                            {
                                Console.WriteLine("Request error: " + e.Message);
                                return NotFound("Could not find gear.");
                            }
                        }

                        // Will check if exists in database
                        Gear? gear =  _context.Gears.FirstOrDefault(g => g.Name == GearName);

                        if (gear is null)
                        {   // Gear does not exist so we create and add.
                            Gear newGear = Gear.CreateGearFromEtro(GearILevel,GearName);
                            await _context.Gears.AddAsync(newGear);
                        }
                        _context.SaveChanges();
                        // Now give gearsId to player.
                        gear = await _context.Gears.SingleAsync(g => g.Name == GearName);

                        switch (gear.GearType)
                        {
                            case GearType.Weapon:
                                player.BisWeaponGearId = gear.Id;break;
                            case GearType.Head:
                                player.BisHeadGearId = gear.Id;break;
                            case GearType.Body:
                                player.BisBodyGearId = gear.Id;break;
                            case GearType.Hands:
                                player.BisHandsGearId = gear.Id;break;
                            case GearType.Legs:
                                player.BisLegsGearId = gear.Id;break;
                            case GearType.Feet:
                                player.BisFeetGearId = gear.Id;break;
                            case GearType.Earrings:
                                player.BisEarringsGearId = gear.Id;break;
                            case GearType.Necklace:
                                player.BisNecklaceGearId = gear.Id;break;
                            case GearType.Bracelets:
                                player.BisBraceletsGearId = gear.Id;break;
                            case GearType.RightRing:
                                player.BisRightRingGearId = gear.Id;break;
                            case GearType.LeftRing:
                                player.BisLeftRingGearId = gear.Id;break;
                        }
                    }
            }
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