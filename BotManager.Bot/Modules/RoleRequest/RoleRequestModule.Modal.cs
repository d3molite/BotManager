using BotManager.Bot.Extensions;
using BotManager.Bot.Modules.Definitions;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.RoleRequest;

public partial class RoleRequestModule
{
	public async Task ExecuteModal(SocketModal modal)
	{
		switch (modal.Data.CustomId)
		{
			case Modals.RoleRequestModalId:
				await ExecuteRoleRequestModal(modal);
				break;
		}
	}

	private async Task ExecuteRoleRequestModal(SocketModal modal)
	{
		var data = modal.Data.Components.ToList();
		
		var guild = client.GetGuild(guildConfig.GuildId);

		var user = guild.GetUser(modal.User.Id);
		var receiver = await client.GetUserAsync(_roleConfig.ReceiverId);

		var request = new RoleRequest()
		{
			GuildId = modal.GuildId!.Value,
			Email = data.GetString(Modals.RoleRequestEmail),
			Status = RoleRequestStatus.Open,
			UserId = modal.User.Id,
			UserNick = $"{user.Nickname} (@{user.Username})",
		};

		await receiver.SendMessageAsync(
			"",
			embed: CreateRoleRequestReceiverEmbed(request),
			components: CreateRoleRequestReceiverButtons(request)
		);

		await modal.RespondAsync("Anfrage erhalten, die Überprüfung erfolgt in Kürze.", ephemeral: true);
	}

	private static Modal CreateRoleRequestModal()
	{
		var modal = new ModalBuilder();

		modal.WithCustomId(Modals.RoleRequestModalId);
		modal.WithTitle("LAN.User Rolle anfragen.");

		modal.AddTextInput(
			"E-Mail aus deiner Ticketbestellung",
			Modals.RoleRequestEmail,
			TextInputStyle.Short,
			"max@mustermann.de",
			required: true
		);

		return modal.Build();
	}

	private static Embed CreateRoleRequestReceiverEmbed(RoleRequest request)
	{
		var embed = new EmbedBuilder();

		embed.WithTitle($"LAN.User Rollenanfrage vom {DateTime.UtcNow:dd.MM.yyyy HH:mm:ss} (UTC)");
		embed.AddField("Guild", request.GuildId);
		embed.AddField(RoleRequestEmbedFields.UserId, request.UserId);
		embed.AddField(RoleRequestEmbedFields.Status, request.Status);
		embed.AddField(RoleRequestEmbedFields.UserNick, request.UserNick);
		embed.AddField(RoleRequestEmbedFields.Email, request.Email);

		return embed.Build();
	}
}