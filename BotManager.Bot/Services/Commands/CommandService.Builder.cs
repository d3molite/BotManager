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
using BotManager.Bot.Modules.WatchParty;
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
			// #if RELEASE
			// var guild = client.GetGuild(guildConfig.GuildId);
			// await guild.DeleteApplicationCommandsAsync();
			// #endif

			if (guildConfig.HasOrderTrackingModule)
			{
				var service = DependencyManager.Provider.GetRequiredService<IOrderService>();
				await SetupModule<OrderTrackingModule>(guildConfig, false, service);
			}

			if (guildConfig.HasBirthdayModule)
			{
				var service = DependencyManager.Provider.GetRequiredService<IBirthdayService>();
				await SetupModule<BirthdaysModule>(guildConfig, false, service);
			}

			if (guildConfig.HasImageModule)
				await SetupModule<ImageModule>(guildConfig, true);
			
			if (guildConfig.HasVoiceChannelModule)
				SetupVoiceChannelModule(guildConfig);
			
			if (guildConfig.HasRoleRequestModule)
				await SetupModule<RoleRequestModule>(guildConfig);
			
			if (guildConfig.HasFeedbackModule)
				await SetupModule<FeedbackModule>(guildConfig);
			
			if (guildConfig.HasWatchPartyModule)
				await SetupModule<WatchPartyModule>(guildConfig);
		}
	}

	private async Task SetupModule<T>(GuildConfig guildConfig, bool isLongLoading = false, params object[] parameters)
		where T : IRefCommandModule
	{
		var constructorParameters = new List<object>()
		{
			client, guildConfig,
		};
		
		constructorParameters.AddRange(parameters);
		
		var module = Activator.CreateInstance(typeof(T), constructorParameters.ToArray()) as IRefCommandModule;
		
		if (isLongLoading)
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			Task.Run(async () => await module!.BuildCommands());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		
		else
			await module!.BuildCommands();
		
		RegisterRefModule(guildConfig, module!);
	}
	
	private void SetupVoiceChannelModule(GuildConfig guildConfig)
	{
		var module = new VoiceChannelModule(client, guildConfig);
		Task.Run(async() => await module.BuildCommands(guildConfig.VoiceChannelConfig!, guildConfig.GuildId));
		RegisterModule(guildConfig, module, ModuleType.Voice);
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