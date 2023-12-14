using BotManager.Db.Models;
using EfExtensions.Core.Interfaces.Repository;

namespace BotManager.Db.Interfaces;

public interface IBotConfigRepository : IBaseKeyedRepository<BotConfig, string>;