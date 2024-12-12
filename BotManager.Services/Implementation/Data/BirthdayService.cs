using BotManager.Db.Models.Modules.Birthdays;
using BotManager.Interfaces.Services.Data;
using Demolite.Db.Enum;
using Demolite.Db.Interfaces;

namespace BotManager.Services.Implementation.Data;

public class BirthdayService(IDbRepository<Birthday> repository) : IBirthdayService
{
	public async Task<IEnumerable<Birthday>> GetBirthdays(string configId, ulong guildId)
	{
		return await repository.GetAllCustomAsync(x => x.BirthdayConfigId == configId && x.GuildId == guildId);
	}

	public async Task<bool> Upsert(string configId, ulong guildId, ulong userId, DateOnly date)
	{
		var existing = await repository.GetCustomAsync(x => x.BirthdayConfigId == configId && x.GuildId == guildId && x.UserId == userId);

		if (existing is null)
		{
			var insert = new Birthday()
			{
				BirthdayConfigId = configId,
				UserId = userId,
				GuildId = guildId,
				Date = date,
				OperationType = Operation.Created,
			};

			return (await repository.CrudAsync(insert)).Success;
		}

		existing.Date = date;
		existing.OperationType = Operation.Updated;
		
		return (await repository.CrudAsync(existing)).Success;
	}

	public async Task<bool> Delete(string configId, ulong guildId, ulong userId)
	{
		var existing = await repository.GetCustomAsync(x => x.BirthdayConfigId == configId && x.GuildId == guildId && x.UserId == userId);

		if (existing is null)
			return false;

		existing.OperationType = Operation.Removed;

		return (await repository.CrudAsync(existing)).Success;
	}
}