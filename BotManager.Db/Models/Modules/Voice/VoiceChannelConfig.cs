using BotManager.Db.Models.Abstract;

namespace BotManager.Db.Models.Modules.Voice;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class VoiceChannelConfig : AbstractGuildConfig
{
	public ulong CommandChannelId { get; set; }

	public ulong VoiceCategoryId { get; set; }
}