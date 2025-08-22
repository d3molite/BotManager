using System.Reflection;
using BotManager.Bot.Interfaces.Modules;
using BotManager.Bot.Modules.Birthdays;
using BotManager.Bot.Modules.Constants;
using BotManager.Bot.Modules.Feedback;
using BotManager.Bot.Modules.Image;
using BotManager.Bot.Modules.LanPlanner;
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
				await SetupModule<OrderTrackingModule>(guildConfig, service);
			}
			
			if (guildConfig.HasLanPlannerModule)
			{
				var service = DependencyManager.Provider.GetRequiredService<ILanPlanService>();
				await SetupModule<LanPlannerModule>(guildConfig, service);
			}

			if (guildConfig.HasBirthdayModule)
			{
				var service = DependencyManager.Provider.GetRequiredService<IBirthdayService>();
				await SetupModule<BirthdaysModule>(guildConfig, service);
			}

			if (guildConfig.HasImageModule)
				await SetupModule<ImageModule>(guildConfig);
			
			if (guildConfig.HasVoiceChannelModule)
				await SetupModule<VoiceChannelModule>(guildConfig);
			
			if (guildConfig.HasRoleRequestModule)
				await SetupModule<RoleRequestModule>(guildConfig);
			
			if (guildConfig.HasFeedbackModule)
				await SetupModule<FeedbackModule>(guildConfig);
			
			if (guildConfig.HasWatchPartyModule)
				await SetupModule<WatchPartyModule>(guildConfig);
		}
	}

	private async Task SetupModule<T>(GuildConfig guildConfig, params object[] parameters)
		where T : IRefCommandModule
	{
		var constructorParameters = new List<object>()
		{
			client, guildConfig,
		};
		
		constructorParameters.AddRange(parameters);

		IRefCommandModule? refCommandModule;
		
		try
		{
			refCommandModule = Activator.CreateInstance(typeof(T), constructorParameters.ToArray()) as IRefCommandModule;
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Failed to instantiate command module: {Type}", typeof(T));
			return;
		}
		
		
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		Task.Run(async () => await refCommandModule!.BuildCommands());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		
		RegisterModule(guildConfig, refCommandModule!);
	}
	
	private void RegisterModule(GuildConfig guildConfig, IRefCommandModule module)
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

		var moduleDefinition = new RefModuleInfo(module, ClientId, guildConfig.GuildId);
		ModuleRegister.RefCommandModules.Add(moduleDefinition);
	}
	
}