using BotManager.Bot.Interfaces.Services;
using BotManager.Db.Models.Modules.Image;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Image;

public partial class ImageModule(DiscordSocketClient client) : ICommandModule<ImageConfig>
{
    public Task ExecuteButton(SocketMessageComponent component)
    {
        throw new NotImplementedException();
    }

    public Task ExecuteModal(SocketModal modal)
    {
        throw new NotImplementedException();
    }

    public Task ExecuteSelect(SocketMessageComponent component)
    {
        throw new NotImplementedException();
    }
}