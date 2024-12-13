using BotManager.Bot.Extensions;
using BotManager.Bot.Modules.AntiSpam;
using Discord;

namespace BotManager.Bot.Modules.Logging;

public partial class LoggingModule
{
	/// <summary>
	/// Task that is called when a user is timed out by the <see cref="AntiSpamModule"/>.
	/// </summary>
	/// <param name="user">User which was timed out.</param>
	public async Task LogUserTimedOut(IUser user)
	{
		await SendLogEmbed(UserTimedOutEmbed(user), true);
	}

	/// <summary>
	/// Generate an embed that logs when a user was timed out.
	/// </summary>
	/// <param name="user">The user which was timed out.</param>
	/// <returns>A formatted embed.</returns>
	private static Embed UserTimedOutEmbed(IUser user)
	{
		var builder = GetLoggingEmbedBuilder(critical: true);
		builder.AddField("User timed out.", $"User {user.GetEmbedInfo()} has been timed out for spamming.");
		return builder.Build();
	}
}