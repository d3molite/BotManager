using System.ComponentModel;
using System.Text;
using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models.Modules.Order;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.OrderTracking;

public partial class OrderTrackingModule
{
	private async Task UpdateOrderMessage(Order order, IUserMessage message)
	{
		var embeds = CreateOrderEmbed(order);
		var components = CreateOrderButtons(order);

		await message.ModifyAsync(
			msg =>
			{
				msg.Embeds = embeds.ToArray();
				msg.Components = components;
			}
		);
	}

	private static StringBuilder GetUserPingList(Order order)
	{
		var sb = new StringBuilder();

		if (order.OrderItems is null || !order.OrderItems.Any())
			return sb;

		var users = order.OrderItems.Select(x => x.UserId).Distinct();

		foreach (var user in users)
		{
			sb.AppendLine($"<@{user}>");
		}

		return sb;
	}

	private static MessageComponent CreateOrderButtons(Order order)
	{
		return new ComponentBuilder().WithButton("Mitbestellen", Components.OrderButtonAdd, disabled: !order.IsOpen)
									.WithButton(
										"Position löschen",
										Components.OrderButtonRemove,
										disabled: !order.IsOpen
									)
									.WithButton(
										"Essen ist da!",
										Components.OrderButtonArrived,
										ButtonStyle.Success,
										disabled: (order.IsOpen || order.IsDelivered),
										row: 1
									)
									.WithButton("Reload Embed", 
										Components.OrderButtonReload,
										ButtonStyle.Secondary,
										row:1)
									.WithButton(
										"Bestellung schließen",
										Components.OrderButtonClose,
										ButtonStyle.Secondary,
										row: 2,
										disabled: !order.IsOpen
									)
									.WithButton(
										"Bestellung löschen",
										Components.OrderButtonDelete,
										ButtonStyle.Danger,
										row: 2
									)
									.Build();
	}

	private IEnumerable<Embed> CreateOrderEmbed(Order order)
	{
		List<Embed> ret = new();
		
		string status;

		if (!order.IsOpen)
		{
			status = order.IsDelivered ? "Geliefert" : "Geschlossen";
		}
		else
		{
			status = "Offen";
		}
		

		var statusBuilder = new EmbedBuilder().WithTitle("Bestellung")
										.AddField("Besteller", $"<@{order.OwnerId}>")
										.AddField("Restaurant", order.RestaurantName)
										.AddField("Bestellung wird abgeschickt um", order.OrderTime)
										.AddField("Speisekarte", $"[klicke hier!]({ProcessLink(order.MenuLink)})")
										.AddField("Paypal:", $"[klicke hier!]({ProcessLink(order.PaypalLink)})")
										.AddField("Status:", status);

		if (order.OrderItems.Any())
		{
			statusBuilder.AddField("Total:", $"**{order.OrderItems.Sum(x => x.TotalPrice):0.00} €**");
		}
										
		
		ret.Add(statusBuilder.Build());
		
		ret.AddRange(AddOrderEmbedData(order));

		return ret;
	}
	
	private string ProcessLink(string input)
	{
		if (input.StartsWith("http://") || input.StartsWith("https://"))
			return input;

		if (input.StartsWith("www."))
			return $"http://{input}";

		return input;
	}

	private IEnumerable<Embed> AddOrderEmbedData(Order? order)
	{
		if (order?.OrderItems is null)
			return new List<Embed>();

		if (!order.OrderItems.Any())
			return new List<Embed>();
		
		var ret = new List<Embed>();
		
		var grouped = order.OrderItems.GroupBy(x => x.UserId);

		foreach (var chunk in grouped.Chunk(18))
		{
			var builder = new EmbedBuilder();
			
			foreach (var group in chunk)
			{
				var sb = new StringBuilder();

				foreach (var item in group.OrderBy(x => x.ItemNumber))
				{
					sb.AppendLine($"**{item.ItemAmount}x** - {item.ItemNumber} {item.ItemName} - {item.ItemPrice}€");
				}

				var user = client.GetUser(group.Key);
				builder.AddField($"{user.Username} ({group.Sum(x => x.TotalPrice):0.00}€)", sb.ToString());
			}
			
			ret.Add(builder.Build());
		}

		return ret;
	}

	private async Task SendOrderDeliveredMessage(Order order, ISocketMessageChannel componentChannel)
	{
		var sb = GetUserPingList(order);
		
		sb.AppendLine($"# ESSEN VON {order.RestaurantName} IST DA!");

		await componentChannel.SendMessageAsync(sb.ToString());
	}

	private async Task<MessageComponent> CreateRemoveFromOrder(SocketMessageComponent component)
	{
		var messageId = component.Message.Id;
		var userId = component.User.Id;

		var order = await orderService.GetOrderAsync(messageId);

		return new ComponentBuilder().WithRows(
										new List<ActionRowBuilder>()
										{
											new ActionRowBuilder().WithSelectMenu(
												new SelectMenuBuilder().WithCustomId(Components.OrderRemoveSelectMenu)
																		.WithOptions(
																			order!.OrderItems!.Where(
																					x => x.UserId == userId
																				)
																				.Select(
																					x => new SelectMenuOptionBuilder()
																						.WithLabel(x.ItemName)
																						.WithValue(x.Id)
																				)
																				.ToList()
																		)
											)
										}
									)
									.Build();
	}
}