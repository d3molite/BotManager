using System.ComponentModel.DataAnnotations;
using BotManager.Db.Models.Abstract;

namespace BotManager.Db.Models.Modules.AntiSpam;

public class AntiSpamConfig : AbstractGuildConfig
{
	[MaxLength(255)]
	public string IgnorePrefixes { get; set; } = "";
}