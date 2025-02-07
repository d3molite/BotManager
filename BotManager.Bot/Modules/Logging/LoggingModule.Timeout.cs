using BotManager.Bot.Extensions;
using BotManager.Bot.Modules.AntiSpam;
using BotManager.Resources;
using BotManager.Resources.Manager;
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
	private Embed UserTimedOutEmbed(IUser user)
	{
		var builder = GetLoggingEmbedBuilder(critical: true);

		builder.AddField(
			Resolver.GetString(_ => LoggingResource.Header_UserTimedOut, Locale),
			Resolver.GetString(_ => LoggingResource.Body_UserTimedOut, Locale).Insert(user)
		);

		return builder.Build();
	}
}