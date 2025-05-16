using BotManager.Bot.Attributes;
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

	[ModalExecutor(Modals.VoiceModalId)]
	public async Task ProcessVoiceModal(SocketModal modal)
	{
		var data = modal.Data.Components.ToList();

		var nextChannel = 1;

		if (CurrentChannels.Count != 0) 
			nextChannel += CurrentChannels.Select(x => x.ChannelNumber).Max();
		
		var channelName = $"[{nextChannel}] {data.Get(Modals.VoiceModalName).Value}";

		var guild = Client.GetGuild(GuildConfig.GuildId);

		RestVoiceChannel channel;

		try
		{
			channel = await guild.CreateVoiceChannelAsync(
				channelName,
				properties =>
				{
					properties.CategoryId = ModuleConfig.VoiceCategoryId;
					properties.Bitrate = 384000;
				}
			);
		}
		catch
		{
			channel = await guild.CreateVoiceChannelAsync(
				channelName,
				properties => { properties.CategoryId = ModuleConfig.VoiceCategoryId; }
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
					.WithCustomId(Modals.VoiceModalId)
					.WithTitle(ResourceResolver.GetString(_ => CommandResource.Voice_Modal_Title, Locale))
					.AddTextInput(
						ResourceResolver.GetString(_ => CommandResource.Voice_Modal_ChannelName, Locale),
						Modals.VoiceModalName,
						minLength: 3,
						maxLength: 14
					);

		return builder.Build();
	}
}