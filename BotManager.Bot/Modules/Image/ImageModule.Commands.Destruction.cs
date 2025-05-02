using BotManager.Bot.Attributes;
using BotManager.Bot.Modules.Definitions;
using BotManager.ImageProcessing.Commands;
using Discord.WebSocket;
// ReSharper disable MemberCanBePrivate.Global

namespace BotManager.Bot.Modules.Image;

public partial class ImageModule
{
    [CommandExecutor(Commands.NeedsMoreJpeg)]
    public async Task ExecuteMoreJpegCommand(SocketSlashCommand command)
    {
        var result = await DeferAndTryGet(command);

        if (result is not null)
        {
            var file = result.ImageModel!;
            
            await DestructionCommands.ExecuteJpeg(file);
            await SendAndDelete(file, command);
        }
    }
}