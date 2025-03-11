using BotManager.Bot.Interfaces.Modules;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.RoleRequest;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.RoleRequest;

public partial class RoleRequestModule(DiscordSocketClient client, GuildConfig guildConfig) : ICommandModule<RoleRequestConfig>
{
	private readonly RoleRequestConfig _roleConfig = guildConfig.RoleRequestConfig!;

	public Task ExecuteSelect(SocketMessageComponent component)
		=> throw new NotImplementedException();
}