using BotManager.Bot.Interfaces.Modules;
using BotManager.Bot.Services.Register;
using BotManager.Db.Models;
using BotManager.Db.Models.Modules.AntiSpam;
using Discord;
using Discord.WebSocket;
using Serilog;

namespace BotManager.Bot.Modules.AntiSpam;

public class AntiSpamModule(DiscordSocketClient client, GuildConfig config) : IUtilityModule
{
	private readonly List<SpamHandler> _spamHandlers = [];

	private Dictionary<IUser, MessageQueue> _userMessages = [];
	
	private readonly PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromMinutes(4));

	private AntiSpamConfig Config => config.AntiSpamConfig!;

	public Task RegisterModuleAsync()
	{
		client.MessageReceived += CheckForSpam;
		Task.Run(async () => await CleanupQueues());
		return Task.CompletedTask;
	}

	private async Task CleanupQueues()
	{
		while (await _timer.WaitForNextTickAsync())
		{
			foreach (var kvp in _userMessages)
			{
				kvp.Value.DequeueOldItems();
			}
		}
	}

	private Task CheckForSpam(SocketMessage message)
	{
		if (IgnoreMessage(message))
			return Task.CompletedTask;

		// check for an existing spam cleaner that is currently running
		var existingHandler = _spamHandlers.FirstOrDefault(x => x.User == message.Author);

		if (existingHandler != null)
		{
			AddToRunning(existingHandler, message);
			return Task.CompletedTask;
		}

		EnqueueMessage(message);
		return Task.CompletedTask;
	}

	private static void AddToRunning(SpamHandler existingHandler, SocketMessage message)
		=> existingHandler.MessageQueue.ForceEnqueue(message);

	private void EnqueueMessage(SocketMessage message)
	{
		if (_userMessages.TryGetValue(message.Author, out var messageQueue))
		{
			messageQueue.Enqueue(message);
			CheckQueueForSpam(messageQueue, message.Author);
			return;
		}

		var queue = new MessageQueue(10);
		queue.Enqueue(message);
		_userMessages.Add(message.Author, queue);
	}

	private bool IgnoreMessage(SocketMessage message)
	{
		if (string.IsNullOrWhiteSpace(message.Content) || message.Author.Id == client.CurrentUser.Id)
			return true;

		if (message.Channel is not SocketGuildChannel channel)
			return true;

		if (channel.Guild.Id != config.GuildId)
			return true;

		try
		{
			var prefixes = Config
							.IgnorePrefixes.Split(',')
							.Select(x => x.Trim())
							.Where(x => !string.IsNullOrEmpty(x));
			
			if (prefixes.Any(x => message.Content.StartsWith(x)))
				return true;
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Could not split prefixes for Config {ConfigId}", Config.Id);
		}

		return false;
	}

	private void CheckQueueForSpam(MessageQueue queue, IUser user)
	{
		if (!ContainsSpam(queue))
			return;

		var module = ModuleRegister.TryGetLogger(client.CurrentUser.Id, config.GuildId);
		
		var handler = new SpamHandler(module)
		{
			User = (SocketGuildUser)user,
			Client = client,
			MessageQueue = queue,
		};

		handler.SpamDeleted += Cleanup;
		_spamHandlers.Add(handler);
	}

	private void Cleanup(object? sender, EventArgs e)
	{
		if (sender is SpamHandler handler)
			_spamHandlers.Remove(handler);
	}

	private static bool ContainsSpam(MessageQueue queue)
	{
		return queue
				.Queue.GroupBy(message => message.Content)
				.Any(group => group.Count() > 5);
	}
}