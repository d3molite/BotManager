using Microsoft.AspNetCore.Authorization;

namespace BotManager.Authentication.Access;

public class BotSpecificAccessRequirement(string botConfigId) : IAuthorizationRequirement
{
	public string BotConfigId { get; } = botConfigId;
}