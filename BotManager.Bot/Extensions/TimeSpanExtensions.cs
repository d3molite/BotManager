namespace BotManager.Bot.Extensions;

public static class TimeSpanExtensions
{
	public static string ToHumanFriendlyString(this TimeSpan timeSpan)
	{
		var roundedAge = Math.Round(timeSpan.TotalDays, 2);

		return roundedAge switch
		{
			// if the time is smaller than one day.
			< 1 => Math.Round(timeSpan.TotalHours, 2) + " hours",

			// if the time is smaller than a year.
			< 365 => roundedAge + " days",

			// if the time is greater than a year.
			var _ => Math.Round(timeSpan.TotalDays / 365, 2) + " years",
		};
	}
}