using Discord;
using Discord.WebSocket;
using Serilog;

namespace BotManager.Bot.Extensions;

public static class CommandRegisterExtension
{
	public static async Task TryCreateGuildCommand(this SocketGuild guild, SlashCommandBuilder builder)
	{
		try
		{
			await guild.CreateApplicationCommandAsync(builder.Build());
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Failed to build {CommandName} command", builder.Name);
		}
	}
}