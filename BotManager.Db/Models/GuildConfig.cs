using BotManager.Db.Models.Modules.Birthdays;
using BotManager.Db.Models.Modules.Image;
using BotManager.Db.Models.Modules.Logging;
using BotManager.Db.Models.Modules.Order;
using BotManager.Db.Models.Modules.Voice;
using Demolite.Db.Models;

namespace BotManager.Db.Models;

public class GuildConfig : AbstractDbItem
{
	public ulong GuildId { get; set; }
	public OrderTrackingConfig? OrderTrackingConfig { get; set; }
	
	public VoiceChannelConfig? VoiceChannelConfig { get; set; }
	public ImageConfig? ImageConfig { get; set; }
	public BirthdayConfig? BirthdayConfig { get; set; }
	public LoggingConfig? LoggingConfig { get; set; }
	
	public virtual BotConfig BotConfig { get; set; }
	
	public string BotConfigId { get; set; }
}