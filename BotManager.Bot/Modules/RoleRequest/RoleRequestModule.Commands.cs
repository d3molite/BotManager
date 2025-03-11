using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models.Modules.RoleRequest;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.RoleRequest;

public partial class RoleRequestModule
{
	public async Task BuildCommands(RoleRequestConfig config, ulong guildId)
	{
		await BuildRoleRequestCommand(guildId);
	}

	private async Task BuildRoleRequestCommand(ulong guildId)
	{
		var guild = client.GetGuild(guildId);

		var command = new SlashCommandBuilder();
		command.WithName(Commands.LanRolle);
		command.WithDescription("Request your LAN.User role");
		command.AddDescriptionLocalization("de", "Frage deine LAN.User Rolle an");

		await guild.CreateApplicationCommandAsync(command.Build());
	}

	public async Task ExecuteCommands(SocketSlashCommand command)
	{
		switch (command.CommandName)
		{
			case Commands.LanRolle:
				await ExecuteRoleRequestCommand(command);
				break;
		}
	}

	private async Task ExecuteRoleRequestCommand(SocketSlashCommand command)
	{
		var guild = client.GetGuild(guildConfig.GuildId);
		var user = guild.GetUser(command.User.Id);

		if (user.Roles.Any(x => x.Id == _roleConfig.RoleId))
		{
			await command.RespondAsync("Du hast bereits die aktuelle LAN.User Rolle!", ephemeral:true);
			return;
		}

		var receiver = await client.GetUserAsync(_roleConfig.ReceiverId);
		var dmChannel = await receiver.CreateDMChannelAsync();

		await foreach (var message in dmChannel.GetMessagesAsync())
		{
			foreach (var item in message.Where(x => x.Embeds.Count > 0))
			{
				var request = ReadFromEmbed(item.Embeds.First());

				if (request.UserId != command.User.Id || request.Status != RoleRequestStatus.Open) 
					continue;

				await command.RespondAsync("Du hast bereits eine offene Anfrage.", ephemeral: true);
				return;
			}
		}
		
		var modal = CreateRoleRequestModal();
		await command.RespondWithModalAsync(modal);
	}
}