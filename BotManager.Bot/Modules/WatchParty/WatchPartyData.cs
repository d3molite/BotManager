using System.Linq.Expressions;
using System.Text;

namespace BotManager.Bot.Modules.WatchParty;

public class WatchPartyData
{
	public string Name { get; set; } = string.Empty;

	public string Time { get; set; } = string.Empty;

	public List<ulong> JoinedUsers { get; set; } = [];

	public List<ulong> InterestedUsers { get; set; } = [];

	public List<ulong> DontWaitUsers { get; set; } = [];

	public List<ulong> NotInterestedUsers { get; set; } = [];

	public string GetUserList(Func<WatchPartyData, List<ulong>> accessExpression)
	{
		var list = accessExpression.Invoke(this);

		if (list.Count == 0)
			return "-";

		var sb = new StringBuilder();

		foreach (var user in list)
		{
			sb.AppendLine($"<@{user}>");
		}

		return sb.ToString();
	}

	public void MoveIntoList(Func<WatchPartyData, List<ulong>> accessExpression, ulong userId)
	{
		var list = accessExpression.Invoke(this);

		if (list.Contains(userId))
			return;
		
		JoinedUsers.Remove(userId);
		InterestedUsers.Remove(userId);
		DontWaitUsers.Remove(userId);
		NotInterestedUsers.Remove(userId);
		
		list.Add(userId);
	}
}