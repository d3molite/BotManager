using System.ComponentModel.DataAnnotations.Schema;
using EfExtensions.Items.Model;

namespace BotManager.Db.Models.Modules.Order;

public class OrderItem : DbItem<string>
{
	public ulong UserId { get; set; }

	public string ItemName { get; set; } = "";
	
	public string? ItemNumber { get; set; } = "";

	public int ItemAmount { get; set; } = 1;
	
	public decimal ItemPrice { get; set; }

	[NotMapped]
	public decimal TotalPrice => ItemPrice * ItemAmount;
}