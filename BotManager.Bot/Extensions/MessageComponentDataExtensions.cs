using Discord.WebSocket;

namespace BotManager.Bot.Extensions;

public static class MessageComponentDataExtensions
{
	public static SocketMessageComponentData Get(this IEnumerable<SocketMessageComponentData> items, string key)
	{
		return items.First(x => x.CustomId == key);
	}
	
	public static string GetString(this IEnumerable<SocketMessageComponentData> items, string key)
	{
		var item = items.FirstOrDefault(x => x.CustomId == key);
		return item != null ? item.Value : string.Empty;
	}
}