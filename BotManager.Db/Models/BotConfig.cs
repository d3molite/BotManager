using EfExtensions.Items.Model;

namespace BotManager.Db.Models;

public class BotConfig : DbItem<string>
{
	public required string Name { get; set; }
	
	public required string Token { get; set; }
	
	public string? Presence { get; set; }
	
	public bool Active { get; set; }
	
	public bool Debug { get; set; }
	
	public IEnumerable<GuildConfig> GuildConfigs { get; set; }
}