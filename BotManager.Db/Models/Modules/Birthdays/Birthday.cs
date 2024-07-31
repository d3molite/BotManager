using EfExtensions.Items.Model;

namespace BotManager.Db.Models.Modules.Birthdays;

public class Birthday : DbItem<string>
{
	public DateOnly Date { get; set; }

	public string ConfigId { get; set; }
	
	public ulong GuildId { get; set; }

	public ulong UserId { get; set; }
}