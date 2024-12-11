using System.ComponentModel.DataAnnotations;
using Demolite.Db.Models;

namespace BotManager.Db.Models.Modules.Voice;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class VoiceChannelConfig : AbstractDbItem
{
    public ulong CommandChannelId { get; set; }
    
    public ulong VoiceCategoryId { get; set; }

    [MaxLength(40)]
    public string GuildConfigId { get; set; }

    public virtual GuildConfig GuildConfig { get; set; } = null!;
}