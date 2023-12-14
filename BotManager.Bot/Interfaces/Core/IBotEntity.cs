namespace BotManager.Bot.Interfaces.Core;

public interface IBotEntity
{
	public string Id { get; }
	
	public string Name { get; }
	
	public bool AutoStart { get; }
	
	public bool Debug { get; }

	public Task StartAsync();

	public Task StopAsync();

	public Task RestartAsync();
}