using BotManager.Bot.Interfaces.Services;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.Logging;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Logging;

public partial class LoggingModule(DiscordSocketClient client, GuildConfig guildConfig) : ICommandModule<LoggingConfig>
{
	public void RegisterLogger()
	{
		client.MessageDeleted += LogMessageDeleted;
		client.UserJoined += LogUserJoined;
		client.UserLeft += LogUserLeft;
	}

	private async Task LogMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
	{
		var channelObject = await channel.GetOrDownloadAsync();

		if (((IGuildChannel)channelObject).GuildId != guildConfig.GuildId)
			return;
		
		var messageObject = await message.GetOrDownloadAsync();

		if (messageObject is null)
		{
			await SendLogEmbed(MessageDeletedEmbed());
			return;
		}
		
		if (messageObject.Author.Id == client.CurrentUser.Id)
			return;
		
		var guildId = ((SocketGuildChannel)message.Value.Channel).Guild.Id;
		
		if (guildId != guildConfig.GuildId)
			return;

		await SendLogEmbed(MessageDeletedEmbed(messageObject));
	}
	
	private async Task LogUserJoined(SocketGuildUser user)
	{
		if (user.Guild.Id != guildConfig.GuildId)
			return;
		
		await SendLogEmbed(UserJoinedEmbed(user));
	}

	private async Task LogUserLeft(SocketGuild guild, SocketUser user)
	{
		if (guild.Id != guildConfig.GuildId)
			return;
		
		await SendLogEmbed(UserLeftEmbed(user));
	}

	private async Task SendLogEmbed(Embed embed)
	{
		var guild = client.GetGuild(guildConfig.GuildId);
		var loggingChannel = guild.GetTextChannel(guildConfig.LoggingConfig!.LoggingChannelId);
		
		await loggingChannel.SendMessageAsync(embed: embed);
	}

	private static Embed MessageDeletedEmbed(IMessage? message = null)
	{
		var builder = GetLoggingEmbedBuilder();

		if (message != null)
		{
			builder.AddField("Message deleted", $"Message by user {GetUserTextInfo(message.Author)} has been deleted from the server.");
			builder.AddField("Content:", message.Content);
		}
		else
			builder.AddField("Message deleted", "A message was deleted but the content could not be retrieved from cache.");
		
		return builder.Build();
	}

	private static Embed UserLeftEmbed(IUser user)
	{
		var builder = GetLoggingEmbedBuilder();
		builder.AddField("User left", $"User {GetUserTextInfo(user)} has left the server.");
		return builder.Build();
	}

	private static Embed UserJoinedEmbed(SocketGuildUser user)
	{
		var builder = GetLoggingEmbedBuilder();
		
		builder.AddField("User joined", $"User {GetUserTextInfo(user)} joined the server.");
		
		var ageString = OffsetToDate(DateTime.Now - user.CreatedAt);
		builder.AddField("Account age:", ageString);
		
		return builder.Build();
	}

	private static string GetUserTextInfo(IUser user)
	{
		return $"<@{user.Id}> (@{user.Username})";
	}

	private static EmbedBuilder GetLoggingEmbedBuilder()
	{
		var builder = new EmbedBuilder();
		
		builder.WithTitle($"Log - {DateTime.UtcNow:HH:mm:ss}");

		return builder;
	}
	
	private static string OffsetToDate(TimeSpan time)
	{
		var age = Math.Round(time.TotalDays, 2);

		return age switch
		{
			// if the time is smaller than one day.
			< 1 => Math.Round(time.TotalHours, 2) + " hours",

			// if the time is smaller than a year.
			< 365 => age + " days",

			// if the time is greater than a year.
			_ => Math.Round(time.TotalDays / 365, 2) + " years",
		};
	}
}