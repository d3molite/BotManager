using BotManager.Db.Context;
using BotManager.Db.Interfaces;
using BotManager.Db.Models.Modules.Order;
using EfExtensions.Core.Enum;
using EfExtensions.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace BotManager.Db.Repositories;

public class OrderRepository(IDbContextFactory<BotManagerContext> dbContextFactory)
	: AbstractGeneratedContextRepository<Order, string, BotManagerContext>(dbContextFactory), IOrderRepository
{
	protected override void UpdateCollections(Order incoming, Order existing, BotManagerContext db)
	{
		// do nothing
	}

	protected override void UpdateEfItem(Order incoming, ref Order existing)
	{
		// do nothing
	}
}