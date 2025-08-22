using BotManager.Db.Models.Abstract;

namespace BotManager.Db.Models.Modules.LanPlanner;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class LanPlannerConfig : AbstractGuildConfig
{
	public ulong PlannerChannelId { get; set; }
	
	public ulong MemberRoleId { get; set; }
}