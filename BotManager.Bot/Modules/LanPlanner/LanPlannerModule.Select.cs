using BotManager.Bot.Attributes;
using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models.Modules.LanPlanner;
using Demolite.Db.Enum;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.LanPlanner;

public partial class LanPlannerModule
{
	[MessageComponentExecutor(Components.PlanUserSelectMenu)]
	public async Task ExecuteSelect(SocketMessageComponent component)
	{
		var message = await component.Channel.GetMessageAsync(component.Message.Reference.MessageId.Value);
		var data = component.Data.Values.First();

		if (!ulong.TryParse(data, out var userId))
		{
			await component.UpdateAsync(msg =>
			{
				msg.Content = "User id invalid!";
				msg.Components = null;
			});
			return;
		}

		var plan = await planService.GetLanPlanAsync(message.Id);

		if (plan!.Members.Any(x => x.UserId == userId))
		{
			await component.UpdateAsync(msg =>
			{
				msg.Content = "User is already in the plan!";
				msg.Components = null;
			});
			return;
		}

		var guild = client.GetGuild(GuildConfig.GuildId);
		var user = guild.GetUser(userId);
		
		plan.Members.Add(new LanMember()
		{
			Id = Guid.NewGuid().ToString(),
			UserId = userId,
			Nickname = user.Username,
			OperationType = Operation.Created,
		});

		var result = await planService.UpdatePlanAsync(plan);

		if (!result.Success)
		{
			await component.UpdateAsync(msg =>
				{
					msg.Content = "Ein Fehler ist Aufgetreten.";
					msg.Embed = new EmbedBuilder().AddField("Error:", result.ErrorMessage).Build();
					msg.Components = null;
				}
			);

			return;
		}
		
		var role = await guild.GetRoleAsync(ModuleConfig.MemberRoleId);
		await user.AddRoleAsync(role);
		
		await UpdatePlanMessage(plan, (IUserMessage)message);

		await component.UpdateAsync(msg =>
			{
				msg.Content = $"Member <@{userId}> was added to the plan!";
				msg.Components = null;
			}
		);
	}

	[MessageComponentExecutor(Components.PlanUserEditSelectMenu)]
	public async Task ExecuteUserSelect(SocketMessageComponent component)
	{
		var message = await component.Channel.GetMessageAsync(component.Message.Reference.MessageId.Value);
		var data = component.Data.Values.First();

		if (!ulong.TryParse(data, out var userId))
		{
			await component.UpdateAsync(msg =>
			{
				msg.Content = "User id invalid!";
				msg.Components = null;
			});
			return;
		}
		
		var plan = await planService.GetLanPlanAsync(message.Id);
		var member = plan!.Members.FirstOrDefault(x => x.UserId == userId);
		
		if (member is null)
		{
			await component.UpdateAsync(msg =>
			{
				msg.Content = "User is not in the plan!";
				msg.Components = null;
			});
			return;
		}

		await component.RespondWithModalAsync(CreateUserEditModal(plan, member));
	}

	[MessageComponentExecutor(Components.PlanUserRemoveSelectMenu)]
	public async Task ExecuteUserRemove(SocketMessageComponent component)
	{
		var message = await component.Channel.GetMessageAsync(component.Message.Reference.MessageId.Value);
		var data = component.Data.Values.First();

		if (!ulong.TryParse(data, out var userId))
		{
			await component.UpdateAsync(msg =>
			{
				msg.Content = "User id invalid!";
				msg.Components = null;
			});
			return;
		}

		var plan = await planService.GetLanPlanAsync(message.Id);

		var member = plan!.Members.FirstOrDefault(x => x.UserId == userId);
		
		if (member is null)
		{
			await component.UpdateAsync(msg =>
			{
				msg.Content = "User is not in the plan!";
				msg.Components = null;
			});
			return;
		}

		var guild = client.GetGuild(GuildConfig.GuildId);
		var user = guild.GetUser(userId);
		
		var result = await planService.RemoveFromPlanAsync(member);

		if (!result.Success)
		{
			await component.UpdateAsync(msg =>
				{
					msg.Content = "Ein Fehler ist Aufgetreten.";
					msg.Embed = new EmbedBuilder().AddField("Error:", result.ErrorMessage).Build();
					msg.Components = null;
				}
			);

			return;
		}
		
		var role = await guild.GetRoleAsync(ModuleConfig.MemberRoleId);
		await user.RemoveRoleAsync(role);
		
		await UpdatePlanMessage(plan, (IUserMessage)message);

		await component.UpdateAsync(msg =>
			{
				msg.Content = $"Member <@{userId}> was added to the plan!";
				msg.Components = null;
			}
		);
	}
}