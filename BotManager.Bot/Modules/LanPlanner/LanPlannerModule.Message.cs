using System.Text;
using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models.Modules.LanPlanner;
using Discord;

namespace BotManager.Bot.Modules.LanPlanner;

public partial class LanPlannerModule
{
	private static async Task UpdatePlanMessage(LanPlan plan, IUserMessage message)
	{
		var embeds = CreatePlanEmbed(plan);
		var components = CreatePlanButtons();

		await message.ModifyAsync(msg =>
			{
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
		List<Embed> ret = [];

		var builder = new EmbedBuilder();
		builder.WithTitle($"Planung für {plan.EventName}");
		builder.AddField("Seats belegt:", $"{plan.Members.Sum(x => x.NumberOfSeats)}/{plan.MaxSeats}");
		ret.Add(builder.Build());
		
		ret.AddRange(CreateUserGroupDetails(plan));

		return ret;
	}

	private static IEnumerable<Embed> CreateUserGroupDetails(LanPlan plan)
	{
		List<Embed> ret = [];
		var groups = plan.Members.GroupBy(x => x.SeatingGroup);

		foreach (var group in groups)
		{
			var builder = new EmbedBuilder()
				.WithTitle($"Gruppe: {group.Key}");

			foreach (var member in group)
			{
				var name = !string.IsNullOrWhiteSpace(member.Nickname) ? member.Nickname : member.UserId.ToString();
				builder.AddField(name, UserInfo(member));
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