using BotManager.Bot.Extensions;
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
		if (guild.Id != guildConfig.GuildId)
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
	private static Embed UserBannedEmbed(IUser user, string? reason = null)
	{
		var builder = GetLoggingEmbedBuilder(critical: true);
		
		builder.AddField("User banned.", $"User {user.GetEmbedInfo()} has been banned from the server.");

		builder.AddField("Ban reason:", reason ?? "Could not fetch ban reason.");

		return builder.Build();
	}
	
}