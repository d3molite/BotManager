using System.Security.Claims;
using BotManager.Interfaces.Services.Data;
using Microsoft.AspNetCore.Authorization;

namespace BotManager.Authentication.Access;

public class BotSpecificAccessHandler(IBotAuthorizationService authorizationService)
	: AuthorizationHandler<BotSpecificAccessRequirement>
{
	protected override async Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		BotSpecificAccessRequirement requirement
	)
	{
		var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (string.IsNullOrEmpty(userId) || !ulong.TryParse(userId, out var userUlong))
		{
			context.Fail();
			return;
		}

		var hasBotAccess = await authorizationService.HasBotAccessAsync(userUlong, requirement.BotConfigId);

		if (!hasBotAccess)
			context.Fail();

		context.Succeed(requirement);
	}
}