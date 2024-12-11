using BotManager.Db.Context;
using BotManager.Db.Models.Modules.Birthdays;
using Microsoft.EntityFrameworkCore;

namespace BotManager.Db.Repositories;

public class BirthdayRepository(IDbContextFactory<BotManagerContext> dbContextFactory)
	: AbstractBotManagerRepository<Birthday>(dbContextFactory);