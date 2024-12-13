using System.Text;
using BotManager.Bot.Extensions;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Logging;

public partial class LoggingModule
{
	private async Task LogMessageEdited(
		Cacheable<IMessage, ulong> originalMessage,
		SocketMessage editedMessage,
		ISocketMessageChannel channel
	)
	{
		if (!IsChannelInCorrectGuild(channel))
			return;

		var originalMessageObject = originalMessage.Value;

		if (originalMessageObject is null)
		{
			await SendLogEmbed(MessageEditedEmbed(editedMessage), true);
			return;
		}

		if (originalMessageObject.Content == editedMessage.Content)
			return;
		
		await SendLogEmbed(MessageEditedEmbed(editedMessage, originalMessageObject), true);
	}

	private async Task LogMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
	{
		if (!await IsChannelInCorrectGuild(channel))
			return;

		var messageObject = await message.GetOrDownloadAsync();

		if (messageObject is null)
		{
			await SendLogEmbed(MessageDeletedEmbed(), true);
			return;
		}

		if (messageObject.Author.Id == client.CurrentUser.Id)
			return;

		await SendLogEmbed(MessageDeletedEmbed(messageObject), true);
	}

	private static Embed MessageDeletedEmbed(IMessage? message = null)
	{
		var builder = GetLoggingEmbedBuilder();

		if (message != null)
		{
			builder.AddField(
				"Message deleted",
				$"Message by user {message.Author.GetEmbedInfo()} has been deleted from the server."
			);

			builder.AddField("Content:", message.Content);
		}
		else
			builder.AddField(
				"Message deleted",
				"A message was deleted but the content could not be retrieved from cache."
			);

		builder.WithColor(WarningColor);

		return builder.Build();
	}

	private static Embed MessageEditedEmbed(IMessage editedMessage, IMessage? originalMessage = null)
	{
		var builder = GetLoggingEmbedBuilder();
		builder.WithColor(InfoColor);

		builder.AddField(
			"Message edited.",
			$"A message by user {editedMessage.Author.GetEmbedInfo()} has been edited."
		);

		if (originalMessage != null)
		{
			builder.AddField("Original message:", originalMessage.Content);
			builder.AddField("New message:", GetChangesForNewMessage(originalMessage.Content, editedMessage.Content));
		}
		else
		{
			builder.AddField("Original message:", "The content could not be retrieved from the cache.");
			builder.AddField("New message:", editedMessage.Content);
		}

		builder.AddField(
			"Actions:",
			$"[Link to Message](https://discord.com/channels/{((SocketGuildChannel)editedMessage.Channel).Guild.Id}/{editedMessage.Channel.Id}/{editedMessage.Id})"
		);
		
		return builder.Build();
	}

	private static string GetChangesForNewMessage(string original, string modified)
	{
		var originalWords = original.Split(' ').ToList();
		var modifiedWords = modified.Split(' ').ToList();

		var newWords = new List<string>();

		foreach (var item in modifiedWords.Select((x, i) => new { Word = x, Index = i }))
		{
			var word = item.Word;
			var index = item.Index;

			if (originalWords.Contains(word))
			{
				newWords.Add(originalWords.IndexOf(word) == index ? word : $"**{word}**");
			}
			else
			{
				newWords.Add($"**{word}**");
			}
		}

		var newString = string.Join(" ", newWords);
		return newString;
	}
}