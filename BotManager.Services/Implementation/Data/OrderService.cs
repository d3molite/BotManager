using BotManager.Db.Interfaces;
using BotManager.Db.Models.Modules.Order;
using BotManager.Interfaces.Services.Data;
using EfExtensions.Core.Enum;
using EfExtensions.Core.Interfaces.Result;
using EfExtensions.Items.Result;

namespace BotManager.Services.Implementation.Data;

public class OrderService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository) : IOrderService
{
	public async Task<Order?> GetOrderAsync(ulong messageId)
	{
		var result = await orderRepository.GetCustomAsync(x => x.MessageId == messageId);

		if (result is null) return null;
		
		result.OperationType = Operation.Updated;

		foreach (var item in result.OrderItems)
		{
			item.OperationType = Operation.Updated;
		}

		return result;
	}


	public async Task<IDbResult<Order>> CreateOrderAsync(Order order)
	{
		var result = await orderRepository.CreateAsync(order);
		return result;
	}

	public async Task<IDbResult<Order>> UpdateOrderAsync(Order order)
	{
		order.OperationType = Operation.Updated;

		if (order.OrderItems != null && order.OrderItems.Any())
		{
			
			var firstResult = await orderItemRepository.CrudManyAsync(order.OrderItems);

			var isFailed = firstResult.FirstOrDefault(x => x.Success == false);
			
			if (isFailed != null)
				return DbResult<Order>.Failed(order, isFailed.ErrorMessage);
		}
		
		return await orderRepository.CrudAsync(order);
	}

	public async Task<IDbResult<OrderItem>> RemoveFromOrderAsync(OrderItem item)
	{
		item.OperationType = Operation.Removed;
		return await orderItemRepository.CrudAsync(item);
	}

	public async Task<IDbResult<Order>> DeleteOrderAsync(Order order)
	{
		var orderItems = order.OrderItems;
		order.OperationType = Operation.Removed;

		if (order.OrderItems != null && order.OrderItems.Any())
		{
			foreach (var orderItem in orderItems)
			{
				orderItem.OperationType = Operation.Removed;
			}
			var firstResult = await orderItemRepository.CrudManyAsync(orderItems);
			
			var isFailed = firstResult.FirstOrDefault(x => x.Success == false);
			
			if (isFailed != null)
				return DbResult<Order>.Failed(order, isFailed.ErrorMessage);
		}

		order.OrderItems = new();
		return await orderRepository.CrudAsync(order);
	}
}