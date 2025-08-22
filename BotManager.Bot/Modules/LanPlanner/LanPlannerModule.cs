using BotManager.Bot.Modules.Core;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.LanPlanner;
using BotManager.Interfaces.Services.Data;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.LanPlanner;

public partial class LanPlannerModule(DiscordSocketClient client, GuildConfig guildConfig, ILanPlanService planService)
	: AbstractCommandModuleBase<LanPlannerConfig>(client, guildConfig)
{
}