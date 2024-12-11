using System.ComponentModel.DataAnnotations;
using Demolite.Db.Models;

namespace BotManager.Db.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class BotConfig : AbstractDbItem
{
	[MaxLength(50)]
	public required string Name { get; set; }
	
	[MaxLength(255)]
	public required string Token { get; set; }
	
	[MaxLength(255)]
	public string? Presence { get; set; }
	
	public bool Active { get; set; }
	
	public bool Debug { get; set; }

	public IEnumerable<GuildConfig> GuildConfigs { get; set; }
}