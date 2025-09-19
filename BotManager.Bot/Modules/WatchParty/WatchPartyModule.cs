using BotManager.Bot.Attributes;
using BotManager.Bot.Extensions;
using BotManager.Bot.Modules.Core;
using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.WatchParty;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.WatchParty;

public class WatchPartyModule(DiscordSocketClient client, GuildConfig config)
	: AbstractCommandModuleBase<WatchPartyConfig>(client, config)
{
	public override string ModuleName => "Watch Party";
	
	[CommandBuilder(Commands.WatchParty)]
	public async Task BuildWatchPartyCommand(SocketGuild guild)
	{
		var builder = new SlashCommandBuilder();
		builder.WithName(Commands.WatchParty);
		builder.WithDescription("Create a watch party request");
		builder.AddDescriptionLocalization("de", "Erstelle eine Watchparty");

		await guild.TryCreateGuildCommand(builder);
	}

	[CommandExecutor(Commands.WatchParty)]
	public async Task ExecuteWatchPartyCommand(SocketSlashCommand command)
		=> await command.RespondWithModalAsync(CreateWatchPartyModal());

	[ModalExecutor(Modals.WatchPartyModalId)]
	public async Task ExecuteWatchPartyModal(SocketModal modal)
	{
		var data = new WatchPartyData();

		data.Name = modal.Data.Components.GetString(Modals.WatchPartyModalName);
		data.Time = modal.Data.Components.GetString(Modals.WatchPartyModalTime);

		await modal.RespondAsync(
			$"<@&{ModuleConfig.PingRoleId}>",
			embed: CreateWatchPartyEmbed(data),
			components: CreateWatchPartyButtons(),
			allowedMentions: AllowedMentions.All
		);
	}

	[MessageComponentExecutor(Components.WatchPartyButtonJoin)]
	[MessageComponentExecutor(Components.WatchPartyButtonInterestedPleaseWait)]
	[MessageComponentExecutor(Components.WatchPartyButtonInterestedDontWait)]
	[MessageComponentExecutor(Components.WatchPartyButtonNotInterested)]
	public async Task ExecuteButton(SocketMessageComponent component)
	{
		var message = component.Message;
		var embed = message.Embeds.First();
		var userId = component.User.Id;

		var data = GetDataFromEmbed(embed);

		switch (component.Data.CustomId)
		{
			case Components.WatchPartyButtonJoin:
				data.MoveIntoList(x => x.JoinedUsers, userId);
				break;

			case Components.WatchPartyButtonInterestedPleaseWait:
				data.MoveIntoList(x => x.InterestedUsers, userId);
				break;
			
			case Components.WatchPartyButtonInterestedDontWait:
				data.MoveIntoList(x => x.DontWaitUsers, userId);
				break;

			case Components.WatchPartyButtonNotInterested:
				data.MoveIntoList(x => x.NotInterestedUsers, userId);
				break;
		}

		await UpdateWatchPartyMessage(data, component.Message);

		await component.RespondAsync("Ok!", ephemeral: true);
	}

	private static WatchPartyData GetDataFromEmbed(Embed embed)
	{
		var title = embed.Fields[0];
		var time = embed.Fields[1];

		var usersJoined = embed.Fields[2];
		var usersInterested = embed.Fields[3];
		var usersDontWait = embed.Fields[4];
		var usersNotInterested = embed.Fields[5];

		var data = new WatchPartyData()
		{
			Name = title.Value,
			Time = time.Value,
			JoinedUsers = SplitIntoUserIds(usersJoined.Value),
			InterestedUsers = SplitIntoUserIds(usersInterested.Value),
			DontWaitUsers = SplitIntoUserIds(usersDontWait.Value),
			NotInterestedUsers = SplitIntoUserIds(usersNotInterested.Value),
		};

		return data;
	}

	private static List<ulong> SplitIntoUserIds(string embedData)
	{
		if (embedData == "-")
			return [];

		var splitData = embedData.Split(Environment.NewLine);

		var toReturn = new List<ulong>();

		foreach (var line in splitData)
		{
			var processed = line.Replace("<@", "").Replace(">", "").Trim();
			toReturn.Add(ulong.Parse(processed));
		}

		return toReturn;
	}

	private static Modal CreateWatchPartyModal()
	{
		var builder = new ModalBuilder();

		builder.WithCustomId(Modals.WatchPartyModalId);
		builder.WithTitle("Watchparty");
		builder.AddTextInput("Wir wollen schauen:", Modals.WatchPartyModalName);
		builder.AddTextInput("Wann?", Modals.WatchPartyModalTime);

		return builder.Build();
	}

	private static Embed CreateWatchPartyEmbed(WatchPartyData data)
	{
		var builder = new EmbedBuilder();
		builder.WithTitle($"Watchparty");
		builder.AddField("Wir schauen:", data.Name);
		builder.AddField("Wann?", data.Time);
		builder.AddField("Da:", data.GetUserList(x => x.JoinedUsers));
		builder.AddField("Nicht da, bitte wartet:", data.GetUserList(x => x.InterestedUsers));
		builder.AddField("Nicht da, müsst nicht warten:", data.GetUserList(x => x.DontWaitUsers));
		builder.AddField("Nicht interessiert:", data.GetUserList(x => x.NotInterestedUsers));

		return builder.Build();
	}

	private static MessageComponent CreateWatchPartyButtons()
	{
		var builder = new ComponentBuilder()
			.WithButton("Bin Dabei", Components.WatchPartyButtonJoin, ButtonStyle.Success)
			.WithButton("Nicht da, bitte wartet", Components.WatchPartyButtonInterestedPleaseWait, ButtonStyle.Secondary)
			.WithButton("Prinzipiell interesse, müsst aber nicht warten", Components.WatchPartyButtonInterestedDontWait, ButtonStyle.Secondary)
			.WithButton("Nicht interessiert", Components.WatchPartyButtonNotInterested, ButtonStyle.Danger);

		return builder.Build();
	}

	private static async Task UpdateWatchPartyMessage(WatchPartyData data, IUserMessage message)
	{
		var embed = CreateWatchPartyEmbed(data);
		var components = CreateWatchPartyButtons();

		await message.ModifyAsync(msg =>
			{
				msg.Embed = embed;
				msg.Components = components;
			}
		);
	}
}