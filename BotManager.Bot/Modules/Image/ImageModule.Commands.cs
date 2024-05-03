using BotManager.Bot.Modules.Definitions;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Image;

public partial class ImageModule
{
    public async Task ExecuteCommands(SocketSlashCommand command)
    {
        switch (command.CommandName)
        {
            case Commands.Waaw:
            case Commands.Woow:
            case Commands.Haah:
            case Commands.Hooh:
                await ExecuteMirrorCommand(command);
                break;
            
            case Commands.Reverse:
                await ExecuteReverseCommand(command);
                break;
            
            case Commands.NeedsMoreJpeg:
                await ExecuteMoreJpegCommand(command);
                break;
        }
    }
}