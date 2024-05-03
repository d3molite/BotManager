
using BotManager.Bot.Modules.Constants;
using BotManager.Bot.Modules.Image;
using BotManager.Bot.Modules.Models;
using BotManager.Bot.Modules.OrderTracking;
using BotManager.Db.Models;
using BotManager.DI;
using BotManager.Interfaces.Services.Data;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BotManager.Bot.Services;

public class CommandService(BotConfig config, DiscordSocketClient client)
{
	private ulong ClientId => client.Rest.CurrentUser.Id;
	
	public async Task BuildCommands()
	{
		await Task.Run(async () => await BuildCommandsInternal());
	}

	private async Task BuildCommandsInternal()
	{
		foreach (var guildConfig in config.GuildConfigs)
		{
			if (guildConfig.OrderTrackingConfig != null)
			{
				Log.Debug("Found Order Module for {BotName} in {Guild}", client.Rest.CurrentUser.GlobalName, guildConfig.GuildId);
				
				var service = DependencyManager.Provider.GetRequiredService<IOrderService>();
				var module = new OrderTrackingModule(service, client);
				
				await module.BuildCommands(guildConfig.OrderTrackingConfig, guildConfig.GuildId);

				var key = new ModuleData(ModuleType.Order, ClientId, guildConfig.GuildId);
				
				ModuleRegister.Modules[key] = module;
			}

			if (guildConfig.ImageConfig != null)
			{
				Log.Debug("Found Image Module for {BotName} in {Guild}", client.Rest.CurrentUser.GlobalName, guildConfig.GuildId);
				var module = new ImageModule(client);
				
				await module.BuildCommands(guildConfig.ImageConfig, guildConfig.GuildId);

				var key = new ModuleData(ModuleType.Image, ClientId, guildConfig.GuildId);

				ModuleRegister.Modules[key] = module;
			}
		}
	}

	public async Task ExecuteCommand(SocketSlashCommand command)
	{
		Log.Debug("Received Command {CommandName}", command.Data.Name);

		try
		{
			var module = ModuleRegister.TryGetFromCommand(command.CommandName, ClientId, command.GuildId!.Value);

			if (module != null)
			{
				await module.ExecuteCommands(command);
			}
			else
			{
				Log.Debug("Module for command {CommandName} was not found", command.CommandName);
			}
		}
		catch (Exception ex)
		{
			Log.Error(ex, "{BotName}:", config.Name);
		}
	}

	public async Task ExecuteModalResponse(SocketModal modal)
	{
		var data = modal.Data.CustomId;

		var module = ModuleRegister.TryGetFromModal(data, ClientId, modal.GuildId!.Value);
		
		if (module != null)
			await module.ExecuteModal(modal);

		else
			Log.Debug("Module for modal {ModalName} was not found", data);
	}

	public async Task ExecuteButtonResponse(SocketMessageComponent component)
	{
		var data = component.Data.CustomId;

		var module = ModuleRegister.TryGetFromButton(data, ClientId, component.GuildId!.Value);
		
		if (module != null)
			await module.ExecuteButton(component);

		else
			Log.Debug("Module for button {ButtonName} was not found", data);
	}

	public async Task ExecuteSelectResponse(SocketMessageComponent component)
	{
		var data = component.Data.CustomId;

		var module = ModuleRegister.TryGetFromSelect(data, ClientId,component.GuildId!.Value);
		
		if (module != null)
			await module.ExecuteButton(component);

		else
			Log.Debug("Module for button {ButtonName} was not found", data);
	}
}