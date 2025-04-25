using BotManager.Bot.Attributes;
using BotManager.Bot.Modules.Definitions;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.OrderTracking;

public partial class OrderTrackingModule
{
	[CommandBuilder(Commands.Order)]
	public static async Task BuildOrderCommand(SocketGuild guild)
	{
		var command = new SlashCommandBuilder();
		command.WithName(Commands.Order);
		command.WithDescription("Create an Order");
		command.AddDescriptionLocalization("de", "Erfasse eine Bestellung!");

		await guild.CreateApplicationCommandAsync(command.Build());
	}

	[CommandExecutor(Commands.Order)]
	public async Task ExecuteOrderCommand(SocketSlashCommand command)
	{
		var modal = CreateOrderModal();
		await command.RespondWithModalAsync(modal);
	}
}