using BotManager.Db.Models.Modules.Image;
using BotManager.Db.Models.Modules.Order;
using BotManager.Db.Models.Modules.Voice;
using EfExtensions.Items.Model;

namespace BotManager.Db.Models;

public class GuildConfig : DbItem<string>
{
	public ulong GuildId { get; set; }
	
	public OrderTrackingConfig? OrderTrackingConfig { get; set; }
	
	public VoiceChannelConfig? VoiceChannelConfig { get; set; }
	
	public ImageConfig? ImageConfig { get; set; }
}