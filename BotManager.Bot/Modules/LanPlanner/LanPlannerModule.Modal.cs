using BotManager.Bot.Attributes;
using BotManager.Bot.Extensions;
using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models.Modules.LanPlanner;
using Demolite.Db.Enum;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.LanPlanner;

public partial class LanPlannerModule
{
	[ModalExecutor(Modals.PlanModalId)]
	public async Task ProcessPlanModal(SocketModal modal)
	{
		var data = modal.Data.Components.ToList();
		await modal.RespondAsync("Creating", ephemeral: true);

		var message = await modal.Channel.SendMessageAsync("_ _");

		var plan = new LanPlan()
		{
			OperationType = Operation.Created,
			MessageId = message.Id,
			OwnerId = modal.User.Id,
			EventName = data.Get(Modals.PlanModalName).Value,
			MaxSeats = ParseModalInt(data.Get(Modals.PlanModalMaxSeats).Value),
		};

		var created = await planService.CreatePlanAsync(plan);

		if (!created.Success)
		{
			await message.DeleteAsync();
			return;
		}

		await UpdatePlanMessage(created.Item, message);
	}

	[ModalExecutor(Modals.PlanEditModalId)]
	public async Task ProcessEditModal(SocketModal modal)
	{
		var data = modal.Data.Components.ToList();
		await modal.RespondAsync("Editing", ephemeral: true);

		var ids = data.Get(Modals.PlanEditModalPlanId).Value;
		var planId = ids.Split(":")[0];
		var userId = ids.Split(":")[1];

		var plan = await planService.GetLanPlanAsync(planId);

		if (plan is null)
		{
			await modal.RespondAsync("Der plan wurde nicht gefunden", ephemeral: true);
			return;
		}

		var member = plan.Members.FirstOrDefault(x => x.Id == userId);

		if (member is null)
		{
			await modal.RespondAsync("Member wurde nicht gefunden", ephemeral: true);
			return;
		}

		try
		{
			var groupId = data.Get(Modals.PlanEditModalGroup).Value;

			if (!string.IsNullOrEmpty(groupId))
				member.SeatingGroup = groupId;
		}
		catch
		{
			// do nothing, as the user self-edit modal has no id
		}

		if (data.Get(Modals.PlanEditModalNick).Value is { } nick)
			member.Nickname = nick;

		if (data.Get(Modals.PlanEditModalSeatA).Value is { } seatA)
			member.SeatA = seatA;

		if (data.Get(Modals.PlanEditModalSeatB).Value is { } seatB)
			member.SeatB = seatB;

		var result = await planService.UpdateMemberAsync(member);

		if (!result.Success)
		{
			await modal.RespondAsync("Fehler beim aktualisieren", ephemeral: true);
			return;
		}

		var message = await modal.Channel.GetMessageAsync(plan.MessageId);
		await UpdatePlanMessage((await planService.GetLanPlanAsync(plan.Id))!, (IUserMessage)message);
	}

	private static Modal CreatePlanStartModal()
	{
		var builder = new ModalBuilder()
			.WithTitle("Neuer Plan")
			.WithCustomId(Modals.PlanModalId)
			.AddTextInput("Name", Modals.PlanModalName, placeholder: "CGG 202X")
			.AddTextInput("Maximale Seats", Modals.PlanModalMaxSeats, placeholder: "80");

		return builder.Build();
	}

	private static Modal CreateUserEditModal(LanPlan plan, LanMember member)
	{
		var builder = new ModalBuilder()
			.WithTitle("Teilnehmer bearbeiten")
			.WithCustomId(Modals.PlanEditModalId)
			.AddTextInput("Gruppe", Modals.PlanEditModalGroup, value: member.SeatingGroup)
			.AddTextInput(
				"Nickname",
				Modals.PlanEditModalNick,
				placeholder: "Helga",
				value: member.Nickname,
				required: true
			)
			.AddTextInput("Seat A", Modals.PlanEditModalSeatA, value: member.SeatA, required: false)
			.AddTextInput("Seat B", Modals.PlanEditModalSeatB, value: member.SeatB, required: false)
			.AddTextInput("NICHT BEARBEITEN", Modals.PlanEditModalPlanId, value: $"{plan.Id}:{member.Id}");

		return builder.Build();
	}

	private static Modal CreateSelfUserEditModal(LanPlan plan, LanMember member)
	{
		var builder = new ModalBuilder()
			.WithTitle("Teilnehmer bearbeiten")
			.WithCustomId(Modals.PlanEditModalId)
			.AddTextInput(
				"Nickname",
				Modals.PlanEditModalNick,
				placeholder: "Helga",
				value: member.Nickname,
				required: true
			)
			.AddTextInput("Seat A", Modals.PlanEditModalSeatA, value: member.SeatA, required: false)
			.AddTextInput("Seat B", Modals.PlanEditModalSeatB, value: member.SeatB, required: false)
			.AddTextInput("NICHT BEARBEITEN", Modals.PlanEditModalPlanId, value: $"{plan.Id}:{member.Id}");

		return builder.Build();
	}

	private async Task<MessageComponent?> RemoveUserSelectMenu(LanPlan plan)
	{
		if (plan.Members.Count < 25)
		{
			var builder = new ComponentBuilder().WithRows(
				new List<ActionRowBuilder>()
				{
					new ActionRowBuilder().WithSelectMenu(
						new SelectMenuBuilder()
							.WithCustomId(Components.PlanUserRemoveSelectMenu)
							.WithOptions(
								plan
									.Members.Select(x
										=> new SelectMenuOptionBuilder()
											.WithLabel(
												!string.IsNullOrEmpty(x.Nickname) ? x.Nickname : x.UserId.ToString()
											)
											.WithValue(x.UserId.ToString())
									)
									.ToList()
							)
					), }
			);

			return builder.Build();
		}

		var chunks = plan.Members.Chunk(24);

		var builderChunked = new ComponentBuilder().WithRows(
			chunks
				.Select((chunk, index) => new ActionRowBuilder().WithComponents(
						[
							new SelectMenuBuilder()
								.WithCustomId(Components.PlanUserRemoveSelectMenu + index)
								.WithOptions(
									chunk
										.Select(user
											=> new SelectMenuOptionBuilder()
												.WithLabel(
													!string.IsNullOrEmpty(user.Nickname)
														? user.Nickname
														: user.UserId.ToString()
												)
												.WithValue(user.UserId.ToString())
										)
										.ToList()
								)
						]
					)
				)
				.ToList()
		);

		return builderChunked.Build();
	}

	private async Task<MessageComponent?> CreateUserSelectMenu(ulong componentGuildId, LanPlan plan)
	{
		var guild = client.GetGuild(componentGuildId);

		var users = (await guild.GetUsersAsync().ToArrayAsync())
			.SelectMany(x => x)
			.Where(x => plan.Members.All(y => y.UserId != x.Id))
			.OrderBy(x => x.DisplayName)
			.ToArray();

		if (users.Length < 25)
		{
			var builder = new ComponentBuilder().WithRows(
				new List<ActionRowBuilder>()
				{
					new ActionRowBuilder().WithSelectMenu(
						new SelectMenuBuilder()
							.WithCustomId(Components.PlanUserSelectMenu)
							.WithOptions(
								users
									.Select(x => new SelectMenuOptionBuilder()
										.WithLabel(x.DisplayName)
										.WithValue(x.Id.ToString())
									)
									.ToList()
							)
					), }
			);

			return builder.Build();
		}

		var chunks = users.Chunk(24);

		var builderChunked = new ComponentBuilder().WithRows(
			chunks
				.Select((chunk, index) => new ActionRowBuilder().WithComponents(
						[
							new SelectMenuBuilder()
								.WithCustomId(Components.PlanUserSelectMenu + index)
								.WithOptions(
									chunk
										.Select(user
											=> new SelectMenuOptionBuilder()
												.WithLabel(user.DisplayName)
												.WithValue(user.Id.ToString())
										)
										.ToList()
								)
						]
					)
				)
				.ToList()
		);

		return builderChunked.Build();
	}

	private async Task<MessageComponent> EditUserSelectMenu(LanPlan plan)
	{
		if (plan.Members.Count < 25)
		{
			var builder = new ComponentBuilder().WithRows(
				new List<ActionRowBuilder>()
				{
					new ActionRowBuilder().WithSelectMenu(
						new SelectMenuBuilder()
							.WithCustomId(Components.PlanUserEditSelectMenu)
							.WithOptions(
								plan
									.Members.Select(x
										=> new SelectMenuOptionBuilder()
											.WithLabel(
												!string.IsNullOrEmpty(x.Nickname) ? x.Nickname : x.UserId.ToString()
											)
											.WithValue(x.UserId.ToString())
									)
									.ToList()
							)
					), }
			);

			return builder.Build();
		}

		var chunks = plan.Members.Chunk(24);

		var builderChunked = new ComponentBuilder().WithRows(
			chunks
				.Select((chunk, index) => new ActionRowBuilder().WithComponents(
						[
							new SelectMenuBuilder()
								.WithCustomId(Components.PlanUserEditSelectMenu + index)
								.WithOptions(
									chunk
										.Select(user
											=> new SelectMenuOptionBuilder()
												.WithLabel(
													!string.IsNullOrEmpty(user.Nickname)
														? user.Nickname
														: user.UserId.ToString()
												)
												.WithValue(user.UserId.ToString())
										)
										.ToList()
								)
						]
					)
				)
				.ToList()
		);

		return builderChunked.Build();
	}
}