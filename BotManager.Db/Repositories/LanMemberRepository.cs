using BotManager.Db.Context;
using BotManager.Db.Models.Modules.LanPlanner;
using Microsoft.EntityFrameworkCore;

namespace BotManager.Db.Repositories;

public class LanMemberRepository(IDbContextFactory<BotManagerContext> factory)
	: AbstractBotManagerRepository<LanMember>(factory);