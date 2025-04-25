using BotManager.Bot.Interfaces.Modules;

namespace BotManager.Bot.Modules.Models;

public class RefModuleInfo(IRefCommandModule module, ulong clientId, ulong guildId)
{
	public ulong GuildId { get; } = guildId;
	
	public ulong ClientId { get; } = clientId;
	
	public IRefCommandModule Module { get; } = module;
}