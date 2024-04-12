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
		var embed = CreateOrderEmbed(order);
		var components = CreateOrderButtons(order);

		await message.ModifyAsync(
			msg =>
			{
				msg.Embed = embed;
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
		return new ComponentBuilder().WithButton("Mitbestellen", ControlNames.OrderButtonAdd, disabled: !order.IsOpen)
									.WithButton(
										"Position löschen",
										ControlNames.OrderButtonRemove,
										disabled: !order.IsOpen
									)
									.WithButton(
										"Essen ist da!",
										ControlNames.OrderButtonArrived,
										ButtonStyle.Success,
										disabled: (order.IsOpen || order.IsDelivered),
										row: 1
									)
									.WithButton(
										"Bestellung schließen",
										ControlNames.OrderButtonClose,
										ButtonStyle.Secondary,
										row: 2,
										disabled: !order.IsOpen
									)
									.WithButton(
										"Bestellung löschen",
										ControlNames.OrderButtonDelete,
										ButtonStyle.Danger,
										row: 2
									)
									.Build();
	}

	private Embed CreateOrderEmbed(Order order)
	{
		string status;

		if (!order.IsOpen)
		{
			status = order.IsDelivered ? "Geliefert" : "Geschlossen";
		}
		else
		{
			status = "Offen";
		}

		var builder = new EmbedBuilder().WithTitle("Bestellung")
										.AddField("Besteller", $"<@{order.OwnerId}>")
										.AddField("Restaurant", order.RestaurantName)
										.AddField("Bestellung wird abgeschickt um", order.OrderTime)
										.AddField("Speisekarte", $"[klicke hier!]({ProcessLink(order.MenuLink)})")
										.AddField("Paypal:", $"[klicke hier!]({ProcessLink(order.PaypalLink)})")
										.AddField("Status:", status);

		AddOrderEmbedData(builder, order);

		return builder.Build();
	}
	
	private string ProcessLink(string input)
	{
		if (input.StartsWith("http://") || input.StartsWith("https://"))
			return input;

		if (input.StartsWith("www."))
			return $"http://{input}";

		return input;
	}

	private void AddOrderEmbedData(EmbedBuilder builder, Order? order)
	{
		if (order?.OrderItems is null)
			return;

		if (!order.OrderItems.Any())
			return;
		
		var grouped = order.OrderItems.GroupBy(x => x.UserId);

		foreach (var group in grouped)
		{
			var sb = new StringBuilder();

			foreach (var item in group.OrderBy(x => x.ItemNumber))
			{
				sb.AppendLine($"{item.ItemAmount}x - {item.ItemNumber}{item.ItemName} - {item.ItemPrice}€");
			}

			var user = client.GetUser(group.Key);
			builder.AddField($"{user.Username} ({group.Sum(x => x.TotalPrice):0.00}€)", sb.ToString());
		}
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
												new SelectMenuBuilder().WithCustomId(ControlNames.OrderRemoveSelectMenu)
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