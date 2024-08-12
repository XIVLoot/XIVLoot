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
using Microsoft.EntityFrameworkCore;
using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.User;
using System.Text.Json;
using FFXIV_RaidLootAPI.Models;
using Microsoft.AspNetCore.Identity;
using RestSharp;
using FFXIV_RaidLootAPI.DTO;
using System.Net;

namespace FFXIV_RaidLootAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDbContextFactory<DataContext> _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _jwtKey;

        public AuthController(UserManager<ApplicationUser> userManager,IDbContextFactory<DataContext> context,IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
            _jwtKey = _configuration["JwtSettings:Key"]!;
            _userManager = userManager;
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
            //Console.WriteLine("Exception during JWT reading: " + ex.Message);
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
        //Console.WriteLine("HERE CONTENT :  " + content.ToString());
        using (var client = new HttpClient())
        {   
            //Console.WriteLine("CONTENT :  " + content.ToString());
            var formContent = new FormUrlEncodedContent(content);
            var response = await client.PostAsync("https://discord.com/api/oauth2/token", formContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            //Console.WriteLine("RESPONSE :  " + responseContent);
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
                //Console.WriteLine("Error fetching user info: " + errorContent);
                throw new HttpRequestException($"Error fetching user info: {response.StatusCode}, Content: {errorContent}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            Dictionary<string, object> responseData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseString)!;
            //Console.WriteLine("Adding discord user");
            //Console.WriteLine(responseData.ToString());
            // Access the 'id' value from the dictionary
            string discordId = responseData["id"].ToString()!;
            using (var context = _context.CreateDbContext())
            {

                Users? user = await context.User.FirstOrDefaultAsync(u => u.user_discord_id == discordId);
                if (user is null){
                    await context.User.AddAsync(new Users{
                        user_discord_id=discordId,
                        user_saved_static = string.Empty,
                        user_claimed_playerId=string.Empty,
                    });
                    await context.SaveChangesAsync();
                }
            }
            return Ok(responseString);
        }
    }

    [HttpGet("GetDiscordJWT/{at}")]
    [EnableCors("AllowSpecificOrigins")]
    public async Task<IActionResult> GetDiscordJWT(string at)
    {
        // Generate JWT using the provided access token
        string jwt = GenerateJwtToken(at);
        //Console.WriteLine("Generated JWT: " + jwt);

        // Set the JWT as a cookie
        HttpContext.Response.Cookies.Append("jwt_xivloot", jwt, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddYears(2)
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
                Expires = DateTime.UtcNow.AddYears(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    [HttpPost("forgot-password")]
    [EnableCors("AllowSpecificOrigins")]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        using (var context = _context.CreateDbContext())
            {
                ApplicationUser? user = await _userManager.FindByEmailAsync(email);
                if (user is null)
                    return NotFound();

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"http://localhost:4200/reset-password?email={user.Email}&token={WebUtility.UrlEncode(token)}";
            Console.WriteLine("RESET LINK : " + resetLink);

            var client = new RestClient("https://send.api.mailtrap.io/api/send");
            var request = new RestRequest();
            request.AddHeader("Authorization", "Bearer 565dd4a78917b21604c8cc612b4e0aaf");
            request.AddHeader("Content-Type", "application/json");

        var emailPayload = new
        {
            from = new { email = "mailtrap@xivloot.com", name = "Password Reset" },
            to = new[] { new { email = email } },
            template_uuid = "cae81b2c-2bb2-405d-9a11-6bc40087bc7c",
            template_variables = new
            {
                user_email = email,
                pass_reset_link = resetLink
            }
        };

        var jsonPayload = JsonSerializer.Serialize(emailPayload);
        request.AddParameter("application/json", jsonPayload, ParameterType.RequestBody);


            var response = client.Post(request);
            System.Console.WriteLine(response.Content);

            if (response.IsSuccessful)
            {
                return Ok(new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Email sent with password reset link. Please check your email."
                });
            }
            else
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = response.Content!.ToString()
                });
            } 
            }
    }

    [HttpPost("ResetPassword/{email}/{token}/{newPassword}")]
    [EnableCors("AllowSpecificOrigins")]
    public async Task<IActionResult> ResetPassword(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            token = WebUtility.UrlDecode(token);

            if (user is null)
            {
                Console.WriteLine("USER NOT FOUND");
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User does not exist with this email"
                });
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                return Ok(new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Password reset Successfully"
                });
            }

            return BadRequest(new AuthResponseDto
            {
                IsSuccess = false,
                Message = result.Errors.FirstOrDefault()!.Description
            });
        }



    }
}