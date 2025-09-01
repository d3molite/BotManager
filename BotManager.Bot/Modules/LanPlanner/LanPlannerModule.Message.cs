using System.Text;
using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models.Modules.LanPlanner;
using Discord;

namespace BotManager.Bot.Modules.LanPlanner;

public partial class LanPlannerModule
{
	private static async Task UpdatePlanMessage(LanPlan plan, IUserMessage message)
	{
		var messageText = CreatePlanMessage(plan);
		var embeds = CreatePlanEmbed(plan);
		var components = CreatePlanButtons();

		await message.ModifyAsync(msg =>
			{
				msg.Content = messageText;
				msg.Embeds = embeds.ToArray();
				msg.Components = components;
			}
		);
	}

	private static MessageComponent CreatePlanButtons()
	{
		return new ComponentBuilder()
			.WithButton("Hinzufügen", Components.PlanButtonAdd)
			.WithButton("Entfernen", Components.PlanButtonRemove)
			.WithButton("Bearbeiten", Components.PlanButtonEdit)
			.WithButton("Plan Löschen", Components.PlanButtonDelete, ButtonStyle.Danger, row: 1)
			.WithButton("Neu laden", Components.PlanButtonReload, ButtonStyle.Secondary, row: 1)
			.Build();
	}

	private static IEnumerable<Embed> CreatePlanEmbed(LanPlan plan)
	{
		return CreateUserGroupDetails(plan);
	}

	private static string CreatePlanMessage(LanPlan plan)
	{
		var builder = new StringBuilder();
		builder.AppendLine($"Planung für **{plan.EventName}**");
		builder.AppendLine($"Seats belegt: {plan.Members.Sum(x => x.NumberOfSeats)}/{plan.MaxSeats}");
		builder.AppendLine($"Teilnehmer: {plan.Members.Count}");
		builder.AppendLine($"- Beginner: {plan.Members.Sum(x => x.BeginnerCount)}");
		builder.AppendLine($"- Expert: {plan.Members.Sum(x => x.ExpertCount)}");
		builder.AppendLine($"- Legend: {plan.Members.Sum(x => x.LegendCount)}");
		return builder.ToString();
	}

	private static List<Embed> CreateUserGroupDetails(LanPlan plan)
	{
		List<Embed> ret = [];
		var groups = plan.Members.GroupBy(x => x.SeatingGroup);

		foreach (var group in groups.OrderBy(x => x.Key))
		{
			var builder = new EmbedBuilder()
				.WithTitle($"Gruppe: {group.Key}");

			foreach (var member in group)
			{
				var name = !string.IsNullOrWhiteSpace(member.Nickname) ? member.Nickname : member.UserId.ToString();
				builder.AddField(name, UserInfo(member), inline:true);
			}
			
			ret.Add(builder.Build());
		}

		return ret;
	}

	private static string UserInfo(LanMember member)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"User: <@{member.UserId}>");
		sb.AppendLine($"Seats: {member.SeatDescriptions}");
		
		return sb.ToString();
	}
}