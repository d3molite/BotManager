using BotManager.Bot.Attributes;
using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models.Modules.LanPlanner;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.LanPlanner;

public partial class LanPlannerModule
{
	[MessageComponentExecutor(Components.PlanButtonAdd)]
	public async Task AddToPlan(SocketMessageComponent component)
	{
		var plan = await CheckForOwner(component);
		if (plan is null) return;

		await component.RespondAsync(
			"Wähle einen Teilnehmer aus!",
			ephemeral: true,
			components: await CreateUserSelectMenu(component.GuildId!.Value)
		);
	}

	[MessageComponentExecutor(Components.PlanButtonReload)]
	public async Task ReloadPlan(SocketMessageComponent component)
	{
		var plan = await CheckForOwner(component);
		if (plan is null) return;

		await UpdatePlanMessage(plan, component.Message);

		await component.RespondAsync("Neu geladen!", ephemeral: true);
	}

	[MessageComponentExecutor(Components.PlanButtonEdit)]
	public async Task EditUser(SocketMessageComponent component)
	{
		var plan = await GetPlanFromMessage(component);
		if (plan is null) return;

		if (plan.OwnerId == component.User.Id)
		{
			await component.RespondAsync(
				"Wähle einen Teilnehmer aus!",
				ephemeral: true,
				components: await CreateUserSelectMenu(plan)
			);

			return;
		}

		var member = plan.Members.FirstOrDefault(x => x.UserId == component.User.Id);

		if (member is null)
		{
			await component.RespondAsync("Du bist noch kein Teilnehmer!", ephemeral: true);
			return;
		}

		await component.RespondWithModalAsync(CreateSelfUserEditModal(plan, member));
	}

	private async Task<LanPlan?> CheckForOwner(SocketMessageComponent component)
	{
		var order = await planService.GetLanPlanAsync(component.Message.Id);
		var userId = component.User.Id;

		if (order!.OwnerId == userId) return order;

		await component.RespondAsync("Du bist nicht besitzer dieses Plans!", ephemeral: true);
		return null;
	}

	private async Task<LanPlan?> GetPlanFromMessage(SocketMessageComponent component)
	{
		var order = await planService.GetLanPlanAsync(component.Message.Id);

		return order;
	}
}