using BotManager.Bot.Extensions;
using BotManager.Resources;
using BotManager.Resources.Formatting;
using BotManager.Resources.Manager;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;

namespace BotManager.Bot.Modules.Logging;

public partial class LoggingModule
{
	/// <summary>
	/// Task that is called when a user is banned from a guild the bot is in.
	/// </summary>
	/// <param name="user">User which was banned.</param>
	/// <param name="guild">Guild the user was banned from.</param>
	private async Task LogUserBanned(SocketUser user, SocketGuild guild)
	{
		if (guild.Id != config.GuildId)
			return;

		// wait for discord to send the user ban to the audit log
		Thread.Sleep(500);

		var reason = await TryFetchAuditLogBan(guild, user);

		await SendLogEmbed(UserBannedEmbed(user, reason), true);
	}

	/// <summary>
	/// Will try to fetch the last ban entry from the audit log.
	/// </summary>
	/// <param name="guild">Guild to query.</param>
	/// <param name="user"></param>
	/// <returns>The audit log entry, or null if the call has failed due to permissions or other reasons.</returns>
	private static async Task<string?> TryFetchAuditLogBan(SocketGuild guild, SocketUser user)
	{
		try
		{
			var banReason = await guild.GetBanAsync(user);
			return banReason.Reason;
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Failed to fetch audit log");
			return null;
		}
	}

	/// <summary>
	/// Generate an embed that logs when a user was banned.
	/// </summary>
	/// <param name="user">The banned user.</param>
	/// <param name="reason">A <see cref="RestAuditLogEntry"/> related to the ban. Can be null.</param>
	/// <returns>A formatted embed.</returns>
	private Embed UserBannedEmbed(IUser user, string? reason = null)
	{
		var builder = GetLoggingEmbedBuilder(critical: true);

		builder.AddField(
			ResourceResolver.GetString(_ => LoggingResource.Header_UserBanned, Locale),
			ResourceResolver.GetString(_ => LoggingResource.Body_UserBanned, Locale).Insert(user)
		);

		builder.AddField(
			ResourceResolver.GetString(_ => LoggingResource.Header_UserBanned_Reason, Locale),
			reason ?? 
			ResourceResolver.GetString(_ => LoggingResource.Body_UserBanned_ReasonNotFound, Locale)
		);

		return builder.Build();
	}
}