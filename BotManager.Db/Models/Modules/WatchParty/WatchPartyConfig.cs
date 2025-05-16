using BotManager.Db.Models.Abstract;

namespace BotManager.Db.Models.Modules.WatchParty;

public class WatchPartyConfig : AbstractGuildConfig
{
	public ulong PingRoleId { get; set; }
}