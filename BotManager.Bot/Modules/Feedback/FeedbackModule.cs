using BotManager.Bot.Extensions;
using BotManager.Bot.Interfaces.Modules;
using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.Feedback;
using BotManager.Resources;
using BotManager.Resources.Manager;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Feedback;

public class FeedbackModule(DiscordSocketClient client, GuildConfig config) : ICommandModule<FeedbackConfig>
{
	private ulong _channelId;
	private bool _addReaction;

	private string _locale =>  config.GuildLocale;
	public async Task BuildCommands(FeedbackConfig config, ulong guildId)
	{
		_channelId = config.FeedbackChannelId;
		_addReaction = config.AddReactions;
		
		var guild = client.GetGuild(guildId);

		await BuildFeedbackCommand(guild);
	}

	private static async Task BuildFeedbackCommand(SocketGuild guild)
	{
		var command = new SlashCommandBuilder();
		command.WithName(Commands.Feedback);
		command.WithDescription(Resolver.GetString(x => CommandResource.Feedback_Description, ""));
		command.AddDescriptionLocalization("de", Resolver.GetString(x => CommandResource.Feedback_Description, "de"));

		await guild.CreateApplicationCommandAsync(command.Build());
	}

	public async Task ExecuteCommands(SocketSlashCommand command)
	{
		switch (command.CommandName)
		{
			case Commands.Feedback:
				await ExecuteFeedbackCommand(command);
				break;
		}
	}

	private static async Task ExecuteFeedbackCommand(SocketSlashCommand command)
	{
		var modal = CreateFeedbackModal();
		await command.RespondWithModalAsync(modal);
	}

	private static Modal CreateFeedbackModal()
	{
		var builder = new ModalBuilder()
			.WithTitle("Feedback")
			.WithCustomId(ModalFields.FeedbackModalId)
			.AddTextInput("Feedback Text", ModalFields.FeedbackText, style:TextInputStyle.Paragraph);
		
		return builder.Build();
	}

	public Task ExecuteButton(SocketMessageComponent component)
		=> throw new NotImplementedException();

	public async Task ExecuteModal(SocketModal modal)
	{
		var id = modal.Data.CustomId;

		switch (id)
		{
			case ModalFields.FeedbackModalId:
				await ProcessFeedbackModal(modal);
				break;
		}
	}

	private async Task ProcessFeedbackModal(SocketModal modal)
	{
		var data = modal.Data.Components.ToList();
		await modal.RespondAsync(Resolver.GetString(x => CommandResource.Feedback_Received, _locale), ephemeral: true);

		var message = data.Get(ModalFields.FeedbackText).Value;

		var guild = client.GetGuild(modal.GuildId!.Value);
		var channel = guild.GetTextChannel(_channelId);

		var sentMessage = await channel.SendMessageAsync(
			" ",
			embed: FeedbackEmbed(message, (SocketGuildUser)modal.User)
		);
		
		if (_addReaction)
			await sentMessage.AddReactionsAsync(
				new List<IEmote>
				{
					new Emoji("👍"),
					new Emoji("👎"),
				});
	}

	private Embed FeedbackEmbed(string feedbackText, SocketGuildUser user)
	{
		var embedBuilder = new EmbedBuilder();

		var title = Resolver.GetString(x => EmbedResource.FeedbackEmbedTitle, _locale);
		embedBuilder.Title = string.Format(title, user.GetEmbedInfo(), DateTime.Now.ToString("dd.MM.yyyy"));

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

	public Task ExecuteSelect(SocketMessageComponent component)
		=> throw new NotImplementedException();
}