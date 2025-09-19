using BotManager.Db.Context;
using BotManager.Db.Models;
using BotManager.Interfaces.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace BotManager.Services.Implementation.Data;

public class BotAuthorizationService(IDbContextFactory<BotManagerContext> dbContextFactory) : IBotAuthorizationService
{
	public async Task<bool> HasAnyAccessAsync(ulong userId)
	{
		await using var context = await dbContextFactory.CreateDbContextAsync();

		var configs = await context.Configs.ToListAsync();
		return configs.Any(x => x.AdminIds.Contains(userId));
	}

	public async Task<bool> HasBotAccessAsync(ulong userId, string botId)
	{
		await using var context = await dbContextFactory.CreateDbContextAsync();

		var bot = await context.Configs.FindAsync(botId);

		return bot is not null && bot.AdminIds.Contains(userId);
	}

	public async Task<IEnumerable<BotConfig>> GetConfigsWithAccessAsync(ulong userId)
	{
		await using var context = await dbContextFactory.CreateDbContextAsync();
		
		var configs = await context.Configs.ToListAsync();
		
		return configs.Where(x => x.AdminIds.Contains(userId));
	}
}