using BotManager.Db.Models.Abstract;
using Demolite.Db.Models;

namespace BotManager.Db.Models.Modules.Reactions;

public class ReactionConfig : AbstractGuildConfig
{
	public List<ReactionItem> Reactions { get; set; }
}