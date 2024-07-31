using BotManager.Db.Models.Modules.Birthdays;
using EfExtensions.Core.Interfaces.Repository;

namespace BotManager.Db.Interfaces;

public interface IBirthdayRepository : IBaseKeyedRepository<Birthday, string>
{
	
}