using BotManager.Bot.Extensions;
using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models.Modules.Voice;
using BotManager.Resources;
using BotManager.Resources.Manager;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.VoiceChannel;

public partial class VoiceChannelModule
{
	public async Task BuildCommands(VoiceChannelConfig config, ulong guildId)
	{
		var guild = _client.GetGuild(guildId);

		var command = new SlashCommandBuilder();
		command.WithName(Commands.Voice);
		command.WithDescription(Resolver.GetString(_ => CommandResource.Voice_Description, Locale));
		await guild.CreateApplicationCommandAsync(command.Build());
	}

	public async Task ExecuteCommands(SocketSlashCommand command)
	{
		switch (command.CommandName)
		{
			case Commands.Voice:
				await ExecuteVoiceCommand(command);
				break;
		}
	}

	private async Task ExecuteVoiceCommand(SocketSlashCommand command)
	{
		if (command.Channel.Id != Config.CommandChannelId)
		{
			await command.RespondAsync(
				Resolver.GetString(_ => CommandResource.Voice_Error_Channel, Locale).Insert(Config.CommandChannelId),
				ephemeral: true
			);

			return;
		}

		await command.RespondWithModalAsync(CreateVoiceModal());
	}
}