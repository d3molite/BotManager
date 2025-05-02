using BotManager.Bot.Attributes;
using BotManager.Bot.Modules.Definitions;
using BotManager.ImageProcessing.Commands;
using BotManager.ImageProcessing.Enum;
using BotManager.ImageProcessing.Model;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Image;

public partial class ImageModule
{
    [CommandExecutor(Commands.Waaw)]
    [CommandExecutor(Commands.Woow)]
    [CommandExecutor(Commands.Haah)]
    [CommandExecutor(Commands.Hooh)]
    public async Task ExecuteMirrorCommand(SocketSlashCommand command)
    {
        var result = await DeferAndTryGet(command);

        if (result is not null)
        {
            var file = result.ImageModel!;
            
            var direction = command.CommandName switch
            {
                Commands.Waaw => MirrorDirection.LeftToRight,
                Commands.Woow => MirrorDirection.RightToLeft,
                Commands.Haah => MirrorDirection.TopToBottom,
                Commands.Hooh => MirrorDirection.BottomToTop,
                _ => MirrorDirection.LeftToRight
            };
            
            await MirrorCommands.ExecuteMirror(file, direction);
            await SendAndDelete(file, command);
        }
    }
    
    private async Task<ImageResult?> DeferAndTryGet(SocketInteraction command)
    {
        await command.DeferAsync();
        var file = await TryGetImage(command);
        
        return file is { Success: true, ImageModel: not null } ? file : null;
    }
}