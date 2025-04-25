using BotManager.Bot.Extensions;
using BotManager.Bot.Interfaces.Modules;
using BotManager.Db.Models;
using Discord.WebSocket;
using Serilog;

namespace BotManager.Bot.Modules.Core;

public class AbstractCommandModuleBase<T>(DiscordSocketClient client, GuildConfig config)
	: AbstractModuleBase<T>(config), IRefCommandModule
	where T : class
{
	protected DiscordSocketClient Client => client;

	/// <summary>
	/// Builds and registers all commands for this module.
	/// </summary>
	public async Task BuildCommands()
	{
		var guild = Client.GetGuild(GuildConfig.GuildId);

		foreach (var builder in GetType().GetCommandBuilders())
		{
			await (Task)builder.Invoke(this, [guild])!;
		}
	}

	/// <summary>
	/// Executes a provided command within this module.
	/// </summary>
	/// <param name="slashCommand">The command to execute.</param>
	public async Task ExecuteCommand(SocketSlashCommand slashCommand)
	{
		var task = GetType().GetCommandExecutor(slashCommand.CommandName);

		if (task is null)
		{
			Log.Error(
				"Command {CommandName} was not found on Module {ModuleName}",
				slashCommand.CommandName,
				GetType().Name
			);
		}

		await (Task)task?.Invoke(this, [slashCommand])!;
	}

	/// <summary>
	/// Processes a provided modal within this module.
	/// </summary>
	/// <param name="modal">The modal to process.</param>
	public async Task ExecuteModal(SocketModal modal)
	{
		var task = GetType().GetModalExecutor(modal.Data.CustomId);

		if (task is null)
		{
			Log.Error(
				"Modal {ModalId} was not found on Module {ModuleName}",
				modal.Data.CustomId,
				GetType().Name
			);
		}

		await (Task)task?.Invoke(this, [modal])!;
	}

	/// <summary>
	/// Processes a provided message component within this module.
	/// </summary>
	/// <param name="component">The component to process.</param>
	public async Task ExecuteMessageComponent(SocketMessageComponent component)
	{
		var task = GetType().GetComponentExecutor(component.Data.CustomId);

		if (task is null)
		{
			Log.Error(
				"Component {ComponentId} was not found on Module {ModuleName}",
				component.Data.CustomId,
				GetType().Name
			);
		}

		await (Task)task?.Invoke(this, [component])!;
	}
}