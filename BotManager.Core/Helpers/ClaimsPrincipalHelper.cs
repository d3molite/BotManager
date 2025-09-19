using System.Security.Claims;

namespace BotManager.Core.Helpers;

public static class ClaimsPrincipalHelper
{
	public static bool TryGetUserId(this ClaimsPrincipal claimsPrincipal, out ulong discordUserId)
	{
		var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (string.IsNullOrEmpty(userId) || !ulong.TryParse(userId, out var ulongUserId))
		{
			discordUserId = 0;
			return false;
		}
		
		discordUserId = ulongUserId;
		return true;
	}
}