using BotManager.Bot.Services.Register;
using BotManager.Db.Models;
using Discord.WebSocket;
using Serilog;

namespace BotManager.Bot.Services.Commands;

public partial class CommandModuleService(BotConfig config, DiscordSocketClient client)
{
	

	public async Task BuildCommands()
	{
		await BuildCommandsInternal();
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

		var module = ModuleRegister.TryGetFromSelect(data, ClientId, component.GuildId!.Value);

		if (module != null)
			await module.ExecuteButton(component);

		else
			Log.Debug("Module for button {ButtonName} was not found", data);
	}
}