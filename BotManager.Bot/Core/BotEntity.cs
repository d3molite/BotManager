using BotManager.Bot.Services;
using BotManager.Bot.Services.Commands;
using BotManager.Bot.Services.Utilities;
using BotManager.Db.Models;
using BotManager.Interfaces.Core;
using Discord;
using Discord.WebSocket;
using Serilog;

namespace BotManager.Bot.Core;

public class BotEntity : IBotEntity
{
	private readonly DiscordSocketClient _client;
	private readonly CancellationTokenSource _tokenSource = new();
	private readonly BotConfig _config;

	private CommandModuleService CommandModuleService { get; }
	private UtilityModuleService UtilityModuleService { get; }
	

	public BotEntity(BotConfig config)
	{
		_config = config;

		_client = new DiscordSocketClient(
			new DiscordSocketConfig()
			{
				GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers | GatewayIntents.MessageContent,
				MessageCacheSize = 200,
				AlwaysDownloadUsers = true,
				#if DEBUG
				UseInteractionSnowflakeDate = false,
				#endif
			}
		);

		CommandModuleService = new CommandModuleService(_config, _client);
		UtilityModuleService = new UtilityModuleService(_config, _client);
		
		_client.Log += LogClientEvent;
		_client.Ready += OnClientReady;
		_client.SlashCommandExecuted += CommandModuleService.ExecuteCommand;
		_client.ModalSubmitted += CommandModuleService.ExecuteModalResponse;
		_client.ButtonExecuted += CommandModuleService.ExecuteComponentResponse;
		_client.SelectMenuExecuted += CommandModuleService.ExecuteSelectResponse;
	}

	private async Task OnClientReady()
	{
		Task.Run(async () => await CommandModuleService.BuildCommands());
		Task.Run(async () => await UtilityModuleService.InitializeAsync());
	}

	private async Task LogClientEvent(LogMessage arg)
	{
		switch (arg.Severity)
		{
			case LogSeverity.Critical:
			case LogSeverity.Error:
				Log.Error(arg.Exception, "{BotName}: {LogMessage}", Name, arg.Message);
				break;
			
			case LogSeverity.Warning:
				Log.Error(arg.Exception, "{BotName}: {LogMessage}", Name, arg.Message);
				break;
			
			case LogSeverity.Info:
			case LogSeverity.Verbose:
			case LogSeverity.Debug:
				Log.Debug(arg.Exception, "{BotName}: {LogMessage}", Name, arg.Message);
				break;
				
			default:
				Log.Debug("{BotName}: {LogMessage}", Name, arg.Message);
				break;
		}
	}

	public string Id => _config.Id;

	public string Name => _config.Name;

	public bool AutoStart => _config.Active;

	public bool Debug => _config.Debug;

	public async Task StartAsync()
	{
		await _client.LoginAsync(TokenType.Bot, _config.Token);
		await _client.StartAsync();

		Log.Debug("Logged In {BotName}", Name);

		await _client.SetActivityAsync(new Game(_config.Presence));

		while (!_tokenSource.Token.IsCancellationRequested)
		{
			await Task.Delay(1);
		}
			
		await _client.StopAsync();
		await _client.LogoutAsync();
	}

	public async Task StopAsync()
	{
		await _tokenSource.CancelAsync();
	}

	public async Task RestartAsync()
	{
		await _tokenSource.CancelAsync();
		await StartAsync();
	}
}