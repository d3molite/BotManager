using System.Globalization;
using BotManager.Bot.Attributes;
using BotManager.Bot.Interfaces.Modules;
using BotManager.Bot.Modules.Core;
using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.Birthdays;
using BotManager.Interfaces.Services.Data;
using Discord;
using Discord.WebSocket;

// Disable private warning because of reflection access.
// ReSharper disable MemberCanBePrivate.Global
namespace BotManager.Bot.Modules.Birthdays;

public class BirthdaysModule : AbstractCommandModuleBase<BirthdayConfig>
{
	private string ConfigId => ModuleConfig.Id;
	private ulong GuildId => GuildConfig.GuildId;

	private ulong NotificationChannelId => ModuleConfig.PingChannelId;

	private readonly PeriodicTimer _timer = new(TimeSpan.FromHours(1));
	private readonly IBirthdayService _birthdayService;

	public BirthdaysModule(DiscordSocketClient client, IBirthdayService birthdayService, GuildConfig guildConfig) : base(client, guildConfig)
	{
		_birthdayService = birthdayService;
		
		Task.Run(async () => await StartCheckTask());
	}

	[CommandBuilder(Commands.Birthday)]
	public static async Task BuildAddCommand(SocketGuild guild)
	{
		var command = new SlashCommandBuilder();
		command.WithName(Commands.Birthday);
		command.WithDescription("Add your birthday! (DD.MM.YYYY)");
		command.AddDescriptionLocalization("de", "Füge deinen Geburtstag hinzu! (TT.MM.JJJJ)");

		command.AddOption(
			"date",
			ApplicationCommandOptionType.String,
			"Your birth date (DD.MM.YYYY)",
			isRequired: true
		);

		await guild.CreateApplicationCommandAsync(command.Build());
	}

	[CommandBuilder(Commands.Birthday)]
	public static async Task BuildRemoveCommand(SocketGuild guild)
	{
		var command = new SlashCommandBuilder();
		command.WithName(Commands.ClearBirthday);
		command.WithDescription("Delete your birthday.");
		command.AddDescriptionLocalization("de", "Lösche deinen Geburtstag.");

		await guild.CreateApplicationCommandAsync(command.Build());
	}

	public async Task StartCheckTask()
	{
		while (await _timer.WaitForNextTickAsync(CancellationToken.None))
		{
			var now = DateTime.Now;
			if (now.Hour != 13) continue;

			var birthdays = await _birthdayService.GetBirthdays(ConfigId, GuildId);
			var birthday = birthdays.FirstOrDefault(x => x.Date.Month == now.Month && x.Date.Day == now.Day);

			if (birthday is null) continue;

			var guild = Client.GetGuild(GuildId);
			var channel = guild.GetChannel(NotificationChannelId);

			if (channel is ISocketMessageChannel messageChannel)
				await messageChannel.SendMessageAsync($"<@{birthday.UserId}> hat heute Geburtstag!!!111!!!");
		}
	}
	
	[CommandExecutor(Commands.Birthday)]
	public async Task ExecuteBirthdayCommand(SocketSlashCommand command)
	{
		var date = command
					.Data.Options.First()
					.Value.ToString();

		try
		{
			var dateTime = DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
			var dateOnly = DateOnly.FromDateTime(dateTime);

			await _birthdayService.Upsert(ConfigId, command.GuildId!.Value, command.User.Id, dateOnly);

			await command.RespondAsync($"Added your birthday! ({dateOnly.ToString("dd.MM.yyyy")})", ephemeral: true);
		}
		catch
		{
			await command.RespondAsync($"Error while parsing: {date}", ephemeral: true);
		}
	}

	[CommandExecutor(Commands.ClearBirthday)]
	public async Task ExecuteClearBirthdayCommand(SocketSlashCommand command)
	{
		var result = await _birthdayService.Delete(ConfigId, command.GuildId!.Value, command.User.Id);

		if (result)
		{
			await command.RespondAsync("Deleted your birthday!", ephemeral: true);
		}
		else
		{
			await command.RespondAsync("Error while deleting your birthday...", ephemeral: true);
		}
	}
}