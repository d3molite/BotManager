using System.ComponentModel.DataAnnotations;
using Demolite.Db.Models;

namespace BotManager.Db.Models.Modules.Birthdays;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class BirthdayConfig : AbstractDbItem
{
	public ulong PingChannelId { get; set; }

	public List<Birthday> Birthdays { get; set; } = [];

	[MaxLength(40)]
	public string GuildConfigId { get; set; } = null!;

	public GuildConfig GuildConfig { get; set; } = null!;
}