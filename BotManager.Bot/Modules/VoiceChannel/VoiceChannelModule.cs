using BotManager.Bot.Interfaces.Modules;
using BotManager.Bot.Modules.Core;
using BotManager.Bot.Modules.Logging;
using BotManager.Bot.Services.Register;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.Voice;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.VoiceChannel;

public partial class VoiceChannelModule : AbstractCommandModuleBase<VoiceChannelConfig>
{
	public override string ModuleName => "Voice Channels";
	
	private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(1));
	private LoggingModule? _loggingModule;

	private List<VoiceState> CurrentChannels { get; set; } = [];

	public VoiceChannelModule(DiscordSocketClient client, GuildConfig guildConfig) : base(client, guildConfig)
	{
		Task.Run(PollChannels);
	}

	private void TryGetLogger()
	{
		_loggingModule ??= (LoggingModule)ModuleRegister.TryGetLogger(Client.CurrentUser.Id, GuildConfig.GuildId)!;
	}
}