using BotManager.Db.Models.Modules.LanPlanner;
using Demolite.Db.Interfaces;

namespace BotManager.Interfaces.Services.Data;

public interface ILanPlanService
{
	public Task<LanPlan?> GetLanPlanAsync(ulong messageId);
	
	public Task<LanPlan?> GetLanPlanAsync(string guid);

	public Task<IDbResult<LanPlan>> CreatePlanAsync(LanPlan plan);

	public Task<IDbResult<LanPlan>> UpdatePlanAsync(LanPlan plan);
	
	public Task<IDbResult<LanMember>> RemoveFromPlanAsync(LanMember member);

	public Task<IDbResult<LanMember>> UpdateMemberAsync(LanMember member);
}