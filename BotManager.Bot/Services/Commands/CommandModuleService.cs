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
			var refModule = ModuleRegister.TryGetFromRefCommand(command.CommandName, ClientId, command.GuildId!.Value);

			if (refModule != null)
			{
				await refModule.ExecuteCommand(command);
			}
			else
			{
				Log.Debug("RefModule for command {CommandName} was not found", command.CommandName);
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

		var refModule = ModuleRegister.TryGetFromRefModal(data, ClientId, modal.GuildId!.Value);

		if (refModule != null)
		{
			await refModule.ExecuteModal(modal);
		}
		else
		{
			Log.Debug("RefModule for modal {CommandName} was not found", data);
		}
	}

	public async Task ExecuteComponentResponse(SocketMessageComponent component)
	{
		var data = component.Data.CustomId;

		var guildId = component.GuildId ?? ulong.Parse(
			component.Message.Embeds.First()
				.Fields.First(f => f.Name == "Guild")
				.Value
		);

		var refModule = ModuleRegister.TryGetFromRefComponent(data, ClientId, guildId);

		if (refModule != null)
		{
			await refModule.ExecuteMessageComponent(component);
		}
		else
		{
			Log.Debug("RefModule for component {CommandName} was not found", data);
		}
	}
}