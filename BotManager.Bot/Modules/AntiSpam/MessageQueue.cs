using System.Text;
using Discord;
using Discord.WebSocket;
using Serilog;

namespace BotManager.Bot.Modules.AntiSpam;

public class MessageQueue(int maxMessages)
{
	private TimeSpan LifeTimeMinutes => TimeSpan.FromMinutes(5);
	
	public List<IMessage> Queue { get; } = [];

	public bool IsEmpty => !Queue.Any();

	/// <summary>
	///     Enqueue a message into the queue.
	///     After <see cref="_maxMessages" /> has been reached, the oldest message will be deleted.
	/// </summary>
	/// <param name="message">Message to enqueue</param>
	public void Enqueue(IMessage message)
	{
		if (Queue.Count >= maxMessages) 
			Queue.RemoveAt(0);
		Queue.Add(message);
	}

	public IMessage Dequeue()
	{
		var item = Queue[0];
		Queue.RemoveAt(0);
		return item;
	}

	/// <summary>
	///     Forces a message into the queue, regardless of the queue size.
	/// </summary>
	/// <param name="message">Message to enqueue</param>
	public void ForceEnqueue(IMessage message)
	{
		if (!Queue.Contains(message))
			Queue.Add(message);
	}

	/// <summary>
	///     Clears the message queue.
	/// </summary>
	public void Clear()
	{
		Queue.Clear();
	}

	/// <summary>
	///     Method which removes old items from the queue.
	/// </summary>
	public void DequeueOldItems()
	{
		try
		{
			var oldMessages = Queue.Where(message => DateTime.Now - message.Timestamp > LifeTimeMinutes);

			foreach (var message in oldMessages) Queue.Remove(message);
		}
		catch (InvalidOperationException ex)
		{
			Log.Error("Error while removing from queue: {Exception}", ex.Message);
		}
	}

	/// <summary>
	///     Groups all queued messages by their channel
	/// </summary>
	/// <returns>A dictionary of grouped messages.</returns>
	public Dictionary<SocketTextChannel, List<IMessage>> GetGroupedByChannels()
	{
		return Queue
				.GroupBy(x => (SocketTextChannel)x.Channel)
				.ToDictionary(g => g.Key, g => g.ToList());
	}

	/// <summary>
	///     Lists all messages in the queue.
	/// </summary>
	/// <returns>A list of all queued messages</returns>
	public override string ToString()
	{
		var sb = new StringBuilder();

		foreach (var message in Queue) sb.AppendLine($"{message.Author} at {message.Timestamp}: {message.Content}");

		return sb.ToString();
	}
}