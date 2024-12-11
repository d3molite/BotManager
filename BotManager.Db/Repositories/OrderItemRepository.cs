using BotManager.Db.Context;
using BotManager.Db.Models.Modules.Order;
using Microsoft.EntityFrameworkCore;

namespace BotManager.Db.Repositories;

public class OrderItemRepository(IDbContextFactory<BotManagerContext> dbContextFactory) : 
	AbstractBotManagerRepository<OrderItem>(dbContextFactory);