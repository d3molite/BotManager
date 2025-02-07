using BotManager.Bot.Extensions;
using BotManager.Resources;
using BotManager.Resources.Manager;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Logging;

public partial class LoggingModule
{
	/// <summary>
	/// Task which is called when a user joins a guild.
	/// </summary>
	/// <param name="user">User which has joined a guild.</param>
	private async Task LogUserJoined(SocketGuildUser user)
	{
		if (!IsCorrectGuild(user))
			return;

		await SendLogEmbed(UserJoinedEmbed(user));
	}

	/// <summary>
	/// Task which is called when a user leaves a guild.
	/// </summary>
	/// <param name="guild">Guild which has been left.</param>
	/// <param name="user">The user which has left.</param>
	private async Task LogUserLeft(SocketGuild guild, SocketUser user)
	{
		if (!IsCorrectGuild(guild))
			return;

		await SendLogEmbed(UserLeftEmbed(user));
	}

	private Embed UserLeftEmbed(IUser user)
	{
		var builder = GetLoggingEmbedBuilder();

		builder.AddField(
			Resolver.GetString(_ => LoggingResource.Header_UserLeft, Locale),
			Resolver.GetString(_ => LoggingResource.Body_UserLeft, Locale).Insert(user)
		);

		return builder.Build();
	}

	private Embed UserJoinedEmbed(SocketGuildUser user)
	{
		var builder = GetLoggingEmbedBuilder();

		builder.AddField(
			Resolver.GetString(_ => LoggingResource.Header_UserJoined, Locale),
			Resolver.GetString(_ => LoggingResource.Body_UserJoined, Locale).Insert(user)
		);

		var ageString = (DateTime.Now - user.CreatedAt).ToHumanFriendlyString();

		builder.AddField(
			Resolver.GetString(_ => LoggingResource.Header_UserJoined_Age, Locale),
			ageString
		);

		return builder.Build();
	}
}