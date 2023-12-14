using System.Text;
using BotManager.Bot.Interfaces;
using BotManager.Bot.Interfaces.Core;
using BotManager.Db.Models;
using Discord;
using Discord.WebSocket;
using Serilog;

namespace BotManager.Bot.Core;

public class BotEntity : IBotEntity
{
	private readonly DiscordSocketClient _client;
	private readonly CancellationTokenSource _tokenSource = new();
	private readonly BotConfig _config;

	public BotEntity(BotConfig config)
	{
		_config = config;

		_client = new DiscordSocketClient(
			new DiscordSocketConfig()
			{
				GatewayIntents = GatewayIntents.All,
			}
		);
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