using BotManager.Bot.Interfaces.Modules;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.Reactions;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Reactions;

public class ReactionModule(DiscordSocketClient client, GuildConfig guildConfig) : IUtilityModule
{
	private readonly Random _random = new();

	private List<ReactionItem> ReactionItems => guildConfig.ReactionConfig!.Reactions;

	public async Task RegisterModuleAsync()
	{
		client.MessageReceived += ReactToMessage;
	}

	private async Task ReactToMessage(SocketMessage message)
	{
		if (!CanReactToMessage(message))
			return;

		var items = GetMatchingReactions(message.Content);

		if (items.Length == 0)
			return;

		var roll = _random.Next(0, 100);

		var matchingItem = GetLowestRollMatch(items, roll);

		switch (matchingItem)
		{
			case null:
				return;

			case { EmojiOnly: true }:
				await AddEmoteToMessage(message, matchingItem.ReactionEmoji!);
				break;
			
			case { EmojiOnly: false }:
				await ReplyToMessage(message, matchingItem.ReactionMessage!);
				break;
		}
	}

	private static async Task ReplyToMessage(SocketMessage message, string reactionMessage)
	{
		var reference = new MessageReference(message.Id, message.Channel.Id, ((SocketGuildChannel)message.Channel).Guild.Id);
		await message.Channel.SendMessageAsync(reactionMessage, messageReference: reference);
	}

	private static async Task AddEmoteToMessage(SocketMessage message, string emoji)
	{
		if (emoji.StartsWith('<') && emoji.EndsWith('>'))
			await message.AddReactionAsync(Emote.Parse(emoji));
		else
			await message.AddReactionAsync(Emoji.Parse(emoji));
	}

	private bool CanReactToMessage(SocketMessage message)
		=> message.Author.Id != client.CurrentUser.Id && message.Channel is SocketGuildChannel;

	private ReactionItem[] GetMatchingReactions(string messageContent)
		=> ReactionItems
			.Where(x => messageContent.Contains(x.ReactionTrigger, StringComparison.CurrentCultureIgnoreCase))
			.ToArray();

	private static ReactionItem? GetLowestRollMatch(ReactionItem[] items, int roll)
		=> items.Where(x => roll <= x.ReactionChance).MinBy(x => x.ReactionChance);
}