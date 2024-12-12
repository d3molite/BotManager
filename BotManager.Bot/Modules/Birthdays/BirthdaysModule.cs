using System.Globalization;
using BotManager.Bot.Interfaces.Modules;
using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models.Modules.Birthdays;
using BotManager.Interfaces.Services.Data;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Birthdays;

public class BirthdaysModule(DiscordSocketClient client, IBirthdayService birthdayService) : ICommandModule<BirthdayConfig>
{
	private string _configId;

	private ulong _guildId;

	private ulong _channelId;

	private readonly PeriodicTimer _timer = new (TimeSpan.FromHours(1));
	
	public async Task BuildCommands(BirthdayConfig config, ulong guildId)
	{
		_configId = config.Id;
		_guildId = guildId;
		_channelId = config.PingChannelId;
		
		var guild = client.GetGuild(_guildId);

		await BuildAddCommand(guild);
		await BuildRemoveCommand(guild);
	}

	private async Task BuildAddCommand(SocketGuild guild)
	{
		var command = new SlashCommandBuilder();
		command.WithName(Commands.Birthday);
		command.WithDescription("Add your birthday! (DD.MM.YYYY)");
		command.AddDescriptionLocalization("de", "Füge deinen Geburtstag hinzu! (TT.MM.JJJJ)");
		command.AddOption("date", ApplicationCommandOptionType.String, "Your birth date (DD.MM.YYYY)", isRequired: true);

		await guild.CreateApplicationCommandAsync(command.Build());
	}

	private async Task BuildRemoveCommand(SocketGuild guild)
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
			var birthdays = await birthdayService.GetBirthdays(_configId, _guildId);

			var now = DateTime.Now;
			
			if (now.Hour != 13) continue;

			var birthday = birthdays.FirstOrDefault(x => x.Date == DateOnly.FromDateTime(now));

			if (birthday is null) continue;

			var guild = client.GetGuild(_guildId);
			var channel = guild.GetChannel(_channelId);

			if (channel is ISocketMessageChannel messageChannel)
				await messageChannel.SendMessageAsync($"<@{birthday.UserId}> hat heute Geburtstag!!!111!!!");
		}
	}

	public async Task ExecuteCommands(SocketSlashCommand command)
	{
		switch (command.CommandName)
		{
			case Commands.Birthday:
				await ExecuteBirthdayCommand(command);
				break;
			
			case Commands.ClearBirthday:
				await ExecuteClearBirthdayCommand(command);
				break;
		}
	}

	public Task ExecuteButton(SocketMessageComponent component) => throw new NotImplementedException();

	public Task ExecuteModal(SocketModal modal) => throw new NotImplementedException();

	public Task ExecuteSelect(SocketMessageComponent component) => throw new NotImplementedException();

	private async Task ExecuteBirthdayCommand(SocketSlashCommand command)
	{
		var date = command.Data.Options.First().Value.ToString();

		try
		{
			var dateTime = DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
			var dateOnly = DateOnly.FromDateTime(dateTime);

			await birthdayService.Upsert(_configId, command.GuildId!.Value, command.User.Id, dateOnly);

			await command.RespondAsync($"Added your birthday! ({dateOnly.ToString("dd.MM.yyyy")})", ephemeral:true);
		}
		catch
		{
			await command.RespondAsync($"Error while parsing: {date}", ephemeral:true);
		}
		
	}
	
	private async Task ExecuteClearBirthdayCommand(SocketSlashCommand command)
	{
		var result = await birthdayService.Delete(_configId, command.GuildId!.Value, command.User.Id);

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