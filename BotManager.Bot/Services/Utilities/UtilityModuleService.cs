using BotManager.Bot.Interfaces.Modules;
using BotManager.Bot.Modules.Constants;
using BotManager.Bot.Modules.Logging;
using BotManager.Bot.Modules.Models;
using BotManager.Bot.Services.Register;
using BotManager.Db.Models;
using Discord.WebSocket;
using Serilog;

namespace BotManager.Bot.Services.Utilities;

public class UtilityModuleService(BotConfig config, DiscordSocketClient client)
{
	private string User => config.Name;
	
	private ulong ClientId => client.Rest.CurrentUser.Id;

	public async Task InitializeAsync()
	{
		await InitializeInternal();
	}

	private async Task InitializeInternal()
	{
		foreach (var guildConfig in config.GuildConfigs)
		{
			if (guildConfig.HasLoggingModule)
				await SetupLoggingModule(guildConfig);
		}
	}

	private async Task SetupLoggingModule(GuildConfig guildConfig)
	{
		var module = new LoggingModule(client, guildConfig);
		await module.RegisterModuleAsync();
		
		RegisterModule(guildConfig, module, ModuleType.Logging);
	}

	private void RegisterModule(GuildConfig guildConfig, IUtilityModule module, ModuleType moduleType)
	{
		Log.Debug("Registering {ModuleName} for {BotName} in {Guild}", module.GetType().Name, User, guildConfig.GuildId);
		var key = new ModuleData(moduleType, ClientId, guildConfig.GuildId);
		ModuleRegister.UtilityModules[key] = module;
	}
}