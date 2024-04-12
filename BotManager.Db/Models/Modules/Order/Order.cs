using EfExtensions.Items.Model;

namespace BotManager.Db.Models.Modules.Order;

public class Order : DbItem<string>
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