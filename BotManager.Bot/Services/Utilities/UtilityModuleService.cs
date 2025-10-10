using BotManager.Bot.Interfaces.Modules;
using BotManager.Bot.Modules.AntiSpam;
using BotManager.Bot.Modules.Constants;
using BotManager.Bot.Modules.Logging;
using BotManager.Bot.Modules.Models;
using BotManager.Bot.Modules.ModMail;
using BotManager.Bot.Modules.Reactions;
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
		if (config.ModMailConfigs.Any())
			await SetupModMailModule(client, config);
		
		foreach (var guildConfig in config.GuildConfigs)
		{
			if (guildConfig.HasLoggingModule)
				await SetupLoggingModule(guildConfig);
			
			if (guildConfig.HasAntiSpamModule)
				await SetupAntiSpamModule(guildConfig);
			
			if (guildConfig.HasReactionModule)
				await SetupReactionModule(guildConfig);
		}
	}

	private async Task SetupModMailModule(DiscordSocketClient client, BotConfig botConfig)
	{
		var module = new ModMailModule(client, botConfig);
		await module.RegisterModuleAsync();
	}

	private async Task SetupLoggingModule(GuildConfig guildConfig)
	{
		var module = new LoggingModule(client, guildConfig);
		await module.RegisterModuleAsync();

		RegisterModule(guildConfig, module, ModuleType.Logging);
	}
	
	private async Task SetupAntiSpamModule(GuildConfig guildConfig)
	{
		var module = new AntiSpamModule(client, guildConfig);
		await module.RegisterModuleAsync();
		
		RegisterModule(guildConfig, module, ModuleType.AntiSpam);
	}
	
	private async Task SetupReactionModule(GuildConfig guildConfig)
	{
		var module = new ReactionModule(client, guildConfig);
		await module.RegisterModuleAsync();
		
		RegisterModule(guildConfig, module, ModuleType.Reactions);
	}

	private void RegisterModule(GuildConfig guildConfig, IUtilityModule module, ModuleType moduleType)
	{
		var guildName = client.GetGuild(guildConfig.GuildId)
							?.Name ?? guildConfig.GuildId.ToString();

		Log.Debug(
			"Registering {ModuleName} for {BotName} in Guild {Guild}",
			module.GetType()
				.Name,
			User,
			guildName
		);

		var key = new ModuleData(moduleType, ClientId, guildConfig.GuildId);
		ModuleRegister.UtilityModules[key] = module;
	}
}