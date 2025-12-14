using BotManager.Metadata.Attributes;
using Discord;
using JetBrains.Annotations;

namespace BotManager.Metadata.Constants;

[UsedImplicitly]
public class Buttons
{
	protected Buttons()
	{
		
	}
	
	[Button(ButtonStyle.Success)]
	public static readonly string WatchPartyJoinBuild = WatchPartyJoin;
	
	public const string WatchPartyJoin = nameof(WatchPartyJoin);
	
	[Button(ButtonStyle.Secondary)]
	public static readonly string WatchPartyInterestedBuild = WatchPartyInterested;
	
	public const string WatchPartyInterested = nameof(WatchPartyInterested);
	
	[Button(ButtonStyle.Secondary)]
	public static readonly string WatchPartyDontWaitBuild = WatchPartyDontWait;
	
	public const string WatchPartyDontWait = nameof(WatchPartyDontWait);
	
	[Button(ButtonStyle.Danger)]
	public static readonly string WatchPartyNotInterestedBuild = WatchPartyNotInterested;
	
	public const string WatchPartyNotInterested = nameof(WatchPartyNotInterested);
}