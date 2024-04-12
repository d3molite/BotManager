using BotManager.Db.Models.Modules.Order;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.OrderTracking;

public partial class OrderTrackingModule
{
	public async Task BuildCommands(OrderTrackingConfig orderTrackingConfig, ulong guildId)
	{
		await BuildOrderCommand(guildId);
	}

	public async Task ExecuteCommands(SocketSlashCommand command)
	{
		await ExecuteOrderCommand(command);
	}
	
	private async Task BuildOrderCommand(ulong guildId)
	{
		var guild = client.GetGuild(guildId);

		var command = new SlashCommandBuilder();
		command.WithName("order");
		command.WithDescription("Create an Order");
		command.AddDescriptionLocalization("de", "Erfasse eine Bestellung!");

		await guild.CreateApplicationCommandAsync(command.Build());
	}

	private static async Task ExecuteOrderCommand(SocketSlashCommand command)
	{
		var modal = CreateOrderModal();
		await command.RespondWithModalAsync(modal);
	}
}