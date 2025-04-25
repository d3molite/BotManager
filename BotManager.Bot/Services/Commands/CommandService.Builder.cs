using System.Reflection;
using BotManager.Bot.Interfaces.Modules;
using BotManager.Bot.Modules.Birthdays;
using BotManager.Bot.Modules.Constants;
using BotManager.Bot.Modules.Feedback;
using BotManager.Bot.Modules.Image;
using BotManager.Bot.Modules.Models;
using BotManager.Bot.Modules.OrderTracking;
using BotManager.Bot.Modules.RoleRequest;
using BotManager.Bot.Modules.VoiceChannel;
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
		
		foreach (var guildConfig in config.GuildConfigs)
		{
			// var guild = client.GetGuild(guildConfig.GuildId);
			// await guild.DeleteApplicationCommandsAsync();
				
			if (guildConfig.HasOrderTrackingModule)
				await SetupOrderModule(guildConfig);
			
			if (guildConfig.HasBirthdayModule)
				await SetupBirthdayModule(guildConfig);

			if (guildConfig.HasImageModule)
				SetupImageModule(guildConfig);
			
			if (guildConfig.HasVoiceChannelModule)
				SetupVoiceChannelModule(guildConfig);
			
			if (guildConfig.HasRoleRequestModule)
				await SetupRoleRequestModule(guildConfig);
			
			if (guildConfig.HasFeedbackModule)
				await SetupFeedbackModule(guildConfig);
		}
	}

	private async Task SetupFeedbackModule(GuildConfig guildConfig)
	{
		var module = new FeedbackModule(client, guildConfig);
		await module.BuildCommands();
		
		RegisterRefModule(guildConfig, module);
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

		Task.Run(async () => await module.BuildCommands(guildConfig.ImageConfig!, guildConfig.GuildId));
		RegisterModule(guildConfig, module, ModuleType.Image);
	}
	
	private void SetupVoiceChannelModule(GuildConfig guildConfig)
	{
		var module = new VoiceChannelModule(client, guildConfig);
		Task.Run(async() => await module.BuildCommands(guildConfig.VoiceChannelConfig!, guildConfig.GuildId));
		RegisterModule(guildConfig, module, ModuleType.Voice);
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

	private async Task SetupRoleRequestModule(GuildConfig guildConfig)
	{
		var module = new RoleRequestModule(client, guildConfig);
		
		await module.BuildCommands(guildConfig.RoleRequestConfig!, guildConfig.GuildId);
		RegisterModule(guildConfig, module, ModuleType.RoleRequest);
	}

	private void RegisterModule(GuildConfig guildConfig, ICommandModule module, ModuleType moduleType)
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
		ModuleRegister.CommandModules[key] = module;
	}

	private void RegisterRefModule(GuildConfig guildConfig, IRefCommandModule module)
	{
		var guildName = client.GetGuild(guildConfig.GuildId)
			?.Name ?? guildConfig.GuildId.ToString();
		
		Log.Debug(
			"Registering Ref {ModuleName} for {BotName} in Guild {Guild}",
			module.GetType()
				.Name,
			User,
			guildName
		);

		var moduleDefinition = new RefModuleInfo(module, ClientId, guildConfig.GuildId);
		ModuleRegister.RefCommandModules.Add(moduleDefinition);
	}
	
}