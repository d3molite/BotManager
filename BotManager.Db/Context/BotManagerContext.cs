using BotManager.Db.Models;
using BotManager.Db.Models.Modules.AntiSpam;
using BotManager.Db.Models.Modules.Birthdays;
using BotManager.Db.Models.Modules.Feedback;
using BotManager.Db.Models.Modules.Image;
using BotManager.Db.Models.Modules.Logging;
using BotManager.Db.Models.Modules.Order;
using BotManager.Db.Models.Modules.Reactions;
using BotManager.Db.Models.Modules.RoleRequest;
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

		modelBuilder.Entity<BotConfig>().HasMany(x => x.GuildConfigs).WithOne(x => x.BotConfig);

		// Guild Config Navigations

		modelBuilder.Entity<GuildConfig>().Navigation(x => x.OrderTrackingConfig).AutoInclude();

		modelBuilder.Entity<GuildConfig>().Navigation(x => x.ImageConfig).AutoInclude();

		modelBuilder.Entity<GuildConfig>().Navigation(x => x.BirthdayConfig).AutoInclude();

		modelBuilder.Entity<GuildConfig>().Navigation(x => x.LoggingConfig).AutoInclude();

		modelBuilder.Entity<GuildConfig>().Navigation(x => x.AntiSpamConfig).AutoInclude();

		modelBuilder.Entity<GuildConfig>().Navigation(x => x.VoiceChannelConfig).AutoInclude();

		modelBuilder.Entity<GuildConfig>().Navigation(x => x.ReactionConfig).AutoInclude();
		
		modelBuilder.Entity<GuildConfig>().Navigation(x => x.RoleRequestConfig).AutoInclude();
		
		modelBuilder.Entity<GuildConfig>().Navigation(x => x.FeedbackConfig).AutoInclude();

		// Guild Config Model Configurations

		modelBuilder.Entity<GuildConfig>().HasOne(x => x.OrderTrackingConfig).WithOne(x => x.GuildConfig);

		modelBuilder.Entity<GuildConfig>().HasOne(x => x.ImageConfig).WithOne(x => x.GuildConfig);

		modelBuilder.Entity<GuildConfig>().HasOne(x => x.BirthdayConfig).WithOne(x => x.GuildConfig);

		modelBuilder.Entity<GuildConfig>().HasOne(x => x.LoggingConfig).WithOne(x => x.GuildConfig);

		modelBuilder.Entity<GuildConfig>().HasOne(x => x.VoiceChannelConfig).WithOne(x => x.GuildConfig);

		modelBuilder.Entity<GuildConfig>().HasOne(x => x.AntiSpamConfig).WithOne(x => x.GuildConfig);

		modelBuilder.Entity<GuildConfig>().HasOne(x => x.ReactionConfig).WithOne(x => x.GuildConfig);
		
		modelBuilder.Entity<GuildConfig>().HasOne(x => x.RoleRequestConfig).WithOne(x => x.GuildConfig);
		
		modelBuilder.Entity<GuildConfig>().HasOne(x => x.FeedbackConfig).WithOne(x => x.GuildConfig);

		// Sub Config Navigations

		modelBuilder.Entity<Order>().Navigation(x => x.OrderItems).AutoInclude();

		modelBuilder.Entity<ReactionConfig>().Navigation(x => x.Reactions).AutoInclude();

		// Sub config model configurations

		modelBuilder.Entity<BirthdayConfig>().HasMany(x => x.Birthdays).WithOne(x => x.BirthdayConfig);

		modelBuilder.Entity<ReactionConfig>().HasMany(x => x.Reactions).WithOne(x => x.ReactionConfig);
	}

	public DbSet<BotConfig> Configs { get; set; } = null!;

	public DbSet<GuildConfig> GuildConfigs { get; set; } = null!;

	public DbSet<OrderTrackingConfig> OrderTrackingConfigs { get; set; } = null!;

	public DbSet<Order> Orders { get; set; } = null!;

	public DbSet<OrderItem> OrderItems { get; set; } = null!;

	public DbSet<ImageConfig> ImageConfigs { get; set; } = null!;

	public DbSet<BirthdayConfig> BirthdayConfigs { get; set; } = null!;

	public DbSet<Birthday> Birthdays { get; set; } = null!;

	public DbSet<LoggingConfig> LoggingConfigs { get; set; } = null!;

	public DbSet<AntiSpamConfig> AntiSpamConfigs { get; set; } = null!;

	public DbSet<ReactionConfig> ReactionConfigs { get; set; } = null!;

	public DbSet<ReactionItem> ReactionItems { get; set; } = null!;
	
	public DbSet<RoleRequestConfig> RoleRequestConfigs { get; set; } = null!;
	
	public DbSet<FeedbackConfig> FeedbackConfigs { get; set; } = null!;
}