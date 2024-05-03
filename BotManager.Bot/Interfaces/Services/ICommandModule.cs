using BotManager.Db.Interfaces;
using Discord.WebSocket;

namespace BotManager.Bot.Interfaces.Services;

public interface ICommandModule<TConfig> : ICommandModule
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