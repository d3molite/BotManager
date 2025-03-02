using BotManager.Bot.Extensions;
using BotManager.Bot.Modules.Definitions;
using BotManager.Resources;
using BotManager.Resources.Manager;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.VoiceChannel;

public partial class VoiceChannelModule
{
	public async Task ExecuteModal(SocketModal modal)
	{
		switch (modal.Data.CustomId)
		{
			case ModalFields.VoiceModalId:
				await ProcessVoiceModal(modal);
				break;
		}
	}

	private async Task ProcessVoiceModal(SocketModal modal)
	{
		var data = modal.Data.Components.ToList();

		var nextChannel = 1;

		if (CurrentChannels.Count != 0) 
			nextChannel += CurrentChannels.Select(x => x.ChannelNumber).Max();
		
		var channelName = $"[{nextChannel}] {data.Get(ModalFields.VoiceModalName).Value}";

		var guild = _client.GetGuild(_guildConfig.GuildId);

		RestVoiceChannel channel;

		try
		{
			channel = await guild.CreateVoiceChannelAsync(
				channelName,
				properties =>
				{
					properties.CategoryId = Config.VoiceCategoryId;
					properties.Bitrate = 384000;
				}
			);
		}
		catch
		{
			channel = await guild.CreateVoiceChannelAsync(
				channelName,
				properties => { properties.CategoryId = Config.VoiceCategoryId; }
			);
		}
		
		var state = new VoiceState()
		{
			ChannelId = channel.Id,
			ChannelName = channelName,
			ChannelNumber = nextChannel,
		};

		CurrentChannels.Add(state);
		
		TryGetLogger();
		await _loggingModule!.LogVoiceChannelCreated(modal, state);
	}

	private Modal CreateVoiceModal()
	{
		var builder = new ModalBuilder()
					.WithCustomId(ModalFields.VoiceModalId)
					.WithTitle(Resolver.GetString(_ => CommandResource.Voice_Modal_Title, Locale))
					.AddTextInput(
						Resolver.GetString(_ => CommandResource.Voice_Modal_ChannelName, Locale),
						ModalFields.VoiceModalName,
						minLength: 3,
						maxLength: 14
					);

		return builder.Build();
	}
}