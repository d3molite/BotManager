using BotManager.Db.Interfaces;
using BotManager.Db.Models;
using BotManager.Services.Interfaces;
using BotManager.Services.Interfaces.Data;

namespace BotManager.Services.Implementation.Data;

public class BotConfigService : IBotConfigService
{
	private readonly IBotConfigRepository _botConfigRepository;
	
	public event EventHandler? ItemsUpdated;

	public IEnumerable<BotConfig> Items { get; set; } = new List<BotConfig>();

	public BotConfigService(IBotConfigRepository botConfigRepository)
	{
		_botConfigRepository = botConfigRepository;
		Load();
	}

	public void Load()
	{
		Items = _botConfigRepository.GetAll();
	}

	public bool Save() => throw new NotImplementedException();
}