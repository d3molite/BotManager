using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models.Modules.Order;
using Demolite.Db.Enum;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.OrderTracking;

public partial class OrderTrackingModule
{
	public async Task ExecuteButton(SocketMessageComponent component)
	{
		var id = component.Data.CustomId;

		switch (id)
		{
			case Components.OrderButtonClose:
				await CloseOrder(component);
				break;
			
			case Components.OrderButtonDelete:
				await DeleteOrder(component);
				break;
			
			case Components.OrderButtonAdd:
				await AddToOrder(component);
				break;
			
			case Components.OrderButtonArrived:
				await MarkAsDelivered(component);
				break;
			
			case Components.OrderButtonRemove:
				await RemoveFromOrder(component);
				break;
			
			case Components.OrderButtonReload:
				await RegenerateEmbed(component);
				break;
			
			case Components.OrderRemoveSelectMenu:
				await ExecuteSelect(component);
				break;
		}
	}

	private async Task RegenerateEmbed(SocketMessageComponent component)
	{
		var order = await CheckForOwner(component);
		if (order is null) return;
		
		await UpdateOrderMessage(order, component.Message);

		await component.RespondAsync("Embed neu generiert.", ephemeral: true);
	}
	
	private async Task DeleteOrder(SocketMessageComponent component)
	{
		var order = await CheckForOwner(component);
		if (order is null) return;

		var sb = GetUserPingList(order);
		
		sb.AppendLine($"# Die Bestellung bei {order.RestaurantName} von <@{order.OwnerId}> wurde abgebrochen.");

		var result = await orderService.DeleteOrderAsync(order);

		if (!result.Success)
		{
			await component.RespondAsync("Ein Fehler ist Aufgetreten.", embed: new EmbedBuilder().AddField("Error:", result.ErrorMessage).Build(), ephemeral:true);
			return;
		}
		
		await component.Message.DeleteAsync();

		await component.Message.Channel.SendMessageAsync(
			sb.ToString()
		);
		
		await component.RespondAsync("Bestellung gelöscht.", ephemeral: true);
	}
	
	private async Task CloseOrder(SocketMessageComponent component)
	{
		var order = await CheckForOwner(component);
		if (order is null) return;

		order.IsOpen = false;
		
		foreach (var orderItem in order.OrderItems)
		{
			orderItem.OperationType = Operation.None;
		}

		var updated = await orderService.UpdateOrderAsync(order);
		
		if (!updated.Success)
		{
			await component.RespondAsync("Ein Fehler ist Aufgetreten.", embed: new EmbedBuilder().AddField("Error:", updated.ErrorMessage).Build(), ephemeral:true);
			return;
		}

		await UpdateOrderMessage(order, component.Message);

		await component.RespondAsync("Bestellung geschlossen.", ephemeral: true);
	}

	private async Task MarkAsDelivered(SocketMessageComponent component)
	{
		var order = await CheckForOwner(component);
		if (order is null) return;

		order.IsOpen = false;
		order.IsDelivered = true;
		
		foreach (var orderItem in order.OrderItems)
		{
			orderItem.OperationType = Operation.None;
		}

		var result = await orderService.UpdateOrderAsync(order);

		if (!result.Success)
		{
			await component.RespondAsync("Ein Fehler ist Aufgetreten.", embed: new EmbedBuilder().AddField("Error:", result.ErrorMessage).Build(), ephemeral:true);
			return;
		}

		await UpdateOrderMessage(order, component.Message);

		await SendOrderDeliveredMessage(order, component.Channel);

		await component.RespondAsync("Erledigt.", ephemeral: true);
	}
	
	private async Task<Order?> CheckForOwner(SocketMessageComponent component)
	{
		var order = await orderService.GetOrderAsync(component.Message.Id);
		var userId = component.User.Id;

		if (order!.OwnerId == userId) return order;

		await component.RespondAsync("Du bist nicht der Besitzer dieser Bestellung!", ephemeral:true);
		return null;
	}

	private static async Task AddToOrder(SocketMessageComponent component)
	{
		await component.RespondWithModalAsync(CreateAddToOrderModal());
	}

	private async Task RemoveFromOrder(SocketMessageComponent component)
	{
		var order = await orderService.GetOrderAsync(component.Message.Id);

		if (order?.OrderItems is null || !order.OrderItems.Any())
		{
			await component.RespondAsync("Du hast keine Artikel!", ephemeral: true);
			return;
		}
		
		if (order.OrderItems.All(x => x.UserId != component.User.Id))
		{
			await component.RespondAsync("Du hast keine Artikel!", ephemeral: true);
			return;
		}
		
		await component.RespondAsync(
			"Bitte Artikel zum Löschen auswählen", 
			components: await CreateRemoveFromOrder(component), 
			ephemeral:true);
	}
}