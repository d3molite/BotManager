using BotManager.Bot.Core;
using BotManager.Bot.Interfaces;
using BotManager.Bot.Interfaces.Core;
using BotManager.Services.Interfaces;
using BotManager.Services.Interfaces.Bot;
using BotManager.Services.Interfaces.Data;
using Serilog;

namespace BotManager.Services.Implementation.Bot;

public class BotService(IBotConfigService botConfigService) : IBotService
{
	public List<IBotEntity> Bots { get; set; } = new ();

	public async Task Initialize()
	{
		Log.Debug("Found {BotCount} Bots", botConfigService.Items.Count());
		
		foreach (var config in botConfigService.Items)
		{
			var botEntity = new BotEntity(config);
			Bots.Add(botEntity);
		}

		#if DEBUG
		
		foreach (var bot in Bots.Where(x => x.Debug))
		{
			await bot.StartAsync();
		}
		
		#elif RELEASE
		
		foreach (var bot in Bots.Where(x => x.AutoStart))
		{
			await bot.StartAsync();
		}
		
		#endif
	}
}