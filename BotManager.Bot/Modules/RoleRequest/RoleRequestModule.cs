using BotManager.Bot.Modules.Core;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.RoleRequest;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.RoleRequest;

public partial class RoleRequestModule(DiscordSocketClient client, GuildConfig guildConfig)
	: AbstractCommandModuleBase<RoleRequestConfig>(client, guildConfig)
{
	public override string ModuleName => "Role Request";
}