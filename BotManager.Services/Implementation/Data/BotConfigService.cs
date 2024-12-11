using BotManager.Db.Models;
using BotManager.Interfaces.Services.Data;
using Demolite.Db.Interfaces;

namespace BotManager.Services.Implementation.Data;

public class BotConfigService : IBotConfigService
{
	private readonly IDbRepository<BotConfig> _botConfigRepository;
	
	public event EventHandler? ItemsUpdated;

	public IEnumerable<BotConfig> Items { get; set; } = new List<BotConfig>();

	public BotConfigService(IDbRepository<BotConfig> botConfigRepository)
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