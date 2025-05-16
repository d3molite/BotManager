using BotManager.Bot.Attributes;
using BotManager.Bot.Modules.Definitions;
using BotManager.Resources;
using BotManager.Resources.Formatting;
using BotManager.Resources.Manager;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.VoiceChannel;

public partial class VoiceChannelModule
{
	[CommandBuilder(Commands.Voice)]
	public async Task BuildVoiceCommand(SocketGuild guild)
	{
		var builder = new SlashCommandBuilder();
		builder.WithName(Commands.Voice);
		builder.WithDescription(ResourceResolver.GetString(_ => CommandResource.Voice_Description, Locale));
		await guild.CreateApplicationCommandAsync(builder.Build());
	}

	[CommandExecutor(Commands.Voice)]
	public async Task ExecuteVoiceCommand(SocketSlashCommand command)
	{
		if (command.Channel.Id != ModuleConfig.CommandChannelId)
		{
			await command.RespondAsync(
				ResourceResolver
					.GetString(_ => CommandResource.Voice_Error_Channel, Locale)
					.Insert(ModuleConfig.CommandChannelId),
				ephemeral: true
			);

			return;
		}

		await command.RespondWithModalAsync(CreateVoiceModal());
	}
}