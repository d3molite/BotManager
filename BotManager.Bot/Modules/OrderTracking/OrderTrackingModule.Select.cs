using BotManager.Bot.Attributes;
using BotManager.Bot.Modules.Definitions;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.OrderTracking;

public partial class OrderTrackingModule
{
	[MessageComponentExecutor(Components.OrderRemoveSelectMenu)]
	public async Task ExecuteSelect(SocketMessageComponent component)
	{
		var message = await component.Channel.GetMessageAsync(component.Message.Reference.MessageId.Value);
		
		var data = component.Data.Values.First();

		var order = await orderService.GetOrderAsync(message.Id);
		var position = order!.OrderItems.FirstOrDefault(x => x.Id == data);

		if (position is null)
		{
			await component.UpdateAsync(message =>
			{
				message.Content = "Diese Position existiert nicht mehr.";
				message.Components = null;
			});
			return;
		}
		
		var result = await orderService.RemoveFromOrderAsync(position);

		if (!result.Success)
		{
			await component.UpdateAsync(message =>
			{
				message.Content = "Ein Fehler ist Aufgetreten.";
				message.Embed = new EmbedBuilder().AddField("Error:", result.ErrorMessage).Build();
				message.Components = null;
			});
			return;
		}
		
		var newOrder = await orderService.GetOrderAsync(message.Id);

		await UpdateOrderMessage(newOrder!, (IUserMessage)message);

		await component.UpdateAsync(
			message =>
			{
				message.Content = $"{position.ItemName} wurde entfernt!";
				message.Components = null;
			}
		);
	}
}