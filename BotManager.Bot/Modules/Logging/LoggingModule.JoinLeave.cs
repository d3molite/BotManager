using BotManager.Bot.Extensions;
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
}