using Discord;

namespace BotManager.Bot.Extensions;

public static class UserExtensions
{
	public static string GetEmbedInfo(this IUser user)
		=> $"<@{user.Id}> (@{user.Username})";
}