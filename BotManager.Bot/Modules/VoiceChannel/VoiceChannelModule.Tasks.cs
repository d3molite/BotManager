using Discord.WebSocket;
using Serilog;

namespace BotManager.Bot.Modules.VoiceChannel;

public partial class VoiceChannelModule
{
	private void PollChannels()
	{
		var category = Client.GetGuild(GuildConfig.GuildId).GetCategoryChannel(ModuleConfig.VoiceCategoryId);

		foreach (var channel in category.Channels.Where(x => x.Name.StartsWith('[')))
		{
			if (channel is SocketVoiceChannel voiceChannel)
				CurrentChannels.Add(
					new VoiceState
					{
						Channel = voiceChannel,
						ChannelId = voiceChannel.Id,
						ChannelName = voiceChannel.Name,
						ChannelNumber = ParseChannelNumber(voiceChannel.Name),
					}
				);
		}
		
		Log.Debug("Found {Count} Channels for {GuildName}:", CurrentChannels.Count, GuildConfig.GuildName);

		foreach (var channel in CurrentChannels)
		{
			Log.Debug("{ChannelName}",channel.ChannelName);
		}

		Task.Run(async () => await RunPeriodicChecks());
	}

	private async Task RunPeriodicChecks()
	{
		while (await _timer.WaitForNextTickAsync(CancellationToken.None))
		{
			var removed = new List<VoiceState>();
			
			foreach (var voiceState in CurrentChannels)
			{
				await ReloadChannel(voiceState);
				
				if (IsToBeDeleted(voiceState))
				{
					try
					{
						await voiceState.Channel!.DeleteAsync();
						removed.Add(voiceState);
						TryGetLogger();
						await _loggingModule!.LogVoiceChannelDeleted(ModuleConfig.CommandChannelId, voiceState);
					}
					catch (Exception ex)
					{
						Log.Error(ex, "Failed to remove voice channel {ChannelName}", voiceState.Channel!.Name);
					}
					continue;
				}

				voiceState.UsersPresent = HasUsersConnected(voiceState);
			}

			foreach (var state in removed) 
				CurrentChannels.Remove(state);
		}
	}
	
	private static bool IsToBeDeleted(VoiceState voiceState)
	{
		if (voiceState.Channel is null)
			return false;
		
		var last = voiceState.UsersPresent;
		var current = HasUsersConnected(voiceState);

		return !last && !current;
	}

	private static bool HasUsersConnected(VoiceState voiceState)
	{
		if (voiceState.Channel is null)
			return true;
		
		return voiceState.Channel.ConnectedUsers.Count > 0;
	}

	private async Task ReloadChannel(VoiceState state)
	{
		var channel = await Client.GetChannelAsync(state.ChannelId);

		if (channel is SocketVoiceChannel voiceChannel)
		{
			state.Channel = voiceChannel;
			state.ChannelId = voiceChannel.Id;
			state.ChannelName = voiceChannel.Name;
			state.ChannelNumber = ParseChannelNumber(voiceChannel.Name);
		}
	}

	private static int ParseChannelNumber(string channelName)
		=> int.Parse(channelName.Split("]")[0][1..]);
}