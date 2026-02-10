using BotManager.Bot.Attributes;
using BotManager.Bot.Modules.Definitions;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.RoleRequest;

public partial class RoleRequestModule
{
	[MessageComponentExecutor(Components.RoleRequestButtonDeny)]
	public async Task ProcessDenyButton(SocketMessageComponent component)
	{
		var request = ReadFromComponent(component);
		
		request.Status = RoleRequestStatus.Denied;
		
		await component.Message.ModifyAsync(x =>
		{
			x.Embed = CreateRoleRequestReceiverEmbed(request);
			x.Components = CreateRoleRequestReceiverButtons(request);
		});
		
		await component.RespondAsync("Anfrage abgelehnt.", ephemeral: true);

		var user = await Client.GetUserAsync(request.UserId);

		await user.SendMessageAsync("Deine Anfrage wurde leider abgelehnt. Bei Rückfragen wende dich bitte an die CGG Orga.");
	}

	[MessageComponentExecutor(Components.RoleRequestButtonAccept)]
	public async Task ProcessAcceptButton(SocketMessageComponent component)
	{
		var request = ReadFromComponent(component);

		request.Status = RoleRequestStatus.Accepted;
		
		var guild = Client.GetGuild(request.GuildId);
		var user = guild.GetUser(request.UserId);
		var role = await guild.GetRoleAsync(ModuleConfig.RoleId);

		await user.AddRoleAsync(role);
		
		await component.RespondAsync("Anfrage angenommen.", ephemeral: true);
		
		await component.Message.ModifyAsync(x =>
		{
			x.Embed = CreateRoleRequestReceiverEmbed(request);
			x.Components = CreateRoleRequestReceiverButtons(request);
		});
	}

	private static RoleRequest ReadFromComponent(SocketMessageComponent component)
	{
		var embed = component.Message.Embeds.First();
		return ReadFromEmbed(embed);
	}
	
	private static RoleRequest ReadFromEmbed(IEmbed embed)
	{
		return new RoleRequest()
		{
			GuildId = ulong.Parse(embed.Fields.FirstOrDefault(x => x.Name == "Guild").Value),
			OrderInformation = embed.Fields.FirstOrDefault(x => x.Name == RoleRequestEmbedFields.OrderInformation).Value,
			UserId = ulong.Parse(embed.Fields.FirstOrDefault(x => x.Name == RoleRequestEmbedFields.UserId).Value),
			Status = Enum.Parse<RoleRequestStatus>(embed.Fields.FirstOrDefault(x => x.Name == RoleRequestEmbedFields.Status).Value),
			UserNick = embed.Fields.FirstOrDefault(x => x.Name == RoleRequestEmbedFields.UserNick).Value,
		};
	}
	
	private static MessageComponent CreateRoleRequestReceiverButtons(RoleRequest request)
	{
		var components = new ComponentBuilder()
						.WithButton(
							"Annehmen und Rolle zuweisen",
							Components.RoleRequestButtonAccept,
							ButtonStyle.Success,
							disabled: request.Status != RoleRequestStatus.Open
						)
						.WithButton(
							"Ablehnen",
							Components.RoleRequestButtonDeny,
							ButtonStyle.Danger,
							disabled: request.Status != RoleRequestStatus.Open
						);

		return components.Build();
	}
}