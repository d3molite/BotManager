using BotManager.Db.Models.Modules.Logging;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Logging;

public partial class LoggingModule
{
	public Task BuildCommands(LoggingConfig config, ulong guildId)
		=> throw new NotImplementedException();

	public Task ExecuteCommands(SocketSlashCommand command)
		=> throw new NotImplementedException();

	public Task ExecuteButton(SocketMessageComponent component)
		=> throw new NotImplementedException();

	public Task ExecuteModal(SocketModal modal)
		=> throw new NotImplementedException();

	public Task ExecuteSelect(SocketMessageComponent component)
		=> throw new NotImplementedException();
}