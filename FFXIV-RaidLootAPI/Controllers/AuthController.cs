using System.Security.Claims;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;

namespace FFXIV_RaidLootAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
    [HttpGet("signin-discord")]
    public IActionResult SignInWithDiscord()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("DiscordCallback", "Auth")
        };
        return Challenge(properties, DiscordAuthenticationDefaults.AuthenticationScheme);
    }

    [HttpGet("signin-discord-callback")]
    public async Task<IActionResult> DiscordCallback()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
            return BadRequest(); // Handle error

        // Handle successful authentication
        return Redirect("http://localhost:4200"); // Redirect to your frontend
    }
        [HttpGet("login")]
        public IActionResult Login()
        {
            var properties = new AuthenticationProperties { RedirectUri = "https://discord.com/oauth2/authorize?client_id=1252372424138166343&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A7203%2Fauth%2Fdiscord%2Fcallback&scope=identify" };
            return Challenge(properties, DiscordAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return BadRequest(); // Handle failed login

            // Handle successful login
            var claims = authenticateResult.Principal.Identities.First().Claims;
            var accessToken = authenticateResult.Properties.GetTokenValue("access_token");

            // Return user info or token
            return Ok(new
            {
                Username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                AccessToken = accessToken
            });
        }
    }
}