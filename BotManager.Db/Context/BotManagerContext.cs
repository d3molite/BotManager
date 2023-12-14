using BotManager.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace BotManager.Db.Context;

public class BotManagerContext : DbContext
{
	public BotManagerContext(DbContextOptions<BotManagerContext> options) : base(options)
	{
		
	}

	public DbSet<BotConfig> Configs { get; set; } = null!;
}