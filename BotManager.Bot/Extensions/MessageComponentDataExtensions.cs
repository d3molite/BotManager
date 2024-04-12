using Discord.WebSocket;

namespace BotManager.Bot.Extensions;

public static class MessageComponentDataExtensions
{
	public static SocketMessageComponentData Get(this IEnumerable<SocketMessageComponentData> items, string key)
	{
		return items.First(x => x.CustomId == key);
	}
}