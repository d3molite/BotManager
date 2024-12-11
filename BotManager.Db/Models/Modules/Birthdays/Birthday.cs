using System.ComponentModel.DataAnnotations;
using Demolite.Db.Models;

namespace BotManager.Db.Models.Modules.Birthdays;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class Birthday : AbstractDbItem
{
	public DateOnly Date { get; set; }
	
	public ulong GuildId { get; set; }

	public ulong UserId { get; set; }

	[MaxLength(40)]
	public string BirthdayConfigId { get; set; } = null!;
	
	public virtual BirthdayConfig BirthdayConfig { get; set; } = null!;
}