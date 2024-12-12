using System.ComponentModel.DataAnnotations;
using Demolite.Db.Models;

namespace BotManager.Db.Models.Abstract;

public class AbstractGuildConfig : AbstractDbItem
{
	[MaxLength(40)]
	public string GuildConfigId { get; set; } = null!;

	public virtual GuildConfig GuildConfig { get; set; } = null!;
}