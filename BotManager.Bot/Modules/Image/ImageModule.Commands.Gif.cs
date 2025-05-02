using BotManager.Bot.Attributes;
using BotManager.Bot.Modules.Definitions;
using BotManager.ImageProcessing.Commands;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Image;

public partial class ImageModule
{
	[CommandExecutor(Commands.Reverse)]
	public async Task ExecuteReverseCommand(SocketSlashCommand command)
	{
		var result = await DeferAndTryGet(command);

		if (result is not null)
		{
			var file = result.ImageModel!;

			if (!file.IsAnimated)
			{
				await command.FollowupAsync("The image must be animated to use this command!", ephemeral: true);
				return;
			}

			await GifCommands.ExecuteReverse(file);
			await SendAndDelete(file, command);
		}
	}

	[CommandExecutor(Commands.Gifspeed)]
	public async Task ExecuteDelayCommand(SocketSlashCommand command)
	{
		var result = await DeferAndTryGet(command);

		if (result is not null)
		{
			var file = result.ImageModel!;

			if (!file.IsAnimated)
			{
				await command.FollowupAsync("The image must be animated to use this command!", ephemeral: true);
				return;
			}

			var delay = (double)command.Data.Options.First().Value;

			await GifCommands.ExecuteSpeed(file, delay);
			await SendAndDelete(file, command);
		}
	}
}