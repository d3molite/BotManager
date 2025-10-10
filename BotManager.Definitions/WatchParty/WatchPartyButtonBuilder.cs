using BotManager.Metadata.Builder;
using BotManager.Metadata.Constants;
using Discord;

namespace BotManager.Metadata.WatchParty;

public static class WatchPartyButtonBuilder
{
	public static MessageComponent Build(string locale)
	{
		var builder = new ComponentBuilder();
		
		builder.AddButton<Buttons>(_ => Buttons.WatchPartyJoin, locale);
		builder.AddButton<Buttons>(_ => Buttons.WatchPartyInterested, locale);
		builder.AddButton<Buttons>(_ => Buttons.WatchPartyDontWait, locale);
		builder.AddButton<Buttons>(_ => Buttons.WatchPartyNotInterested, locale);
		
		return builder.Build();
	}
}