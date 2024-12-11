using System.ComponentModel.DataAnnotations;
using Demolite.Db.Models;

namespace BotManager.Db.Models.Modules.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class LoggingConfig : AbstractDbItem
{
	public ulong LoggingChannelId { get; set; }
	
	public virtual GuildConfig GuildConfig { get; set; } = null!;

	[MaxLength(40)]
	public string GuildConfigId { get; set; } = null!;
}