using BotManager.Bot.Interfaces.Modules;
using BotManager.Bot.Modules.Core;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.Logging;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Logging;

public partial class LoggingModule(DiscordSocketClient client, GuildConfig config) : AbstractModuleBase<LoggingConfig>(config), IUtilityModule
{
	private static readonly Color CriticalColor = Color.Red;

	private static readonly Color WarningColor = Color.Orange;

	private static readonly Color InfoColor = Color.Blue;

	private bool _registered;

	public Task RegisterModuleAsync()
	{
		if (_registered)
			return Task.CompletedTask;
		
		client.MessageDeleted += LogMessageDeleted;
		client.UserJoined += LogUserJoined;
		client.UserLeft += LogUserLeft;
		client.UserBanned += LogUserBanned;
		client.MessageUpdated += LogMessageEdited;

		_registered = true;

		return Task.CompletedTask;
	}

	private async Task SendLogEmbed(Embed embed, bool isCritical = false)
	{
		var guild = client.GetGuild(GuildConfig.GuildId);

		var loggingChannelId = ModuleConfig.LoggingChannelId;

		if (isCritical && ModuleConfig.CriticalMessageChannelId.HasValue)
		{
			loggingChannelId = ModuleConfig.CriticalMessageChannelId.Value;
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
		=> user.Guild.Id == GuildConfig.GuildId;

	private bool IsCorrectGuild(SocketGuild guild)
		=> guild.Id == GuildConfig.GuildId;

	private bool IsCorrectGuild(IGuildChannel channel)
		=> channel.GuildId == GuildConfig.GuildId;

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