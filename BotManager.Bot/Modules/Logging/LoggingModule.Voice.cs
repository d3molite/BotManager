using BotManager.Bot.Extensions;
using BotManager.Bot.Modules.VoiceChannel;
using BotManager.Resources;
using BotManager.Resources.Formatting;
using BotManager.Resources.Manager;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Logging;

public partial class LoggingModule
{
	public async Task LogVoiceChannelCreated(SocketModal modal, VoiceState voiceState)
		=> await modal.RespondAsync("", embed: VoiceChannelCreatedEmbed(voiceState));

	public async Task LogVoiceChannelDeleted(ulong channelId, VoiceState voiceState)
	{
		var channel = await client.GetChannelAsync(channelId);

		if (channel is ITextChannel textChannel)
			await textChannel.SendMessageAsync("", embed: VoiceChannelDeletedEmbed(voiceState.Channel!));
	}

	private Embed VoiceChannelCreatedEmbed(VoiceState voiceState)
	{
		var builder = new EmbedBuilder();

		builder.AddField(
			ResourceResolver.GetString(_ => LoggingResource.Header_VoiceCreated, Locale),
			ResourceResolver.GetString(_ => LoggingResource.Body_VoiceCreated, Locale).Insert(voiceState.ChannelName)
		);

		builder.AddField(
			ResourceResolver.GetString(_ => LoggingResource.Header_Actions, Locale),
			ResourceResolver.GetString(_ => LoggingResource.Body_Actions_LinkToVoiceChannel, Locale).Insert(voiceState.ChannelId)
		);

		return builder.Build();
	}

	private Embed VoiceChannelDeletedEmbed(IVoiceChannel channel)
	{
		var builder = new EmbedBuilder();

		var timeOpen = (DateTime.Now - channel.CreatedAt);

		builder.AddField(
			ResourceResolver.GetString(_ => LoggingResource.Header_VoiceDeleted, Locale),
			ResourceResolver.GetString(_ => LoggingResource.Body_VoiceDeleted, Locale).Insert(channel.Name)
		);

		builder.AddField(
			ResourceResolver.GetString(_ => LoggingResource.Header_VoiceDeleted_TimeOpen, Locale),
			timeOpen.ToHumanFriendlyString()
		);

		return builder.Build();
	}
}