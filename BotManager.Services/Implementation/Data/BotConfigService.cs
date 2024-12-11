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
	}

	public void Load() => throw new NotImplementedException();

	public async Task LoadAsync()
	{
		Items = await _botConfigRepository.GetAllAsync();
	}

	public bool Save() => throw new NotImplementedException();
}