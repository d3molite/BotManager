using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BotManager.Db.Models.Modules.AntiSpam;
using BotManager.Db.Models.Modules.Birthdays;
using BotManager.Db.Models.Modules.Image;
using BotManager.Db.Models.Modules.Logging;
using BotManager.Db.Models.Modules.Order;
using BotManager.Db.Models.Modules.Voice;
using Demolite.Db.Models;

namespace BotManager.Db.Models;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class GuildConfig : AbstractDbItem
{
	[MaxLength(50)]
	public string GuildName { get; set; } = "";

	public ulong GuildId { get; set; }

	public OrderTrackingConfig? OrderTrackingConfig { get; set; }
	
	public VoiceChannelConfig? VoiceChannelConfig { get; set; }
	
	public ImageConfig? ImageConfig { get; set; }
	
	public BirthdayConfig? BirthdayConfig { get; set; }

	public LoggingConfig? LoggingConfig { get; set; }
	
	public AntiSpamConfig? AntiSpamConfig { get; set; }

	[NotMapped]
	public bool HasOrderTrackingModule => OrderTrackingConfig != null;

	[NotMapped]
	public bool HasVoiceChannelModule => VoiceChannelConfig != null;
	
	[NotMapped]
	public bool HasImageModule => ImageConfig != null;
	
	[NotMapped]
	public bool HasBirthdayModule => BirthdayConfig != null;
	
	[NotMapped]
	public bool HasLoggingModule => LoggingConfig != null;
	
	[NotMapped]
	public bool HasAntiSpamModule => AntiSpamConfig != null;

	public virtual BotConfig BotConfig { get; set; } = null!;

	[MaxLength(40)]
	public string BotConfigId { get; set; } = null!;
}