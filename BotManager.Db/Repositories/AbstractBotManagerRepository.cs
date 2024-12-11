using BotManager.Db.Context;
using Demolite.Db.Interfaces;
using Demolite.Db.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BotManager.Db.Repositories;

public class AbstractBotManagerRepository<T>(IDbContextFactory<BotManagerContext> contextFactory)
	: AbstractBaseRepository<T, BotManagerContext>, IDbRepository<T>
	where T : class, IDbItem
{
	protected override async Task<BotManagerContext> GetContextAsync()
		=> await contextFactory.CreateDbContextAsync();

	protected override BotManagerContext GetContext()
		=> contextFactory.CreateDbContext();
}