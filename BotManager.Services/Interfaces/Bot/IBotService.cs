using BotManager.Bot.Interfaces;
using BotManager.Bot.Interfaces.Core;

namespace BotManager.Services.Interfaces.Bot;

public interface IBotService
{
	public List<IBotEntity> Bots { get; set; }

	public Task Initialize();
}