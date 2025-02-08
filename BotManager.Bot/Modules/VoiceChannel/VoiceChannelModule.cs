using BotManager.Bot.Interfaces.Modules;
using BotManager.Bot.Modules.Logging;
using BotManager.Bot.Services.Register;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.Voice;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.VoiceChannel;

public partial class VoiceChannelModule : ICommandModule<VoiceChannelConfig>
{
	private readonly DiscordSocketClient _client;
	private readonly GuildConfig _guildConfig;
	private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(1));
	private readonly LoggingModule _loggingModule;

	private string Locale => _guildConfig.GuildLocale;

	private VoiceChannelConfig Config => _guildConfig.VoiceChannelConfig!;

	private List<VoiceState> CurrentChannels { get; set; } = [];

	public VoiceChannelModule(DiscordSocketClient client, GuildConfig guildConfig)
	{
		_client = client;
		_guildConfig = guildConfig;

		_loggingModule = (LoggingModule)ModuleRegister.TryGetLogger(client.CurrentUser.Id, guildConfig.GuildId)!;

		Task.Run(async () => await PollChannels());
	}

	public Task ExecuteButton(SocketMessageComponent component)
		=> throw new NotImplementedException();

	public Task ExecuteSelect(SocketMessageComponent component)
		=> throw new NotImplementedException();
}