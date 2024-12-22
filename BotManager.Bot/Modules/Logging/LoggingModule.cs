using BotManager.Bot.Extensions;
using BotManager.Bot.Interfaces.Modules;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.Logging;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;

namespace BotManager.Bot.Modules.Logging;

public partial class LoggingModule(DiscordSocketClient client, GuildConfig guildConfig) : IUtilityModule
{
	private LoggingConfig Config => guildConfig.LoggingConfig!;

	private static readonly Color CriticalColor = Color.Red;

	private static readonly Color WarningColor = Color.Orange;

	private static readonly Color InfoColor = Color.Blue;
	
	public Task RegisterModuleAsync()
	{
		client.MessageDeleted += LogMessageDeleted;
		client.UserJoined += LogUserJoined;
		client.UserLeft += LogUserLeft;
		client.UserBanned += LogUserBanned;
		client.MessageUpdated += LogMessageEdited;

		return Task.CompletedTask;
	}

	private async Task SendLogEmbed(Embed embed, bool isCritical = false)
	{
		var guild = client.GetGuild(guildConfig.GuildId);

		var loggingChannelId = Config.LoggingChannelId;
		
		if (isCritical)
		{
			if (Config.CriticalMessageChannelId.HasValue)
				loggingChannelId = Config.CriticalMessageChannelId.Value;
		}
		
		var loggingChannel = guild.GetTextChannel(loggingChannelId);

		await loggingChannel.SendMessageAsync(embed: embed);
	}

	private static EmbedBuilder GetLoggingEmbedBuilder(bool critical = false)
	{
		var builder = new EmbedBuilder();

		builder.WithTitle($"Log - {DateTime.UtcNow:HH:mm:ss}");

		if (critical)
			builder.WithColor(CriticalColor);
		
		return builder;
	}

	private bool IsCorrectGuild(SocketGuildUser user)
		=> user.Guild.Id == guildConfig.GuildId;
	
	private bool IsCorrectGuild(SocketGuild guild)
		=> guild.Id == guildConfig.GuildId;
	
	private bool IsCorrectGuild(IGuildChannel channel)
		=> channel.GuildId == guildConfig.GuildId;

	private async Task<IGuildChannel?> IsChannelInCorrectGuild(Cacheable<IMessageChannel, ulong> channel)
	{
		var channelObject = await channel.GetOrDownloadAsync();
		
		if (channelObject is IGuildChannel guildChannel && IsCorrectGuild(guildChannel))
			return guildChannel;

		return null;
	}

	private bool IsChannelInCorrectGuild(ISocketMessageChannel channel)
		=> channel is IGuildChannel guildChannel && IsCorrectGuild(guildChannel);
}