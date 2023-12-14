namespace BotManager.Services.Core;

public interface IDataService<T>
{
	public event EventHandler? ItemsUpdated;
	
	public IEnumerable<T> Items { get; set; }

	public void Load();

	public bool Save();
}