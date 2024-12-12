using BotManager.Bot.Extensions;
using BotManager.Bot.Interfaces.Modules;
using BotManager.Db.Models;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Logging;

public class LoggingModule(DiscordSocketClient client, GuildConfig guildConfig) : IUtilityModule
{
	public async Task RegisterModuleAsync()
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
			builder.AddField(
				"Message deleted",
				$"Message by user {message.Author.GetEmbedInfo()} has been deleted from the server."
			);

			builder.AddField("Content:", message.Content);
		}
		else
			builder.AddField(
				"Message deleted",
				"A message was deleted but the content could not be retrieved from cache."
			);

		return builder.Build();
	}

	private static Embed UserLeftEmbed(IUser user)
	{
		var builder = GetLoggingEmbedBuilder();
		builder.AddField("User left", $"User {user.GetEmbedInfo()} has left the server.");
		return builder.Build();
	}

	private static Embed UserJoinedEmbed(SocketGuildUser user)
	{
		var builder = GetLoggingEmbedBuilder();

		builder.AddField("User joined", $"User {user.GetEmbedInfo()} joined the server.");

		var ageString = (DateTime.Now - user.CreatedAt).ToHumanFriendlyString();
		builder.AddField("Account age:", ageString);

		return builder.Build();
	}

	private static EmbedBuilder GetLoggingEmbedBuilder()
	{
		var builder = new EmbedBuilder();

		builder.WithTitle($"Log - {DateTime.UtcNow:HH:mm:ss}");

		return builder;
	}
}