using System.Security.Claims;
using BotManager.Interfaces.Services.Data;
using Microsoft.AspNetCore.Authorization;

namespace BotManager.Authentication.Access;

public class HasAnyBotAccessHandler(IBotAuthorizationService authorizationService)
	: AuthorizationHandler<HasAnyBotAccessRequirement>
{
	protected override async Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		HasAnyBotAccessRequirement requirement
	)
	{
		var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (string.IsNullOrEmpty(userId) || !ulong.TryParse(userId, out var userUlong))
		{
			context.Fail();
			return;
		}

		var hasBotAccess = await authorizationService.HasAnyAccessAsync(userUlong);

		if (!hasBotAccess)
			context.Fail();

		context.Succeed(requirement);
	}
}