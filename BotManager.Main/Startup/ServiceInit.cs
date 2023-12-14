using BotManager.Db.Context;
using BotManager.Db.Interfaces;
using BotManager.Db.Repositories;
using BotManager.Services.Implementation.Bot;
using BotManager.Services.Implementation.Data;
using BotManager.Services.Interfaces;
using BotManager.Services.Interfaces.Bot;
using BotManager.Services.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BotManager.Main.Startup;

public static class ServiceInit
{
	public static void ConfigureLogging()
	{
		Log.Logger = new LoggerConfiguration()
					.WriteTo.Debug()
					.WriteTo.Trace()
					.WriteTo.Console()
					.MinimumLevel.Debug()
					.CreateLogger();
	}
	
	public static void RegisterServices(WebApplicationBuilder builder)
	{
		AddDatabaseServices(builder);
		AddDataServices(builder.Services);
	}

	public static async Task ApplyMigrations(WebApplication app)
	{
		var factory = app.Services.GetService<IDbContextFactory<BotManagerContext>>();
		var db = await factory.CreateDbContextAsync();
		
		await db.Database.MigrateAsync();
	}

	public static async Task StartBots(WebApplication app)
	{
		var botService = app.Services.GetRequiredService<IBotService>();
		await botService.Initialize();
	}

	private static void AddDatabaseServices(WebApplicationBuilder builder)
	{
		var appConnectionString = builder.Configuration.GetConnectionString("BotManagerDb");
		builder.Services.AddDbContextFactory<BotManagerContext>(options => options.UseSqlite(appConnectionString));
		builder.Services.AddDbContext<BotManagerContext>();

		var identityConnectionString = "";
	}

	private static void AddDataServices(IServiceCollection collection)
	{
		collection.AddSingleton<IBotConfigRepository, BotConfigRepository>();
		collection.AddSingleton<IBotConfigService, BotConfigService>();
		collection.AddSingleton<IBotService, BotService>();
	}
}