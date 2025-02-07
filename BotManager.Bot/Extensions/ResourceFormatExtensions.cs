using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Extensions;

public static class ResourceFormatExtensions
{
	public static string Insert(this string modifiable, IMessage message, IChannel channel)
		=> string.Format(modifiable, message.Author.GetEmbedInfo(), channel.Id);

	public static string Insert(this string modifiable, IChannel channel)
		=> string.Format(modifiable, channel.Id);

	public static string Insert(this string modifiable, IMessage message)
		=> string.Format(modifiable, message.Author.GetEmbedInfo());

	public static string Insert(this string modifiable, IUser user)
		=> string.Format(modifiable, user.GetEmbedInfo());

	public static string ToMessageLink(this IMessage message)
		=> $"https://discord.com/channels/{((SocketGuildChannel)message.Channel).Guild.Id}/{message.Channel.Id}/{message.Id}";
}