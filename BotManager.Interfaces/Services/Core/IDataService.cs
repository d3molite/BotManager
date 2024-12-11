namespace BotManager.Interfaces.Services.Core;

public interface IDataService<T>
{
	public event EventHandler? ItemsUpdated;
	
	public IEnumerable<T> Items { get; set; }

	public void Load();
	
	public Task LoadAsync();

	public bool Save();
}