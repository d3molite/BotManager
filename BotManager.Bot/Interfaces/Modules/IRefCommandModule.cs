using Discord.WebSocket;

namespace BotManager.Bot.Interfaces.Modules;

public interface IRefCommandModule
{
	public Task BuildCommands();

	public Task ExecuteCommand(SocketSlashCommand slashCommand);

	public Task ExecuteModal(SocketModal modal);

	public Task ExecuteMessageComponent(SocketMessageComponent component);
}