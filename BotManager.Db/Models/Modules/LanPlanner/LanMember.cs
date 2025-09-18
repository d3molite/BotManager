using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Demolite.Db.Models;

namespace BotManager.Db.Models.Modules.LanPlanner;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class LanMember : AbstractDbItem
{
	public ulong UserId { get; set; }

	[MaxLength(32)]
	public string Nickname { get; set; } = string.Empty;

	[MaxLength(4)]
	public string SeatingGroup { get; set; } = string.Empty;

	[MaxLength(32)]
	public string SeatA { get; set; } = string.Empty;

	[MaxLength(32)]
	public string SeatB { get; set; } = string.Empty;
	
	[MaxLength(32)]
	public string SeatName { get; set; } = string.Empty;

	public int SeatingRow { get; set; } = 0;

	public int SeatingOrder { get; set; } = 0;

	[NotMapped]
	public int NumberOfSeats
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(SeatA) && !string.IsNullOrWhiteSpace(SeatB))
				return 2;

			return 1;
		}
	}

	[NotMapped]
	public int ExpertCount
	{
		get
		{
			var result = 0;

			if (SeatA.Contains("expert", StringComparison.CurrentCultureIgnoreCase))
				result++;

			if (SeatB.Contains("expert", StringComparison.CurrentCultureIgnoreCase))
				result++;

			return result;
		}
	}

	[NotMapped]
	public int BeginnerCount
	{
		get
		{
			var result = 0;

			if (SeatA.Contains("beginner", StringComparison.CurrentCultureIgnoreCase))
				result++;

			if (SeatB.Contains("beginner", StringComparison.CurrentCultureIgnoreCase))
				result++;

			return result;
		}
	}

	[NotMapped]
	public int LegendCount
	{
		get
		{
			var result = 0;

			if (SeatA.Contains("legend", StringComparison.CurrentCultureIgnoreCase))
				result++;

			if (SeatB.Contains("legend", StringComparison.CurrentCultureIgnoreCase))
				result++;

			return result;
		}
	}

	[NotMapped]
	public string SeatDescriptions
		=> $"{(!string.IsNullOrWhiteSpace(SeatA) ? SeatA : "")} {(NumberOfSeats > 1 ? "&" : "")} {(!string.IsNullOrWhiteSpace(SeatB) ? SeatB : "")}";
}