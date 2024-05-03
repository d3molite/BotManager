using BotManager.ImageProcessing.Commands;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Image;

public partial class ImageModule
{
    private async Task ExecuteReverseCommand(SocketSlashCommand command)
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
}