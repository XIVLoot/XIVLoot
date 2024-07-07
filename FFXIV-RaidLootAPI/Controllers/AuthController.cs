using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Cors;

namespace FFXIV_RaidLootAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _jwtKey;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtKey = _configuration["JwtSettings:Key"]!;
        }

    [HttpPost("ReadJwt")]
    [EnableCors("AllowSpecificOrigins")]
    public IActionResult ReadJwt([FromBody] string token)
    {
        try
        {
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
            var principal = handler.ValidateToken(token, tokenValidationParameters, out validatedToken);

            var claims = principal.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception during JWT reading: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "Error reading JWT token.");
        }
    }

    [HttpGet("IsLoggedInDiscord")]
    [EnableCors("AllowSpecificOrigins")]
    public IActionResult IsJwtXivlootPresent()
    {
        return Ok(HttpContext.Request.Cookies.ContainsKey("jwt_xivloot"));
    }

    [HttpGet("LogoutDiscord")]
    [EnableCors("AllowSpecificOrigins")]
    public IActionResult LogoutDiscord()
    {
        if (HttpContext.Request.Cookies.ContainsKey("jwt_xivloot"))
        {
            HttpContext.Response.Cookies.Delete("jwt_xivloot");
            return Ok("Logged out successfully.");
        }
        return BadRequest("No jwt_xivloot cookie found.");
    }

    [HttpPost("token")]
    [EnableCors("AllowSpecificOrigins")]
    public async Task<IActionResult> PostToken([FromBody] Dictionary<string, string> content)
    {
        Console.WriteLine("HERE CONTENT :  " + content.ToString());
        using (var client = new HttpClient())
        {   
            Console.WriteLine("CONTENT :  " + content.ToString());
            var formContent = new FormUrlEncodedContent(content);
            var response = await client.PostAsync("https://discord.com/api/oauth2/token", formContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("RESPONSE :  " + responseContent);
            return Content(responseContent, response.Content.Headers.ContentType.ToString());
        }
    }

    [HttpGet("GetDiscordUserInfo")]
    [EnableCors("AllowSpecificOrigins")]
    public async Task<IActionResult> GetDiscordUserInfo()
    {
        // Retrieve the JWT from the cookie
        if (!HttpContext.Request.Cookies.TryGetValue("jwt_xivloot", out var jwt))
        {
            return Unauthorized("JWT cookie not found.");
        }

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
            return Unauthorized("Access token not found in JWT.");
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

            var responseString = await response.Content.ReadAsStringAsync();
            return Ok(responseString);
        }
    }

    [HttpGet("GetDiscordJWT/{at}")]
    [EnableCors("AllowSpecificOrigins")]
    public async Task<IActionResult> GetDiscordJWT(string at)
    {
        // Generate JWT using the provided access token
        string jwt = GenerateJwtToken(at);
        Console.WriteLine("Generated JWT: " + jwt);

        // Set the JWT as a cookie
        HttpContext.Response.Cookies.Append("jwt_xivloot", jwt, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });

        // Return the JWT in the response
        return Ok(new { token = jwt });
    }

    private string GenerateJwtToken(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("access_token", accessToken) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}