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


        async public static Task<string> GetUserDiscordIdFromJwt(string jwt, string _jwtKey){
            //Console.WriteLine("Discord : " + jwt.ToString());
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
                    return string.Empty;
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await client.GetAsync("https://discord.com/api/users/@me");

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine("Error fetching user info: " + errorContent);
                        throw new HttpRequestException($"Error fetching user info: {response.StatusCode}, Content: {errorContent}");
                    }

                        // Deserialize the response content into a dictionary
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Dictionary<string, object> responseData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody)!;

                        // Access the 'id' value from the dictionary
                        string discordId = responseData["id"].ToString()!;
                        return discordId;
                }
        }


        async private Task<bool> UserHasClaimedPlayerFromSameStatic<T>(T user, string playerId, DataContext context) where T : IUserInterface{
            // Now checks if this user claimed a player from the static. In which case they can edit this player.
            Players? player = await context.Players.FirstOrDefaultAsync(p => p.Id == int.Parse(playerId));
            if (player is null)
                return false;

            IEnumerable<Players> validPlayers = context.Players.Where(p => p.staticId == player.staticId);
            foreach (Players playerToInspect in validPlayers){
                if (user.UserClaimedPlayer(playerToInspect.Id.ToString()))
                    return true;
            }
            return false;
        }

        async private Task<bool> UserIsAuthorized(HttpContext HttpContext, string playerId, DataContext context){
            //Console.WriteLine("Checking authorization");
            if (HttpContext.Request.Cookies.TryGetValue("jwt_xivloot", out var jwt)){
                
                string discordId = await GetUserDiscordIdFromJwt(jwt, _jwtKey);

                Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == discordId);
                if (user is null)
                    return false;
                if (user.UserClaimedPlayer(playerId))
                    return true;

                return await UserHasClaimedPlayerFromSameStatic<Users>(user,playerId,context);

            } 
            else if (!(User is null)){
                //Console.WriteLine("DEFAUTL CONNECTED");
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
                var userId = userIdClaim?.Value;

                Console.WriteLine("USER ID FROM EMAIL IS : " + userId);

                ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user is null)
                    return false;

                bool thisUserClaimed = user.UserClaimedPlayer(playerId);
                if (thisUserClaimed)
                    return true;

                return await UserHasClaimedPlayerFromSameStatic<ApplicationUser>(user,playerId,context);
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

            //Console.WriteLine("Playeris FOUND");

            if (player.IsClaimed)
            {   
                bool isAuthorized = await UserIsAuthorized(HttpContext, player.Id.ToString(), context);
                if(!isAuthorized)
                    return Unauthorized("Not Authorized");
            }
             

            ////Console.WriteLine("Turn : " + dto.turn.ToString() + " CheckLock : " + dto.CheckLockPlayer.ToString());

            if (dto.IsFromBook)
                dto.CheckLockPlayer=false;

            await player.change_gear_piece(dto.GearToChange, dto.UseBis, dto.NewGearId, dto.turn, dto.CheckLockPlayer, dto.IsFromBook, context);

            await context.SaveChangesAsync();
            return Ok();
        }
        }

        [HttpPut("FreePlayer/{uuid}/{playerId}")]
        public async Task<IActionResult> FreePlayer(string uuid, string playerId)
        {
            using (var context = _context.CreateDbContext())
            {
                Static? dbStatic = await context.Statics.FirstOrDefaultAsync(s => s.UUID == uuid);
                if (dbStatic is null)
                    return NotFound();

                // Check if can actually do it in case the user was evil):
                if(HttpContext.Request.Cookies.TryGetValue("jwt_xivloot", out var jwt)) // Discord user
                {
                    string discordId = await GetUserDiscordIdFromJwt(jwt, _jwtKey);

                    Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == discordId);
                    if (!(user is null) && user.Id.ToString() != dbStatic.ownerIdString)
                        return Unauthorized();


                    // Looking for owner
                    ApplicationUser? userWhoClaimed = await context.Users.FirstOrDefaultAsync(u => EF.Functions.Like(u.user_claimed_playerId, $"%;{playerId};%") || EF.Functions.Like(u.user_claimed_playerId, $"{playerId};%"));

                    Users? userWhoClaimedDiscord = null;

                    if (userWhoClaimed is null)
                        userWhoClaimedDiscord = await context.User.FirstOrDefaultAsync(u => EF.Functions.Like(u.user_claimed_playerId, $"%;{playerId};%") || EF.Functions.Like(u.user_claimed_playerId, $"{playerId};%"));

                    if (!(userWhoClaimed is null))
                        userWhoClaimed.removePlayerClaim(playerId);
                    else if (!(userWhoClaimedDiscord is null))
                        userWhoClaimedDiscord.removePlayerClaim(playerId);

                    Players? player = await context.Players.FirstOrDefaultAsync(p => p.Id.ToString() == playerId);
                    if (!(player is null))
                        player.IsClaimed = false;
                    
                } 
                    else if (!(User is null)) // Email user. Note - email user id are uuid while discord userid are integer id
                    {
                    var claimsIdentity = User.Identity as ClaimsIdentity;
                    var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

                    if (userIdClaim != null && userIdClaim.Value != dbStatic.ownerIdString)
                        return Unauthorized();

                    if(userIdClaim is null)
                        return NotFound();

                    // Looking for owner
                    ApplicationUser? userWhoClaimed = await context.Users.FirstOrDefaultAsync(u => EF.Functions.Like(u.user_claimed_playerId, $"%;{playerId};%") || EF.Functions.Like(u.user_claimed_playerId, $"{playerId};%"));

                    Users? userWhoClaimedDiscord = null;

                    if (userWhoClaimed is null)
                        userWhoClaimedDiscord = await context.User.FirstOrDefaultAsync(u => EF.Functions.Like(u.user_claimed_playerId, $"%;{playerId};%") || EF.Functions.Like(u.user_claimed_playerId, $"{playerId};%"));

                    if (!(userWhoClaimed is null))
                        userWhoClaimed.removePlayerClaim(playerId);
                    else if (!(userWhoClaimedDiscord is null))
                        userWhoClaimedDiscord.removePlayerClaim(playerId);

                    Players? player = await context.Players.FirstOrDefaultAsync(p => p.Id.ToString() == playerId);
                    if (!(player is null))
                        player.IsClaimed = false;
                    } 
                    else
                        return Unauthorized(); // Has to be a logged in user

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


        [HttpPut("ImportXIVGear/{setNumber}")]
        public async Task<ActionResult> ImportXIVGear(PlayerDTO dto, int setNumber)
        {
            using (var context = _context.CreateDbContext())
            {
                Players? player = await context.Players.FindAsync(dto.Id);
                if (player is null)
                    return NotFound("Player not found");
                Console.WriteLine("HERE");

                if (player.IsClaimed)
                {   
                    bool isAuthorized = await UserIsAuthorized(HttpContext, player.Id.ToString(), context);
                    if(!isAuthorized)
                        return Unauthorized("Not Authorized");
                }
                 Console.WriteLine("HERE2");

                if (dto.UseBis)
                    player.EtroBiS = dto.NewEtro;
                 Console.WriteLine("HERE3");

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.xivgear.app/shortlink/");
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    //Console.WriteLine("HERE4");
                    Object items = new object();
                    //Console.WriteLine("HERE5");

                    // Extracting uuid from link

                   /* https://xivgear.app/?page=sl%7C ac66ee64-8ecd-4382-a52e-2a179f4f991d */
                   /* or https://xivgear.app/?page=sl%7C8f2e2ba8-082b-4fd4-912b-a7851ec3e50c&onlySetIndex=6 */
                    //Console.WriteLine(dto.NewEtro);
                    //Console.WriteLine(dto.NewEtro.Split("https://xivgear.app/?page=sl|")[0]);
                    
                    List<string> firstTry = dto.NewEtro.Split("xivgear.app/?page=sl|").ToList();
                    List<string> secondTry = dto.NewEtro.Split("xivgear.app/?page=sl%7C").ToList();
                    string uuid = firstTry.Count > 1 ? firstTry[1] : secondTry[1];

                    // XIVGear url sometime specify a page index. 
                    // In order to avoir error where the url contains additional information
                    // we will only take the first 36 characters of the uuid which should
                    // always correspond to the actual uuid of the gearset.
                    if (uuid.Length >= 36)
                    {
                        uuid = uuid.Substring(0, 36);
                    }

                    //Console.WriteLine("HERE6");
                    Console.WriteLine("Detected uuid : " + uuid);

                    try{

                        // Make the GET request
                        HttpResponseMessage response = await client.GetAsync(uuid);

                        // Ensure the request was successful
                        response.EnsureSuccessStatusCode();

                        // Read and process the response content
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Dictionary<string, object>? responseData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

                        if (responseData is null)
                            throw new HttpRequestException();
                        
                        if (responseData.ContainsKey("sets")){
                            // Has to take one set. Assume the user has already choosen on the front-end.
                            string? r = responseData["sets"].ToString();
                            List<object> setsList = JsonSerializer.Deserialize<List<object>>(r);

                            if (setNumber > setsList.Count)
                                throw new HttpRequestException();

                            string? setString = setsList[setNumber].ToString();
                            items = JsonSerializer.Deserialize<Dictionary<string, object>>(setString)["items"];


                        } else if (responseData.ContainsKey("items")){
                            items = responseData["items"];
                        } else{
                            throw new HttpRequestException();
                        }

                    } catch (HttpRequestException e){
                        Console.WriteLine("Error happened ");
                        return NotFound("Could not find xivgear.app gearset");
                    }

                    string? itemString = items.ToString();
                    Console.WriteLine(itemString);
                    Dictionary<string,Dictionary<string,object>> itemSet = JsonSerializer.Deserialize<Dictionary<string,Dictionary<string,object>>>(itemString);

                    if (itemSet is null)
                        return NotFound("Error while processing gear set");

                    foreach (KeyValuePair<string,Dictionary<string,object>> pair in itemSet){
                        Console.WriteLine("Value : " + pair.Value["id"].ToString());
                        int gearExternalId = Convert.ToInt32(pair.Value["id"].ToString());
                        Gear? gear;
                    
                        if (pair.Key == "RingRight")
                            {
                                gear = context.Gears.FirstOrDefault(g => g.EtroGearId == gearExternalId && g.GearType == GearType.RightRing);
                            }
                        else if (pair.Key == "RingLeft")
                            {
                                gear = context.Gears.FirstOrDefault(g => g.EtroGearId == gearExternalId && g.GearType == GearType.LeftRing);
                            }
                        else 
                            {
                            gear = context.Gears.FirstOrDefault(g => g.EtroGearId == gearExternalId);
                            }

                        if (gear is null)
                            {
                            Console.WriteLine("Gear not found : " + gearExternalId);
                            continue;
                            }

                        switch(pair.Key){
                            case "Weapon":
                                if (dto.UseBis)
                                    player.BisWeaponGearId = gear.Id;
                                else
                                    player.CurWeaponGearId = gear.Id;
                                break;
                            case "Head":
                                if (dto.UseBis)
                                    player.BisHeadGearId = gear.Id;
                                else
                                    player.CurHeadGearId = gear.Id;
                                break;
                            case "Body":
                                if (dto.UseBis)
                                    player.BisBodyGearId = gear.Id;
                                else
                                    player.CurBodyGearId = gear.Id;
                                break;
                            case "Hand":
                                if (dto.UseBis)
                                    player.BisHandsGearId = gear.Id;
                                else
                                    player.CurHandsGearId = gear.Id;
                                break;
                            case "Feet":
                                if (dto.UseBis)
                                    player.BisFeetGearId = gear.Id;
                                else
                                    player.CurFeetGearId = gear.Id;
                                break;
                            case "Legs":
                                if (dto.UseBis)
                                    player.BisLegsGearId = gear.Id;
                                else
                                    player.CurLegsGearId = gear.Id;
                                break;
                            case "Ears":
                                if (dto.UseBis)
                                    player.BisEarringsGearId = gear.Id;
                                else
                                    player.CurEarringsGearId = gear.Id;
                                break;
                            case "Neck":
                                if (dto.UseBis)
                                    player.BisNecklaceGearId = gear.Id;
                                else
                                    player.CurNecklaceGearId = gear.Id;
                                break;
                            case "Wrist":
                                if (dto.UseBis)
                                    player.BisBraceletsGearId = gear.Id;
                                else
                                    player.CurBraceletsGearId = gear.Id;
                                break;
                            case "RingRight":
                                if (dto.UseBis)
                                    player.BisRightRingGearId = gear.Id;    
                                else
                                    player.CurRightRingGearId = gear.Id;
                                break;
                            case "RingLeft":
                                if (dto.UseBis)
                                    player.BisLeftRingGearId = gear.Id;    
                                else
                                    player.CurLeftRingGearId = gear.Id;
                                break;  
                            default:
                                continue;

                        }
                    }
                    await context.SaveChangesAsync();
                    return Ok();
                }
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
                Console.WriteLine("Etro link : " + dto.NewEtro);
                string uuid = dto.NewEtro.Split("https://etro.gg/gearset/")[1];
                Console.WriteLine("Etro detected uuid : " + uuid);
                try
                {
                        // Make the GET request
                        HttpResponseMessage response = await client.GetAsync("/api/gearsets/"+uuid); // Replace 'endpoint' with the actual endpoint you want to access

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
                        //Console.WriteLine("Request error: " + e.Message);
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
                return Ok();
                //return AllGearPieceFound ? Ok() : Ok("Could not find at least one gear piece.");
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

        [HttpDelete("DeletePlayer/{id}")]
        public async Task<ActionResult> DeletePlayer(int id)
        {
            using (var context = _context.CreateDbContext())
            {   

                Players? player = await context.Players.FindAsync(id);
                if (player is null)
                    return NotFound("Player not found");

                bool isAuthorized = await UserIsAuthorized(HttpContext, player.Id.ToString(), context); // Even if unclaimed player must be in static to delete it.
                if(!isAuthorized)
                    return Unauthorized("Not Authorized");

                context.Players.Remove(player);
                await context.SaveChangesAsync();
                return Ok();
            }
        }
        [HttpPut("SetAltPlayer/{id}")]
        public async Task<ActionResult> SetAltPlayer(int id)
        {
            using (var context = _context.CreateDbContext())
            {
                Players? player = await context.Players.FindAsync(id);
                if (player is null)
                    return NotFound("Player not found");

            if (player.IsClaimed)
            {   
                bool isAuthorized = await UserIsAuthorized(HttpContext, player.Id.ToString(), context);
                if(!isAuthorized)
                    return Unauthorized("Not Authorized");
            }

                player.IsAlt = !player.IsAlt;
                await context.SaveChangesAsync();
                return Ok(player.IsAlt ? "true" : "false");
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

        /*[HttpGet("FixDatabase")]
        public async Task<ActionResult> FixDatabase(){
            using (var context = _context.CreateDbContext())
            {
                List<Players> players = context.Players.Where(p => p.IsClaimed).ToList();
                List<Users> discordUsers = context.User.Where(u => u.user_claimed_playerId != "").ToList();
                List<ApplicationUser> emailUsers = context.Users.Where(u => u.user_claimed_playerId != "").ToList();

                string msg = "";

                foreach (Players player in players){
                    // Checks if the player is actually claimed.

                    bool HasFoundClaimer = false;
                    foreach (Users discord in discordUsers){
                        if (discord.UserClaimedPlayer(player.Id.ToString())){
                            HasFoundClaimer = true;
                            msg += "Verified player : " + player.Id.ToString() + ";\n";
                            break;
                        }
                    }
                    if (!HasFoundClaimer){
                        foreach (ApplicationUser email in emailUsers){
                            if (email.UserClaimedPlayer(player.Id.ToString())){
                                HasFoundClaimer = true;
                                msg += "Verified player : " + player.Id.ToString() + ";\n";
                                break;
                            }
                        }
                        if(!HasFoundClaimer){
                            player.IsClaimed = false;
                            msg += "Unclaimed player : " + player.Id.ToString() + ";\n";
                            
                        }
                    }

                }
                await context.SaveChangesAsync();
                return Ok(msg);
            }
        }*/

        

    }
}