using BotManager.Db.Interfaces;
using BotManager.Db.Models.Modules.Birthdays;
using BotManager.Interfaces.Services.Data;
using EfExtensions.Core.Enum;

namespace BotManager.Services.Implementation.Data;

public class BirthdayService(IBirthdayRepository repository) : IBirthdayService
{
	public async Task<IEnumerable<Birthday>> GetBirthdays(string configId, ulong guildId)
	{
		return await repository.GetAllCustomAsync(x => x.ConfigId == configId && x.GuildId == guildId);
	}

	public async Task<bool> Upsert(string configId, ulong guildId, ulong userId, DateOnly date)
	{
		var existing = await repository.GetCustomAsync(x => x.ConfigId == configId && x.GuildId == guildId && x.UserId == userId);

		if (existing is null)
		{
			var insert = new Birthday()
			{
				ConfigId = configId,
				UserId = userId,
				GuildId = guildId,
				Date = date,
				OperationType = Operation.Created
			};

			return (await repository.CrudAsync(insert)).Success;
		}

		existing.Date = date;
		existing.OperationType = Operation.Updated;
		
		return (await repository.CrudAsync(existing)).Success;
	}

	public async Task<bool> Delete(string configId, ulong guildId, ulong userId)
	{
		var existing = await repository.GetCustomAsync(x => x.ConfigId == configId && x.GuildId == guildId && x.UserId == userId);

		if (existing is null)
			return false;

		existing.OperationType = Operation.Removed;

		return (await repository.CrudAsync(existing)).Success;
	}
}