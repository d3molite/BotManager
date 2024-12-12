using BotManager.Bot.Interfaces.Modules;
using BotManager.Bot.Modules.Birthdays;
using BotManager.Bot.Modules.Constants;
using BotManager.Bot.Modules.Image;
using BotManager.Bot.Modules.Models;
using BotManager.Bot.Modules.OrderTracking;
using BotManager.Bot.Services.Register;
using BotManager.Db.Models;
using BotManager.DI;
using BotManager.Interfaces.Services.Data;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BotManager.Bot.Services.Commands;

public partial class CommandModuleService
{
	private string User => config.Name;
	
	private ulong ClientId => client.Rest.CurrentUser.Id;
	
	private async Task BuildCommandsInternal()
	{
		var user = config.Name;

		foreach (var guildConfig in config.GuildConfigs)
		{
			if (guildConfig.HasOrderTrackingModule)
				await SetupOrderModule(guildConfig);
			
			if (guildConfig.HasImageModule)
				SetupImageModule(guildConfig);

			if (guildConfig.HasBirthdayModule)
				await SetupBirthdayModule(guildConfig);
		}
	}

	private async Task SetupOrderModule(GuildConfig guildConfig)
	{
		var service = DependencyManager.Provider.GetRequiredService<IOrderService>();
		var module = new OrderTrackingModule(service, client);

		await module.BuildCommands(guildConfig.OrderTrackingConfig!, guildConfig.GuildId);
		RegisterModule(guildConfig, module, ModuleType.Order);
	}

	private void SetupImageModule(GuildConfig guildConfig)
	{
		var module = new ImageModule(client);

		Task.Run(async() => await module.BuildCommands(guildConfig.ImageConfig!, guildConfig.GuildId));
		RegisterModule(guildConfig, module, ModuleType.Image);
	}

	private async Task SetupBirthdayModule(GuildConfig guildConfig)
	{
		var service = DependencyManager.Provider.GetRequiredService<IBirthdayService>();
		var module = new BirthdaysModule(client, service);

		await module.BuildCommands(guildConfig.BirthdayConfig!, guildConfig.GuildId);

		RegisterModule(guildConfig, module, ModuleType.Birthdays);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		Task.Run(async () => await module.StartCheckTask());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
	}

	private void RegisterModule(GuildConfig guildConfig, ICommandModule module, ModuleType moduleType)
	{
		Log.Debug("Registering {ModuleName} for {BotName} in {Guild}", module.GetType().Name, User, guildConfig.GuildId);
		var key = new ModuleData(moduleType, ClientId, guildConfig.GuildId);
		ModuleRegister.CommandModules[key] = module;
	}
}