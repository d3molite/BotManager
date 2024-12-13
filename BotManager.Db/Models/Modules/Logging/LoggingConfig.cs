using BotManager.Db.Models.Abstract;

namespace BotManager.Db.Models.Modules.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class LoggingConfig : AbstractGuildConfig
{
	public ulong LoggingChannelId { get; set; }
	
	public ulong? CriticalMessageChannelId { get; set; }
}