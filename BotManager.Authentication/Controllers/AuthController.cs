using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client.AspNetCore;

namespace BotManager.Authentication.Controllers;

[Route("[controller]")]
public class AuthController : ControllerBase
{

	[HttpGet("~/Account/Login")]
	public IActionResult Login(string? returnUrl = null)
	{
		// Pass return URL through OAuth2 state parameter via authentication properties
		var properties = new AuthenticationProperties
		{
			RedirectUri = Url.Action(nameof(Callback)),
		};

		if (!string.IsNullOrEmpty(returnUrl))
		{
			properties.Items["returnUrl"] = returnUrl;
		}

		return Challenge(properties, "Discord");
	}

	[HttpGet("~/callback/login/discord")]
	public async Task<IActionResult> Callback()
	{
		// Get the external authentication result
		var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);

		if (!result.Succeeded || result.Principal == null)
		{
			return Redirect("/?error=auth_failed");
		}

		// Extract claims from Discord
		var claims = new List<Claim>();

		// Discord user ID
		var discordId = result.Principal.FindFirst("sub")?.Value ?? result.Principal.FindFirst("id")?.Value;

		if (!string.IsNullOrEmpty(discordId))
		{
			claims.Add(new Claim(ClaimTypes.NameIdentifier, discordId));
		}

		// Discord username
		var username = result.Principal.FindFirst("username")?.Value ?? result.Principal.FindFirst("name")?.Value;

		if (!string.IsNullOrEmpty(username))
		{
			claims.Add(new Claim(ClaimTypes.Name, username));
		}

		// Discord discriminator (if available)
		var discriminator = result.Principal.FindFirst("discriminator")?.Value;

		if (!string.IsNullOrEmpty(discriminator))
		{
			claims.Add(new Claim("discord_discriminator", discriminator));
		}

		// Discord avatar
		var avatar = result.Principal.FindFirst("avatar")?.Value;

		if (!string.IsNullOrEmpty(avatar))
		{
			claims.Add(new Claim("discord_avatar", avatar));
		}

		// Discord email (if available and verified)
		var email = result.Principal.FindFirst("email")?.Value;

		if (!string.IsNullOrEmpty(email))
		{
			claims.Add(new Claim(ClaimTypes.Email, email));
		}

		// Create identity and sign in
		var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var principal = new ClaimsPrincipal(identity);

		await HttpContext.SignInAsync(
			CookieAuthenticationDefaults.AuthenticationScheme,
			principal,
			new AuthenticationProperties
			{
				IsPersistent = true,
				ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
			}
		);

		// Get return URL from the authentication properties
		var returnUrl = result.Properties?.Items.TryGetValue("returnUrl", out var url) == true ? url : "/";
		return Redirect(returnUrl);
	}

	[HttpGet("~/Account/Logout")]

	[Authorize]
	public async Task<IActionResult> LogoutGet()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		return Redirect("/");
	}

	[HttpGet("~/Account/AccessDenied")]
	public IActionResult AccessDenied()
	{
		return Ok("Access Denied");
	}
}