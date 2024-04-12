using BotManager.Db.Models.Modules.Order;
using EfExtensions.Core.Interfaces.Result;

namespace BotManager.Interfaces.Services.Data;

public interface IOrderService
{
	public Task<Order?> GetOrderAsync(ulong messageId);

	public Task<IDbResult<Order>> CreateOrderAsync(Order order);

	public Task<IDbResult<Order>> UpdateOrderAsync(Order order);

	public Task<IDbResult<OrderItem>> RemoveFromOrderAsync(OrderItem item);

	public Task<IDbResult<Order>> DeleteOrderAsync(Order order);
}