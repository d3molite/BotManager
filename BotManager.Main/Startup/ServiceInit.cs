using BotManager.Authentication.Context;
using BotManager.Authentication.Controllers;
using BotManager.Db.Context;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.Birthdays;
using BotManager.Db.Models.Modules.LanPlanner;
using BotManager.Db.Models.Modules.Order;
using BotManager.Db.Repositories;
using BotManager.DI;
using BotManager.Interfaces.Services.Bot;
using BotManager.Interfaces.Services.Data;
using BotManager.Services.Implementation.Bot;
using BotManager.Services.Implementation.Data;
using Demolite.Db.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
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
		AddAuthenticationServices(builder);
		AddDataServices(builder.Services);
	}

	public static async Task ApplyMigrations(WebApplication app)
	{
		var factory = app.Services.GetRequiredService<IDbContextFactory<BotManagerContext>>();
		var db = await factory.CreateDbContextAsync();

		await db.Database.EnsureCreatedAsync();
		await db.Database.MigrateAsync();

		var idFactory = app.Services.GetRequiredService<IDbContextFactory<AuthenticationDbContext>>();
		var idDb = await idFactory.CreateDbContextAsync();

		await idDb.Database.EnsureCreatedAsync();
		await idDb.Database.MigrateAsync();
	}

	public static void SetContainer(WebApplication app)
	{
		DependencyManager.Provider = app.Services;
	}

	public static async Task StartBots(WebApplication app)
	{
		var botService = app.Services.GetRequiredService<IBotService>();
		await botService.Initialize();
	}

	private static void AddAuthenticationServices(WebApplicationBuilder builder)
	{

		builder
			.Services.AddAuthentication(options =>
				{
					options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = "Discord";
				}
			)
			.AddCookie(
				CookieAuthenticationDefaults.AuthenticationScheme,
				options =>
				{
					options.LoginPath = "/Account/Login";
					options.LogoutPath = "/Account/Logout";
					options.AccessDeniedPath = "/Account/AccessDenied";
					options.ExpireTimeSpan = TimeSpan.FromDays(1);
					options.SlidingExpiration = true;
				}
			);

		builder
			.Services.AddOpenIddict()
			.AddClient(clientOptions =>
				{
					clientOptions.AllowAuthorizationCodeFlow();

					clientOptions.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();

					clientOptions.UseAspNetCore().EnableRedirectionEndpointPassthrough();

					clientOptions.UseSystemNetHttp();

					clientOptions
						.UseWebProviders()
						.AddDiscord(discordOptions
							=> discordOptions
								.SetClientId(Environment.GetEnvironmentVariable("DISCORD_CLIENT_ID")!)
								.SetClientSecret(Environment.GetEnvironmentVariable("DISCORD_CLIENT_SECRET")!)
								.SetRedirectUri("callback/login/discord")
						);
				}
			)
			.AddCore(coreOptions => coreOptions.UseEntityFrameworkCore().UseDbContext<AuthenticationDbContext>());

		builder.Services.AddControllers().AddApplicationPart(typeof(AuthController).Assembly);
		
		builder.Services.AddScoped<IBotAuthorizationService, BotAuthorizationService>();
	}

	private static void AddDatabaseServices(WebApplicationBuilder builder)
	{
		builder.Services.AddDbContextFactory<AuthenticationDbContext>(options
			=> options.UseSqlite("DataSource=./dbdata/identity.db").UseOpenIddict()
		);

		builder.Services.AddDbContext<AuthenticationDbContext>();

		builder.Services.AddDbContextFactory<BotManagerContext>(options
			=> options.UseSqlite("DataSource=./dbdata/main.db")
		);

		builder.Services.AddDbContext<BotManagerContext>();
	}

	private static void AddDataServices(IServiceCollection collection)
	{
		collection.AddSingleton<IDbRepository<BotConfig>, BotConfigRepository>();
		collection.AddSingleton<IBotConfigService, BotConfigService>();
		collection.AddSingleton<IBotService, BotService>();

		collection.AddSingleton<IDbRepository<OrderItem>, OrderItemRepository>();
		collection.AddSingleton<IDbRepository<Order>, OrderRepository>();
		collection.AddSingleton<IOrderService, OrderService>();

		collection.AddSingleton<IDbRepository<Birthday>, BirthdayRepository>();
		collection.AddSingleton<IBirthdayService, BirthdayService>();

		collection.AddSingleton<IDbRepository<LanPlannerConfig>, LanPlannerConfigRepository>();
		collection.AddSingleton<IDbRepository<LanMember>, LanMemberRepository>();
		collection.AddSingleton<IDbRepository<LanPlan>, LanPlanRepository>();
		collection.AddSingleton<ILanPlanService, LanPlanService>();
	}
}