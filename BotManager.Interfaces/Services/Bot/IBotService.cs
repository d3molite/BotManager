using BotManager.Interfaces.Core;

namespace BotManager.Interfaces.Services.Bot;

public interface IBotService
{
	public List<IBotEntity> Bots { get; set; }

	public Task Initialize();
}