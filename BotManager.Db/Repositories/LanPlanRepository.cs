using BotManager.Db.Context;
using BotManager.Db.Models.Modules.LanPlanner;
using Microsoft.EntityFrameworkCore;

namespace BotManager.Db.Repositories;

public class LanPlanRepository(IDbContextFactory<BotManagerContext> factory)
	: AbstractBotManagerRepository<LanPlan>(factory);