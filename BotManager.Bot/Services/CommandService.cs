using BotManager.Bot.Interfaces.Services;
using BotManager.Bot.Modules.Definitions;
using BotManager.Bot.Modules.OrderTracking;
using BotManager.Db.Models;
using BotManager.DI;
using BotManager.Interfaces.Services.Data;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BotManager.Bot.Services;

public class CommandService(BotConfig config, DiscordSocketClient client) : ICommandService
{
	private OrderTrackingModule _orderTrackingModule;
	
	public async Task BuildCommands()
	{
		foreach (var guildConfig in config.GuildConfigs)
		{
			if (guildConfig.OrderTrackingConfig != null)
			{
				var service = DependencyManager.Provider.GetRequiredService<IOrderService>();
				_orderTrackingModule = new OrderTrackingModule(service, client);
				await _orderTrackingModule.BuildCommands(guildConfig.OrderTrackingConfig, guildConfig.GuildId);
			}
		}
	}

	public async Task ExecuteCommand(SocketSlashCommand command)
	{
		Log.Debug("Received Command {CommandName}", command.Data.Name);

		try
		{
			switch (command.CommandName)
			{
				case "order":
					await _orderTrackingModule.ExecuteCommands(command);
					break;
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

		if (data.StartsWith(ModalFields.OrderModal))
			await _orderTrackingModule.ProcessModal(modal);
		
	}

	public async Task ExecuteButtonResponse(SocketMessageComponent component)
	{
		if (component.Data.CustomId.StartsWith(ControlNames.Order))
			await _orderTrackingModule.ExecuteOrderButton(component);
	}

	public async Task ExecuteSelectResponse(SocketMessageComponent component)
	{
		if (component.Data.CustomId.StartsWith(ControlNames.Order))
			await _orderTrackingModule.ExecuteOrderSelect(component);
	}
}