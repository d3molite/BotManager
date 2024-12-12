using Discord.WebSocket;

namespace BotManager.Bot.Interfaces.Modules;

public interface ICommandModule<in TConfig> : ICommandModule
{
	public Task BuildCommands(TConfig config, ulong guildId);
	
}

public interface ICommandModule
{
	public Task ExecuteCommands(SocketSlashCommand command);

	public Task ExecuteButton(SocketMessageComponent component);

	public Task ExecuteModal(SocketModal modal);

	public Task ExecuteSelect(SocketMessageComponent component);
}