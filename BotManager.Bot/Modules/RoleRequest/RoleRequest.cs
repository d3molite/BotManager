namespace BotManager.Bot.Modules.RoleRequest;

public class RoleRequest
{
	public RoleRequestStatus Status { get; set; } = RoleRequestStatus.Open;
	
	public required string Email { get; set; }
	
	public ulong UserId { get; set; }

	public string UserNick { get; set; } = "";

	public ulong GuildId { get; set; }
}