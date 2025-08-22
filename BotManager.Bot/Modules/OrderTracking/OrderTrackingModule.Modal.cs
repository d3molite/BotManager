using System.Globalization;
using BotManager.Bot.Attributes;
using BotManager.Bot.Extensions;
using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models.Modules.Order;
using Demolite.Db.Enum;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.OrderTracking;

public partial class OrderTrackingModule
{
	[ModalExecutor(Modals.OrderModalId)]
	public async Task ProcessOrderModal(SocketModal modal)
	{
		var data = modal.Data.Components.ToList();
		await modal.RespondAsync("Creating.", ephemeral: true);
		
# if DEBUG
		var mentions = AllowedMentions.None;
# elif RELEASE
		var mentions = AllowedMentions.All;
# endif

		var message = await modal.Channel.SendMessageAsync("@here", allowedMentions: mentions);

		var order = new Order()
		{
			OperationType = Operation.Created,
			IsOpen = true,
			MessageId = message.Id,
			OwnerId = modal.User.Id,
			RestaurantName = data.Get(Modals.OrderModalRestaurant).Value,
			MenuLink = data.Get(Modals.OrderModalMenuLink).Value,
			OrderTime = data.Get(Modals.OrderModalTime).Value,
			PaypalLink = data.Get(Modals.OrderModalPaypalLink).Value
		};

		var created = await orderService.CreateOrderAsync(order);

		if (!created.Success)
			return;
		
		await UpdateOrderMessage(created.Item, message);
	}

	[ModalExecutor(Modals.OrderModalAddId)]
	public async Task ProcessAddToOrderModal(SocketModal modal)
	{
		var data = modal.Data.Components.ToList();
		var order = await orderService.GetOrderAsync(modal.Message.Id);

		if (order is null || !order.IsOpen)
		{
			await modal.RespondAsync("Die Bestellung kann nicht mehr bearbeitet werden!", ephemeral: true);
			return;
		}
		
		var orderItem = new OrderItem()
		{
			Id = Guid.NewGuid().ToString(),
			OperationType = Operation.Created,
			ItemName = data.Get(Modals.OrderModalAddName).Value,
			ItemPrice = ParseModalDecimal(data.Get(Modals.OrderModalAddPrice).Value),
			ItemNumber = data.GetString(Modals.OrderModalAddNumber),
			ItemAmount = ParseModalInt(data.Get(Modals.OrderModalAddAmount).Value),
			UserId = modal.User.Id,
		};

		if (order.OrderItems is null)
		{
			order.OrderItems = new List<OrderItem>()
			{
				orderItem
			};
		}
		else
		{
		
			foreach (var item in order.OrderItems)
			{
				item.OperationType = Operation.Updated;
			}
			order.OrderItems.Add(orderItem);
		}
		
		var updated = await orderService.UpdateOrderAsync(order);

		if (!updated.Success)
		{
			await modal.RespondAsync("Ein Fehler ist Aufgetreten.", embed: new EmbedBuilder().AddField("Error:", updated.ErrorMessage).Build(), ephemeral:true);
			return;
		}
		
		await UpdateOrderMessage(updated.Item, modal.Message);
		await modal.RespondAsync("Erledigt!", ephemeral: true);
	}

	
	
	private static Modal CreateOrderModal()
	{
		var builder = new ModalBuilder()
					.WithTitle("Neue Bestellung")
					.WithCustomId(Modals.OrderModalId)
					.AddTextInput("Restaurant", Modals.OrderModalRestaurant, placeholder: "Da Luigi")
					.AddTextInput("Bestellzeit", Modals.OrderModalTime, placeholder: "19:00")
					.AddTextInput("Paypal Bezahllink", Modals.OrderModalPaypalLink, placeholder:"https://paypal.me/...")
					.AddTextInput("Link zur Speisekarte", Modals.OrderModalMenuLink, placeholder: "https://www....");
		
		return builder.Build();
	}

	private static Modal CreateAddToOrderModal()
	{
		var builder = new ModalBuilder().WithTitle("Artikel Hinzufügen")
										.WithCustomId(Modals.OrderModalAddId)
										.AddTextInput(
											"Nummer auf Speisekarte",
											Modals.OrderModalAddNumber,
											required: false,
											placeholder: "Optional"
										)
										.AddTextInput(
											"Name auf der Speisekarte",
											Modals.OrderModalAddName,
											placeholder: ""
										)
										.AddTextInput("Preis", Modals.OrderModalAddPrice, placeholder: "3.50")
										.AddTextInput("Menge", Modals.OrderModalAddAmount, value: "1");

		return builder.Build();
	}

	
}