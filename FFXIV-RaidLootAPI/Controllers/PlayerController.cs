using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Azure.Identity;
using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.DTO;
using FFXIV_RaidLootAPI.Models;
using FFXIV_RaidLootAPI.User;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;


namespace FFXIV_RaidLootAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]
    public class PlayerController : ControllerBase
    {
        private readonly IDbContextFactory<DataContext> _context;
        private readonly IConfiguration _configuration;
        private readonly string _jwtKey;
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

        async private Task<bool> UserIsAuthorized(HttpContext HttpContext, string playerId, DataContext context){
            Console.WriteLine("Checking authorization");
            if (HttpContext.Request.Cookies.TryGetValue("jwt_xivloot", out var jwt)){
                Console.WriteLine("Discord : " + jwt.ToString());
                // Logged in discord
                // Decode the JWT to get the access_token
                var handler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtKey);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken validatedToken;
                var principal = handler.ValidateToken(jwt, tokenValidationParameters, out validatedToken);

                var accessToken = principal.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;

                if (string.IsNullOrEmpty(accessToken))
                {
                    return false;
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await client.GetAsync("https://discord.com/api/users/@me");

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Error fetching user info: " + errorContent);
                        throw new HttpRequestException($"Error fetching user info: {response.StatusCode}, Content: {errorContent}");
                    }

                        // Deserialize the response content into a dictionary
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Dictionary<string, object> responseData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody)!;

                        // Access the 'id' value from the dictionary
                        string discordId = responseData["id"].ToString()!;

                        Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == discordId);
                        if (user is null)
                            return false;
                        return user.UserClaimedPlayer(playerId);
                }
            } 
            else if (!(User is null)){
                Console.WriteLine("DEFAUTL CONNECTED");
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
                var userId = userIdClaim?.Value;

                ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user is null)
                    return false;

                return user.UserClaimedPlayer(playerId);
            }

            return false;
        }

        public PlayerController(IDbContextFactory<DataContext> context,IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
            _jwtKey = _configuration["JwtSettings:Key"];
        }
        // GET

        [HttpGet("ResetJobDependantValues/{Id}")]
        public async Task<ActionResult> ResetJobDependantValues(int Id){
            using (var context = _context.CreateDbContext())
            {
                Players? player = await context.Players.FindAsync(Id);
                if (player is null)
                    return NotFound("Player is not found.");
                if (player.IsClaimed)
                {   
                    bool isAuthorized = await UserIsAuthorized(HttpContext, player.Id.ToString(), context);
                    if(!isAuthorized)
                        return Unauthorized("Not Authorized");
                };
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

        // Returns less info than GetSingletonPlayerInfo
        [HttpGet("GetSingletonPlayerInfoSoft/{id}")]
        public async Task<ActionResult<StaticDTO.PlayerInfoSoftDTO>> GetSingletonPlayerInfoSoft(int id){
            using (var context = _context.CreateDbContext())
            {
                Players? player = await context.Players.FindAsync(id);
                if (player is null)
                    return NotFound("Player not found.");

                return Ok(player.get_player_info_soft(context));

            }
        }

        // POST

        [HttpPost("ResetJobDependantInfo")]
        public async Task<ActionResult> ResetJobDependantInfo(PlayerDTO dto){
            using (var context = _context.CreateDbContext())
            {
                Players? player = await context.Players.FindAsync(dto.Id);
                if (player is null)
                    return NotFound();

                if (player.IsClaimed)
                {   
                    bool isAuthorized = await UserIsAuthorized(HttpContext, player.Id.ToString(), context);
                    if(!isAuthorized)
                        return Unauthorized("Not Authorized");
                };
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

            Console.WriteLine("Playeris FOUND");

            if (player.IsClaimed)
            {   
                bool isAuthorized = await UserIsAuthorized(HttpContext, player.Id.ToString(), context);
                if(!isAuthorized)
                    return Unauthorized("Not Authorized");
            }
             

            Console.WriteLine("Turn : " + dto.turn.ToString() + " CheckLock : " + dto.CheckLockPlayer.ToString());
            await player.change_gear_piece(dto.GearToChange, dto.UseBis, dto.NewGearId, dto.turn, dto.CheckLockPlayer, context);

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

            if (player.IsClaimed)
            {   
                bool isAuthorized = await UserIsAuthorized(HttpContext, player.Id.ToString(), context);
                if(!isAuthorized)
                    return Unauthorized("Not Authorized");
            }

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

            if (player.IsClaimed)
            {   
                bool isAuthorized = await UserIsAuthorized(HttpContext, player.Id.ToString(), context);
                if(!isAuthorized)
                    return Unauthorized("Not Authorized");
            }

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

            if (player.IsClaimed)
            {   
                bool isAuthorized = await UserIsAuthorized(HttpContext, player.Id.ToString(), context);
                if(!isAuthorized)
                    return Unauthorized("Not Authorized");
            }

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

            if (player.IsClaimed)
            {   
                bool isAuthorized = await UserIsAuthorized(HttpContext, player.Id.ToString(), context);
                if(!isAuthorized)
                    return Unauthorized("Not Authorized");
            }

                player.Job = dto.NewJob;
                context.SaveChanges();
                return Ok();
            }
            }
        

    }
}