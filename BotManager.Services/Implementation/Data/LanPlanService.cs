using BotManager.Db.Models.Modules.LanPlanner;
using BotManager.Interfaces.Services.Data;
using Demolite.Db.Enum;
using Demolite.Db.Interfaces;
using Demolite.Db.Result;

namespace BotManager.Services.Implementation.Data;

public class LanPlanService(IDbRepository<LanPlan> planRepository, IDbRepository<LanMember> memberRepository) : ILanPlanService
{
	public async Task<LanPlan?> GetLanPlanAsync(ulong messageId)
	{
		var result = await planRepository.GetCustomAsync(x => x.MessageId == messageId);

		if (result is null)
			return null;

		result.OperationType = Operation.Updated;

		foreach (var member in result.Members)
		{
			member.OperationType = Operation.Updated;
		}

		return result;
	}
	
	public async Task<LanPlan?> GetLanPlanAsync(string guid)
	{
		var result = await planRepository.GetAsync(guid);

		if (result is null)
			return null;

		result.OperationType = Operation.Updated;

		foreach (var member in result.Members)
		{
			member.OperationType = Operation.Updated;
		}

		return result;
	}

	public async Task<IDbResult<LanPlan>> CreatePlanAsync(LanPlan plan)
		=> await planRepository.CreateAsync(plan);

	public async Task<IDbResult<LanPlan>> UpdatePlanAsync(LanPlan plan)
	{
		if (plan.Members is { Count: > 0 })
		{
			var memberResult = await memberRepository.CrudManyAsync(plan.Members);
			
			var isFailed = memberResult.FirstOrDefault(x => !x.Success);
			
			if (isFailed != null)
				return DbResult<LanPlan>.Failed(plan, isFailed.ErrorMessage);
		}
		
		return await planRepository.CrudAsync(plan);
	}

	public async Task<IDbResult<LanMember>> RemoveFromPlanAsync(LanMember member)
	{
		member.OperationType = Operation.Removed;
		return await memberRepository.CrudAsync(member);
	}

	public async Task<IDbResult<LanMember>> UpdateMemberAsync(LanMember member)
	{
		member.OperationType = Operation.Updated;
		return await memberRepository.CrudAsync(member);
	}
}