using BotManager.Bot.Interfaces.Modules;
using BotManager.Db.Models.Modules.Voice;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.VoiceChannel;

public class VoiceChannelModule() : ICommandModule<VoiceChannelConfig>
{
	public Task BuildCommands(VoiceChannelConfig config, ulong guildId)
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