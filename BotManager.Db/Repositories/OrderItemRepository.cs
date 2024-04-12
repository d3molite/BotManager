using BotManager.Db.Context;
using BotManager.Db.Interfaces;
using BotManager.Db.Models.Modules.Order;
using EfExtensions.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace BotManager.Db.Repositories;

public class OrderItemRepository(IDbContextFactory<BotManagerContext> dbContextFactory) : 
	AbstractGeneratedContextRepository<OrderItem, string, BotManagerContext>(dbContextFactory), IOrderItemRepository
{
	protected override void UpdateCollections(OrderItem incoming, OrderItem existing, BotManagerContext db)
	{
		// do nothing
	}

	protected override void UpdateEfItem(OrderItem incoming, ref OrderItem existing)
	{
		// do nothing
	}
}