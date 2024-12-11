using BotManager.Bot.Core;
using BotManager.Interfaces.Core;
using BotManager.Interfaces.Services.Bot;
using BotManager.Interfaces.Services.Data;
using Serilog;

namespace BotManager.Services.Implementation.Bot;

public class BotService(IBotConfigService botConfigService) : IBotService
{
	public List<IBotEntity> Bots { get; set; } = new ();

	public async Task Initialize()
	{
		await botConfigService.LoadAsync();
		Log.Debug("Found {BotCount} Bots", botConfigService.Items.Count());
		
		foreach (var config in botConfigService.Items)
		{
			var botEntity = new BotEntity(config);
			Bots.Add(botEntity);
		}

		#if DEBUG
		
		foreach (var bot in Bots.Where(x => x.Debug))
		{
			Task.Run(async () => await bot.StartAsync());
		}
		
		#elif RELEASE
		
		foreach (var bot in Bots.Where(x => x.AutoStart))
		{
			Task.Run(async () => await bot.StartAsync());
		}
		
		#endif
	}
}