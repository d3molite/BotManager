using BotManager.Db.Models.Modules.Order;
using EfExtensions.Items.Model;

namespace BotManager.Db.Models;

public class GuildConfig : DbItem<string>
{
	public ulong GuildId { get; set; }
	
	public OrderTrackingConfig? OrderTrackingConfig { get; set; }
}