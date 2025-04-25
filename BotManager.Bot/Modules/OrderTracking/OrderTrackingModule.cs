using BotManager.Bot.Modules.Core;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.Order;
using BotManager.Interfaces.Services.Data;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.OrderTracking;

public partial class OrderTrackingModule(IOrderService orderService, DiscordSocketClient client, GuildConfig guildConfig)
	: AbstractCommandModuleBase<OrderTrackingConfig>(client, guildConfig)
{
}