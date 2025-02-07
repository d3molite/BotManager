using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Demolite.Db.Models;

namespace BotManager.Db.Models.Modules.Reactions;

public class ReactionItem : AbstractDbItem
{
	[MaxLength(40)]
	public string ReactionConfigId { get; set; } = null!;

	public virtual ReactionConfig ReactionConfig { get; set; } = null!;

	[MaxLength(255)]
	public required string ReactionTrigger { get; set; }

	[MaxLength(255)]
	public string? ReactionMessage { get; set; }

	[MaxLength(40)]
	public string? ReactionEmoji { get; set; }

	public int ReactionChance { get; set; }

	[NotMapped]
	public bool EmojiOnly => string.IsNullOrEmpty(ReactionMessage);
}