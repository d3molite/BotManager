using BotManager.Bot.Attributes;
using BotManager.Bot.Modules.Definitions;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.LanPlanner;

public partial class LanPlannerModule
{
	[CommandBuilder(Commands.Plan)]
	public static async Task BuildPlanCommand(SocketGuild guild)
	{
		var command = new SlashCommandBuilder();
		command.WithName(Commands.Plan);
		command.WithDescription("Plan a Lan");
		command.AddDescriptionLocalization("de", "Starte einen Plan");

		await guild.CreateApplicationCommandAsync(command.Build());
	}

	[CommandExecutor(Commands.Plan)]
	public async Task ExecutePlanCommand(SocketSlashCommand command)
	{
		var modal = CreatePlanStartModal();
		await command.RespondWithModalAsync(modal);
	}
}