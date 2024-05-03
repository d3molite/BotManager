using EfExtensions.Items.Model;

namespace BotManager.Db.Models.Modules.Voice;

public class VoiceChannelConfig : DbItem<string>
{
    public ulong CommandChannelId { get; set; }
    
    public ulong VoiceCategoryId { get; set; }
}