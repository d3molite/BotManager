using BotManager.Db.Models.Abstract;

namespace BotManager.Db.Models.Modules.Feedback;

public class FeedbackConfig : AbstractGuildConfig
{
	public ulong FeedbackChannelId { get; set; }
	
	public bool AddReactions { get; set; }
}