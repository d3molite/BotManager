using Demolite.Db.Models;

namespace BotManager.Db.Models.Modules.Order;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class Order : AbstractDbItem
{
	public ulong OwnerId { get; set; }
	public ulong MessageId { get; set; }
	
	public bool IsOpen { get; set; }
	
	public bool IsDelivered { get; set; }
	
	public string RestaurantName { get; set; }
	
	public string OrderTime { get; set; }
	
	public string MenuLink { get; set; }
	
	public string PaypalLink { get; set; }

	public List<OrderItem> OrderItems { get; set; } = [];
}