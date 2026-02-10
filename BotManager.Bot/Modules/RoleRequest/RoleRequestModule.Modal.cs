using BotManager.Bot.Attributes;
using BotManager.Bot.Extensions;
using BotManager.Bot.Modules.Definitions;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.RoleRequest;

public partial class RoleRequestModule
{
	[ModalExecutor(Modals.RoleRequestModalId)]
	public async Task ExecuteRoleRequestModal(SocketModal modal)
	{
		var data = modal.Data.Components.ToList();
		
		var guild = Client.GetGuild(GuildConfig.GuildId);

		var user = guild.GetUser(modal.User.Id);
		var receiver = await Client.GetUserAsync(ModuleConfig.ReceiverId);

		var request = new RoleRequest()
		{
			GuildId = modal.GuildId!.Value,
			OrderInformation = data.GetString(Modals.RoleRequestInfo),
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
			"Barcode-Nummer vom Ticket (cgg123...)",
			Modals.RoleRequestInfo,
			TextInputStyle.Short,
			"cgg1234567...",
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
		embed.AddField(RoleRequestEmbedFields.OrderInformation, request.OrderInformation);

		return embed.Build();
	}
}