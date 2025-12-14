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
			allowedMentions: AllowedMentions.All
		);
	}

	private static Modal CreateWatchPartyModal()
	{
		var builder = new ModalBuilder();

		builder.WithCustomId(Modals.WatchPartyModalId);
		builder.WithTitle("Watchparty");
		builder.AddTextInput("Wir schauen:", Modals.WatchPartyModalName);
		builder.AddTextInput("Wann?", Modals.WatchPartyModalTime);

		return builder.Build();
	}

	private static Embed CreateWatchPartyEmbed(WatchPartyData data)
	{
		var builder = new EmbedBuilder();
		builder.WithTitle($"Watchparty");
		builder.AddField("Wir schauen:", data.Name);
		builder.AddField("Wann?", data.Time);

		return builder.Build();
	}
}