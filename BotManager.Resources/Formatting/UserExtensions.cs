using Discord;

namespace BotManager.Resources.Formatting;

public static class UserExtensions
{
	public static string GetEmbedInfo(this IUser user)
		=> $"<@{user.Id}> (@{user.Username})";

	public static string GetEmbedTitle(this IUser user)
	{
		if (user is IGuildUser guildUser)
			return $"{guildUser.Nickname} (@{user.Username})";

		return $"@{user.Username}";
	}
}