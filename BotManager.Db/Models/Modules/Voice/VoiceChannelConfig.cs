using Demolite.Db.Models;

namespace BotManager.Db.Models.Modules.Voice;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class VoiceChannelConfig : AbstractDbItem
{
    public ulong CommandChannelId { get; set; }
    
    public ulong VoiceCategoryId { get; set; }
}