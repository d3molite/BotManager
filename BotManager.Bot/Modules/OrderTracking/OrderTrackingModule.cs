using BotManager.Bot.Interfaces.Modules;
using BotManager.Db.Models.Modules.Order;
using BotManager.Interfaces.Services.Data;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.OrderTracking;

public partial class OrderTrackingModule(IOrderService orderService, DiscordSocketClient client) : ICommandModule<OrderTrackingConfig>
{
	
}