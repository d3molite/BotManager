
using BotManager.Db.Models.Modules.Order;
using EfExtensions.Core.Interfaces.Repository;

namespace BotManager.Db.Interfaces;

public interface IOrderItemRepository : IBaseKeyedRepository<OrderItem, string>;