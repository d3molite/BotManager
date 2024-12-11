using BotManager.Db.Context;
using BotManager.Db.Models.Modules.Order;
using Microsoft.EntityFrameworkCore;

namespace BotManager.Db.Repositories;

public class OrderRepository(IDbContextFactory<BotManagerContext> dbContextFactory)
	: AbstractBotManagerRepository<Order>(dbContextFactory);