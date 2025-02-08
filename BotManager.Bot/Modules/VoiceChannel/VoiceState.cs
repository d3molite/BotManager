using Discord.WebSocket;

namespace BotManager.Bot.Modules.VoiceChannel;

public class VoiceState
{
	public bool UsersPresent { get; set; } = true;
	
	public ulong ChannelId { get; set; }
	
	public string ChannelName { get; set; }
	
	public int ChannelNumber { get; set; }
	
	public SocketVoiceChannel? Channel { get; set; }
}