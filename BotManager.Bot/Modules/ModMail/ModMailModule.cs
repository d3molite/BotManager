using System.Text;
using BotManager.Bot.Interfaces.Modules;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.ModMail;
using BotManager.Resources;
using BotManager.Resources.Manager;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.ModMail;

public class ModMailModule(DiscordSocketClient client, BotConfig config) : IUtilityModule
{
	private IEnumerable<ModMailConfig> ModMailConfigs => config.ModMailConfigs;

	public string ModuleName => "ModMail";

	public Task RegisterModuleAsync()
	{
		client.MessageReceived += ForwardDm;
		client.MessageReceived += ForwardForum;
		return Task.CompletedTask;
	}

	private async Task ForwardDm(SocketMessage message)
		=> await Task.Run(async () => await ForwardDmInternal(message));

	private async Task ForwardForum(SocketMessage message)
		=> await Task.Run(async () => await ForwardForumInternal(message));

	private async Task ForwardForumInternal(SocketMessage message)
	{
		if (message.Author.Id == client.CurrentUser.Id || message.Channel is not SocketThreadChannel thread)
			return;

		if (thread.IsLocked)
			return;

		var id = thread.Name.Split(":")[^1]
			.Replace("[", "")
			.Replace("]", "")
			.Trim();

		if (!ulong.TryParse(id, out var userId))
		{
			await thread.SendMessageAsync($"Could not parse {id} to a valid user Id");
			return;
		}

		var user = thread.Guild.GetUser(userId);

		await user.SendMessageAsync(text: FormatDmMessage(message));

		await message.AddReactionAsync(new Emoji("✅"));
	}

	private async Task ForwardDmInternal(SocketMessage message)
	{
		if (message.Author.Id == client.CurrentUser.Id || message.Channel is not SocketDMChannel dmChannel)
			return;

		var matchingForum = GetMatchingForumForUser(message.Author);

		if (matchingForum is null)
		{
			await dmChannel.SendMessageAsync("To use modmail, you must be on a server which has this feature enabled.");
			return;
		}

		var matchingThread = await GetMatchingThreadForUser(matchingForum, message);

		await matchingThread.SendMessageAsync(text: FormatThreadMessage(message));

		await message.AddReactionAsync(new Emoji("✅"));
	}

	private SocketForumChannel? GetMatchingForumForUser(SocketUser user)
	{
		foreach (var mailConfig in ModMailConfigs)
		{
			var guild = client.GetGuild(mailConfig.GuildId);

			var guildUser = guild.GetUser(user.Id);

			if (guildUser is null)
				continue;

			var channel = guild.GetForumChannel(mailConfig.ForumChannelId);

			return channel;
		}

		return null;
	}

	private async Task<IThreadChannel> GetMatchingThreadForUser(SocketForumChannel forumChannel, SocketMessage message)
	{
		var threads = await forumChannel.GetActiveThreadsAsync();

		var existingThread = threads.Where(x => !x.IsLocked)
			.FirstOrDefault(x => x.Name.Contains($"{message.Author.Id}"));

		if (existingThread is null)
			return await CreatePostAsync(forumChannel, message);

		return forumChannel.Guild.GetThreadChannel(existingThread.Id);
	}

	private async Task<IThreadChannel> CreatePostAsync(SocketForumChannel forumChannel, SocketMessage message)
	{
		return await forumChannel.CreatePostAsync(
			title: FormatThreadTitle(message),
			text: FormatNewPost(message, forumChannel.Guild.Id),
			archiveDuration: ThreadArchiveDuration.OneDay,
			allowedMentions: AllowedMentions.All
		);
	}

	private string FormatNewPost(SocketMessage message, ulong guildId)
	{
		var locale = config.GuildConfigs.First(x => x.GuildId == guildId)
			.GuildLocale;
		
		var sb = new StringBuilder();

		var mailConfig = ModMailConfigs.First(x => x.GuildId == guildId);

		sb.AppendLine($"<@&{mailConfig.PingRoleId}>");

		var newMessage = ResourceResolver.GetString(_ => ModuleResources.ModMailNew, locale);
		sb.AppendLine($"{newMessage} <@{message.Author.Id}>");

		return sb.ToString();
	}

	private static string FormatDmMessage(SocketMessage message)
	{
		var sb = new StringBuilder();

		sb.AppendLine($"[{message.Author.Username}]:");
		sb.AppendLine(message.Content);

		if (message.Attachments.Count == 0)
			return sb.ToString();

		foreach (var attachment in message.Attachments)
			sb.AppendLine(attachment.Url);

		return sb.ToString();
	}

	private static string FormatThreadMessage(SocketMessage message)
	{
		var sb = new StringBuilder();

		sb.AppendLine($"[<@{message.Author.Id}>]:");
		sb.AppendLine(message.Content);

		if (message.Attachments.Count == 0)
			return sb.ToString();

		foreach (var attachment in message.Attachments)
			sb.AppendLine(attachment.Url);

		return sb.ToString();
	}

	private static string FormatThreadTitle(SocketMessage message)
		=> $"{message.Author.GlobalName}: [{message.Author.Id}]";
}