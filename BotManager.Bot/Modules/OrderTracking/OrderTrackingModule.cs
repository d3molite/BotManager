using BotManager.Bot.Modules.Core;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.Order;
using BotManager.Interfaces.Services.Data;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.OrderTracking;

public partial class OrderTrackingModule(DiscordSocketClient client, GuildConfig guildConfig, IOrderService orderService)
	: AbstractCommandModuleBase<OrderTrackingConfig>(client, guildConfig)
{
	public override string ModuleName => "Order Tracking";
}