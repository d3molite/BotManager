using BotManager.Db.Models;
using BotManager.Db.Models.Modules.Image;
using BotManager.Db.Models.Modules.Order;
using Microsoft.EntityFrameworkCore;

namespace BotManager.Db.Context;

public class BotManagerContext : DbContext
{
	public BotManagerContext(DbContextOptions<BotManagerContext> options) : base(options)
	{
		
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<BotConfig>().Navigation(x => x.GuildConfigs).AutoInclude();

		modelBuilder.Entity<GuildConfig>().Navigation(x => x.OrderTrackingConfig).AutoInclude();

		modelBuilder.Entity<Order>().Navigation(x => x.OrderItems).AutoInclude();

		modelBuilder.Entity<GuildConfig>().Navigation(x => x.ImageConfig).AutoInclude();
	}

	public DbSet<BotConfig> Configs { get; set; } = null!;

	public DbSet<GuildConfig> GuildConfigs { get; set; } = null!;

	public DbSet<OrderTrackingConfig> OrderTrackingConfigs { get; set; } = null!;

	public DbSet<Order> Orders { get; set; } = null!;

	public DbSet<OrderItem> OrderItems { get; set; } = null!;

	public DbSet<ImageConfig> ImageConfigs { get; set; } = null!;
}