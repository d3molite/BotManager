using BotManager.Bot.Interfaces.Modules;
using BotManager.Bot.Modules.Logging;
using Discord;
using Discord.WebSocket;
using Serilog;

namespace BotManager.Bot.Modules.AntiSpam;

public class SpamHandler
{
	private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(2));
	
	private readonly Dictionary<string, List<string>> _deletedMessages = new();

	public event EventHandler? SpamDeleted;

	private readonly LoggingModule? _loggingModule;

	public required IGuildUser User { get; set; }

	public required DiscordSocketClient Client { get; set; }
	
	public required MessageQueue MessageQueue { get; set; }

	public SpamHandler(IUtilityModule? loggingModule)
	{
		if (loggingModule is LoggingModule actualLogger)
			_loggingModule = actualLogger;
		
		Task.Run(async () => await TimerTask());
	}

	private async Task TimerTask()
	{
		// Wait a few seconds before deleting the spam.
		await _timer.WaitForNextTickAsync();

		await DeleteSpam();
	}

	private async Task DeleteSpam()
	{
		await TimeoutUser();

		while (!MessageQueue.IsEmpty)
		{
			var nextMessage = MessageQueue.Dequeue();

			var channel = nextMessage.Channel;
			await channel.DeleteMessageAsync(nextMessage);
			
			if (_deletedMessages.TryGetValue(channel.Name, out var messages))
				messages.Add(nextMessage.Content);
			else
				_deletedMessages.Add(channel.Name, [nextMessage.Content]);
			
			Thread.Sleep(200);
		}

		await SendLog();
		SpamDeleted?.Invoke(this, EventArgs.Empty);
	}

	private async Task SendLog()
	{
		if (_loggingModule is not null)
			await _loggingModule.LogUserTimedOut(User);
	}

	private async Task TimeoutUser()
	{
		try
		{
			await User.SetTimeOutAsync(TimeSpan.FromDays(3), new RequestOptions
			{
				AuditLogReason = "Spam detected by {BotName}"
			});
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Could not time out user {User}", User);
		}
	}
}