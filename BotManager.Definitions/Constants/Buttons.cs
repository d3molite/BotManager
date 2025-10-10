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
	public const string WatchPartyJoin = nameof(WatchPartyJoin);
	
	[Button(ButtonStyle.Secondary)]
	public const string WatchPartyInterested = nameof(WatchPartyInterested);
	
	[Button(ButtonStyle.Secondary)]
	public const string WatchPartyDontWait = nameof(WatchPartyDontWait);
	
	[Button(ButtonStyle.Danger)]
	public const string WatchPartyNotInterested = nameof(WatchPartyNotInterested);
}