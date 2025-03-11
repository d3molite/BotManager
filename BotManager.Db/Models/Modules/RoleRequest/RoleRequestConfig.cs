using BotManager.Db.Models.Abstract;

namespace BotManager.Db.Models.Modules.RoleRequest;

public class RoleRequestConfig : AbstractGuildConfig
{
	public ulong RoleId { get; set; }
	
	public ulong ReceiverId { get; set; }
}