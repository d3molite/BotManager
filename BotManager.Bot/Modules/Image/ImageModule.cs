using BotManager.Bot.Interfaces.Modules;
using BotManager.Bot.Modules.Core;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.Image;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Image;

public partial class ImageModule(DiscordSocketClient client, GuildConfig guildConfig) : AbstractCommandModuleBase<ImageConfig>(client, guildConfig)
{
    
}