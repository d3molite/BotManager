using System.ComponentModel.DataAnnotations.Schema;
using Demolite.Db.Models;

namespace BotManager.Db.Models.Modules.Order;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class OrderItem : AbstractDbItem
{
	public ulong UserId { get; set; }

	public string ItemName { get; set; } = "";
	
	public string? ItemNumber { get; set; } = "";

	public int ItemAmount { get; set; } = 1;
	
	public decimal ItemPrice { get; set; }

	[NotMapped]
	public decimal TotalPrice => ItemPrice * ItemAmount;
}