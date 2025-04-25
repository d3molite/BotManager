using BotManager.Bot.Attributes;
using BotManager.Bot.Extensions;
using BotManager.Bot.Modules.Core;
using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.Feedback;
using BotManager.Resources;
using BotManager.Resources.Formatting;
using BotManager.Resources.Manager;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Feedback;

public class FeedbackModule(DiscordSocketClient client, GuildConfig config)
	: AbstractCommandModuleBase<FeedbackConfig>(client, config)
{
	private ulong ChannelId => ModuleConfig.FeedbackChannelId;

	private bool AddReaction => ModuleConfig.AddReactions;

	[CommandBuilder(Commands.Feedback)]
	public static async Task BuildFeedbackCommand(SocketGuild guild)
	{
		var command = new SlashCommandBuilder();
		command.WithName(Commands.Feedback);
		command.WithDescription(ResourceResolver.GetString(x => CommandResource.Feedback_Description, ""));

		command.AddDescriptionLocalization(
			"de",
			ResourceResolver.GetString(x => CommandResource.Feedback_Description, "de")
		);

		await guild.CreateApplicationCommandAsync(command.Build());
	}

	[CommandExecutor(Commands.Feedback)]
	public async Task ExecuteFeedbackCommand(SocketSlashCommand command)
	{
		var modal = CreateFeedbackModal();
		await command.RespondWithModalAsync(modal);
	}

	[ModalExecutor(Modals.FeedbackModal)]
	public async Task ProcessFeedbackModal(SocketModal modal)
	{
		var data = modal.Data.Components.ToList();

		await modal.RespondAsync(
			ResourceResolver.GetString(x => CommandResource.Feedback_Received, Locale),
			ephemeral: true
		);

		var message = data.Get(Modals.FeedbackModalText).Value;

		var guild = Client.GetGuild(modal.GuildId!.Value);
		var channel = guild.GetTextChannel(ChannelId);

		var sentMessage = await channel.SendMessageAsync(
			" ",
			embed: FeedbackEmbed(message, (SocketGuildUser)modal.User)
		);

		if (AddReaction)
			await sentMessage.AddReactionsAsync(
				new List<IEmote>
				{
					new Emoji("👍"),
					new Emoji("👎"),
				}
			);
	}

	private static Modal CreateFeedbackModal()
	{
		var builder = new ModalBuilder()
			.WithTitle("Feedback")
			.WithCustomId(Modals.FeedbackModal)
			.AddTextInput("Feedback Text", Modals.FeedbackModalText, style: TextInputStyle.Paragraph);

		return builder.Build();
	}

	private Embed FeedbackEmbed(string feedbackText, SocketGuildUser user)
	{
		var embedBuilder = new EmbedBuilder();

		var title = ResourceResolver.GetString(x => EmbedResource.FeedbackEmbedTitle, Locale);
		embedBuilder.Title = string.Format(title, user.GetEmbedTitle(), DateTime.Now.ToString("dd.MM.yyyy"));

		var splitText = Split(feedbackText, 900);

		var iter = 1;

		var enumerable = splitText as string[] ?? splitText.ToArray();

		foreach (var text in enumerable)
		{
			embedBuilder.AddField($"{iter} / {enumerable.Length}", text);
			iter++;
		}

		return embedBuilder.Build();
	}

	private static IEnumerable<string> Split(string str, int chunkSize)
	{
		for (var index = 0; index < str.Length; index += chunkSize)
			yield return str.Substring(index, Math.Min(chunkSize, str.Length - index));
	}
}