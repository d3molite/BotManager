using System.ComponentModel.DataAnnotations;
using Demolite.Db.Models;
using JetBrains.Annotations;

namespace BotManager.Db.Models.Modules.ModMail;

[UsedImplicitly]
public class ModMailConfig : AbstractDbItem
{
	public ulong GuildId { get; set; }
	
	public ulong ForumChannelId { get; set; }
	
	public ulong? PingRoleId { get; set; }
	
	public virtual BotConfig BotConfig { get; set; } = null!;

	[MaxLength(40)]
	public string BotConfigId { get; set; } = null!;
}