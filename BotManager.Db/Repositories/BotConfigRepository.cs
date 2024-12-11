using BotManager.Db.Context;
using BotManager.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace BotManager.Db.Repositories;

public class BotConfigRepository(IDbContextFactory<BotManagerContext> dbContextFactory)
	: AbstractBotManagerRepository<BotConfig>(dbContextFactory);