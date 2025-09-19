namespace BotManager.Core.Models;

public class BotInfo
{
	public string Name { get; set; } = string.Empty;
	
	public string ImageUri { get; set; } = string.Empty;

	public bool Online { get; set; }

	public List<string> Modules { get; set; } = [];
}