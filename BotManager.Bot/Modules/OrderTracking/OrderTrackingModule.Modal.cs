using System.Globalization;
using System.Security.Cryptography;
using BotManager.Bot.Extensions;
using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models.Modules.Order;
using Discord;
using Discord.WebSocket;
using EfExtensions.Core.Enum;

namespace BotManager.Bot.Modules.OrderTracking;

public partial class OrderTrackingModule
{
	public async Task ExecuteModal (SocketModal modal)
	{
		var id = modal.Data.CustomId;

		switch (id)
		{
			case ModalFields.OrderModalId:
				await ProcessOrderModal(modal);
				break;
			
			case ModalFields.OrderModalAddId:
				await ProcessAddToOrderModal(modal);
				break;
		}
		
	}
	
	private async Task ProcessOrderModal(SocketModal modal)
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
			RestaurantName = data.Get(ModalFields.OrderModalRestaurant).Value,
			MenuLink = data.Get(ModalFields.OrderModalMenuLink).Value,
			OrderTime = data.Get(ModalFields.OrderModalTime).Value,
			PaypalLink = data.Get(ModalFields.OrderModalPaypalLink).Value
		};

		var created = await orderService.CreateOrderAsync(order);

		if (!created.Success)
			return;
		
		await UpdateOrderMessage(created.Item, message);
	}

	private async Task ProcessAddToOrderModal(SocketModal modal)
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
			ItemName = data.Get(ModalFields.OrderModalAddName).Value,
			ItemPrice = ParseModalDecimal(data.Get(ModalFields.OrderModalAddPrice).Value),
			ItemNumber = data.GetString(ModalFields.OrderModalAddNumber),
			ItemAmount = ParseModalInt(data.Get(ModalFields.OrderModalAddAmount).Value),
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

	private static decimal ParseModalDecimal(string input)
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		
		input = input.Replace(",", ".");
		return decimal.TryParse(input, out var value) ? value : 0m;
	}

	private static int ParseModalInt(string input)
	{
		return int.TryParse(input, out var value) ? value : 1;
	}
	
	private static Modal CreateOrderModal()
	{
		var builder = new ModalBuilder()
					.WithTitle("Neue Bestellung")
					.WithCustomId(ModalFields.OrderModalId)
					.AddTextInput("Restaurant", ModalFields.OrderModalRestaurant, placeholder: "Da Luigi")
					.AddTextInput("Bestellzeit", ModalFields.OrderModalTime, placeholder: "19:00")
					.AddTextInput("Paypal Bezahllink", ModalFields.OrderModalPaypalLink, placeholder:"https://paypal.me/...")
					.AddTextInput("Link zur Speisekarte", ModalFields.OrderModalMenuLink, placeholder: "https://www....");
		
		return builder.Build();
	}

	private static Modal CreateAddToOrderModal()
	{
		var builder = new ModalBuilder().WithTitle("Artikel Hinzufügen")
										.WithCustomId(ModalFields.OrderModalAddId)
										.AddTextInput(
											"Nummer auf Speisekarte",
											ModalFields.OrderModalAddNumber,
											required: false,
											placeholder: "Optional"
										)
										.AddTextInput(
											"Name auf der Speisekarte",
											ModalFields.OrderModalAddName,
											placeholder: ""
										)
										.AddTextInput("Preis", ModalFields.OrderModalAddPrice, placeholder: "3.50")
										.AddTextInput("Menge", ModalFields.OrderModalAddAmount, value: "1");

		return builder.Build();
	}

	
}