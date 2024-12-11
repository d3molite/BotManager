using System.ComponentModel.DataAnnotations;
using Demolite.Db.Models;

namespace BotManager.Db.Models.Modules.Image;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class ImageConfig : AbstractDbItem
{
	[MaxLength(40)]
	public string GuildConfigId { get; set; } = null!;
	public virtual GuildConfig GuildConfig { get; set; } = null!;
}