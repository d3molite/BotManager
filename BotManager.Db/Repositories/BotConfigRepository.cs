using BotManager.Db.Context;
using BotManager.Db.Interfaces;
using BotManager.Db.Models;
using EfExtensions.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace BotManager.Db.Repositories;

public class BotConfigRepository : AbstractGeneratedContextRepository<BotConfig, string, BotManagerContext>, IBotConfigRepository
{
	public BotConfigRepository(IDbContextFactory<BotManagerContext> dbContextFactory) : base(dbContextFactory)
	{
	}

	protected override void UpdateCollections(BotConfig incoming, BotConfig existing, BotManagerContext db)
	{
		throw new NotImplementedException();
	}

	protected override void UpdateEfItem(BotConfig incoming, ref BotConfig existing)
	{
		throw new NotImplementedException();
	}
}