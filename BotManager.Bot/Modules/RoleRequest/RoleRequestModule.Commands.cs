using BotManager.Bot.Attributes;
using BotManager.Bot.Extensions;
using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models.Modules.RoleRequest;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.RoleRequest;

public partial class RoleRequestModule
{
	
	[CommandBuilder(Commands.LanRolle)]
	public async Task BuildRoleRequestCommand(SocketGuild guild)
	{
		var builder = new SlashCommandBuilder();
		builder.WithName(Commands.LanRolle);
		builder.WithDescription("Request your LAN.User role");
		builder.AddDescriptionLocalization("de", "Frage deine LAN.User Rolle an");

		await guild.TryCreateGuildCommand(builder);
	}


	[CommandExecutor(Commands.LanRolle)]
	public async Task ExecuteRoleRequestCommand(SocketSlashCommand command)
	{
		var guild = Client.GetGuild(GuildConfig.GuildId);
		var user = guild.GetUser(command.User.Id);

		if (user.Roles.Any(x => x.Id == ModuleConfig.RoleId))
		{
			await command.RespondAsync("Du hast bereits die aktuelle LAN.User Rolle!", ephemeral:true);
			return;
		}

		var receiver = await Client.GetUserAsync(ModuleConfig.ReceiverId);
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