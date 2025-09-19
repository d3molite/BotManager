using BotManager.Db.Models;

namespace BotManager.Interfaces.Services.Data;

public interface IBotAuthorizationService
{
	public Task<bool> HasAnyAccessAsync(ulong userId);
	
	public Task<bool> HasBotAccessAsync(ulong userId, string botId);
	
	public Task<IEnumerable<BotConfig>> GetConfigsWithAccessAsync(ulong userId);
}