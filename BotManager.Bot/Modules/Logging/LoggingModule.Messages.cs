using System.Text;
using BotManager.Bot.Extensions;
using BotManager.Db.Models;
using BotManager.Resources;
using BotManager.Resources.Formatting;
using BotManager.Resources.Manager;
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
		if (editedMessage.Author.Id == client.CurrentUser.Id)
			return;

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
		var guildChannel = await IsChannelInCorrectGuild(channel);

		if (guildChannel is null)
			return;

		var messageObject = await message.GetOrDownloadAsync();

		if (messageObject is null)
		{
			await SendLogEmbed(MessageDeletedEmbed(guildChannel), true);
			return;
		}

		if (messageObject.Author.Id == client.CurrentUser.Id)
			return;

		if (!string.IsNullOrEmpty(messageObject.Content))
		{
			await SendLogEmbed(MessageDeletedEmbed(guildChannel, messageObject), true);
			return;
		}
	}

	private Embed MessageDeletedEmbed(IGuildChannel guildChannel, IMessage? message = null)
	{
		var builder = GetLoggingEmbedBuilder();

		var header = ResourceResolver.GetString(_ => LoggingResource.Header_MessageDeleted, Locale);

		if (message != null)
		{
			var messageString = ResourceResolver.GetString(_ => LoggingResource.Body_MessageDeleted, Locale)
				.Insert(message, guildChannel);

			builder.AddField(header, messageString);

			if (!string.IsNullOrEmpty(message.Content))
				builder.AddField(
					ResourceResolver.GetString(_ => LoggingResource.Header_Content, Locale),
					message.Content
				);

			else if (message.Attachments.Any())
				builder.AddField(
					ResourceResolver.GetString(_ => LoggingResource.Header_Content, Locale),
					string.Join(", ", message.Attachments.Select(x => x.Filename))
				);

			else
				builder.AddField(
					ResourceResolver.GetString(_ => LoggingResource.Header_Content, Locale),
					ResourceResolver.GetString(_ => LoggingResource.Body_MessageDeleted_Errors, Locale)
				);
			
		}
		else
			builder.AddField(
				header,
				ResourceResolver.GetString(_ => LoggingResource.Body_MessageDeleted_NotFound, Locale)
					.Insert(guildChannel)
			);

		builder.WithColor(WarningColor);

		return builder.Build();
	}

	private Embed MessageEditedEmbed(IMessage editedMessage, IMessage? originalMessage = null)
	{
		var builder = GetLoggingEmbedBuilder();
		builder.WithColor(InfoColor);

		var headerOriginalMessage = ResourceResolver.GetString(
			_ => LoggingResource.Header_MessageEdited_OriginalMessage,
			Locale
		);

		var headerNewMessage = ResourceResolver.GetString(_ => LoggingResource.Header_MessageEdited_NewMessage, Locale);

		builder.AddField(
			ResourceResolver.GetString(_ => LoggingResource.Header_MessageEdited, Locale),
			ResourceResolver.GetString(_ => LoggingResource.Body_MessageEdited, Locale)
				.Insert(editedMessage)
		);

		if (originalMessage != null)
		{
			builder.AddField(headerOriginalMessage, originalMessage.Content);
			builder.AddField(headerNewMessage, GetChangesForNewMessage(originalMessage.Content, editedMessage.Content));
		}
		else
		{
			builder.AddField(
				headerOriginalMessage,
				ResourceResolver.GetString(_ => LoggingResource.Body_MessageEdited_NotFound, Locale)
			);

			builder.AddField(
				headerNewMessage,
				editedMessage.Content ?? ResourceResolver.GetString(
					_ => LoggingResource.Body_MessageEdited_NotFound,
					Locale
				)
			);
		}

		builder.AddField(
			ResourceResolver.GetString(_ => LoggingResource.Header_Actions, Locale),
			$"[{ResourceResolver.GetString(_ => LoggingResource.Body_Actions_LinkToMessage, Locale)}]({editedMessage.ToMessageLink()})"
		);

		return builder.Build();
	}

	private static string GetChangesForNewMessage(string original, string modified)
	{
		var originalWords = original.Split(' ')
			.ToList();

		var modifiedWords = modified.Split(' ')
			.ToList();

		var newWords = new List<string>();

		foreach (var item in modifiedWords.Select((x, i) => new
					{
						Word = x,
						Index = i
					}
				))
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