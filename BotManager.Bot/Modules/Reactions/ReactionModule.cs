using BotManager.Bot.Interfaces.Modules;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Reactions;

public class ReactionModule(DiscordSocketClient client) : IUtilityModule
{
	public async Task RegisterModuleAsync()
	{
		client.MessageReceived += ReactToMessage;
	}

	private Task ReactToMessage(SocketMessage arg)
	{
		throw new NotImplementedException();
	}
}