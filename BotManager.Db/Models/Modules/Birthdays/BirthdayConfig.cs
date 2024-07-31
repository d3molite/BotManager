using EfExtensions.Items.Model;

namespace BotManager.Db.Models.Modules.Birthdays;

public class BirthdayConfig : DbItem<string>
{
	public string GuildConfigId { get; set; }

	public GuildConfig Parent { get; set; } = null!;

	public ulong PingChannelId { get; set; }
	
	public List<Birthday> Birthdays { get; set; }
}