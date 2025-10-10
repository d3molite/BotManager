using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BotManager.Db.Models.Modules.ModMail;
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
	
	[MaxLength(1000)]
	public string AdminUserIds { get; set; } = string.Empty;
	
	[NotMapped]
	public IEnumerable<ulong> AdminIds => string.IsNullOrEmpty(AdminUserIds) ? [] : AdminUserIds.Split(',').Select(ulong.Parse);
	
	public bool Active { get; set; }
	
	public bool Debug { get; set; }

	public IEnumerable<GuildConfig> GuildConfigs { get; set; }
	
	public IEnumerable<ModMailConfig> ModMailConfigs { get; set; }
}