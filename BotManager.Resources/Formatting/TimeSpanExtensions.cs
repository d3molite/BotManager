using BotManager.Resources.Manager;

namespace BotManager.Resources.Formatting;

public static class TimeSpanExtensions
{
	public static string ToHumanFriendlyString(this TimeSpan timeSpan, string locale = "")
	{
		var roundedAge = Math.Round(timeSpan.TotalDays, 2);

		return roundedAge switch
		{
			// if the time is smaller than one day.
			< 1 => $"{Math.Round(timeSpan.TotalHours, 2)} {ResourceResolver.GetString(x => Units.Hours, locale)}",

			// if the time is smaller than a year.
			< 365 => $"{roundedAge} {ResourceResolver.GetString(x => Units.Days, locale)}",

			// if the time is greater than a year.
			var _ => $"{Math.Round(timeSpan.TotalDays / 365, 2)} {ResourceResolver.GetString(x => Units.Years, locale)}",
		};
	}
}