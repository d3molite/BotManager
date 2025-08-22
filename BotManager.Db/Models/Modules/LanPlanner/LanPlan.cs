using Demolite.Db.Models;

namespace BotManager.Db.Models.Modules.LanPlanner;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class LanPlan : AbstractDbItem
{
	public ulong OwnerId { get; set; }

	public ulong MessageId { get; set; }

	public int MaxSeats { get; set; }
	
	public string EventName { get; set; }

	public List<LanMember> Members { get; set; } = [];
}