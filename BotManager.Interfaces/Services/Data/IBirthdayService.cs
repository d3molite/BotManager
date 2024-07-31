using BotManager.Db.Models.Modules.Birthdays;

namespace BotManager.Interfaces.Services.Data;

public interface IBirthdayService
{
	public Task<IEnumerable<Birthday>> GetBirthdays(string configId, ulong guildId);

	public Task<bool> Upsert(string configId, ulong guildId, ulong userId, DateOnly date);

	public Task<bool> Delete(string configId, ulong guildId, ulong userId);
}