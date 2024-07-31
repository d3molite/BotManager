using BotManager.Db.Context;
using BotManager.Db.Interfaces;
using BotManager.Db.Models.Modules.Birthdays;
using EfExtensions.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace BotManager.Db.Repositories;

public class BirthdayRepository : AbstractGeneratedContextRepository<Birthday, string, BotManagerContext>, IBirthdayRepository
{
	public BirthdayRepository(IDbContextFactory<BotManagerContext> dbContextFactory) : base(dbContextFactory)
	{
	}

	protected override void UpdateCollections(Birthday incoming, Birthday existing, BotManagerContext db)
	{
		// do nothing
	}

	protected override void UpdateEfItem(Birthday incoming, ref Birthday existing)
	{
		// do nothing
	}
}