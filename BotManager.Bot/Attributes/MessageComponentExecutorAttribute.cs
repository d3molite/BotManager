namespace BotManager.Bot.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MessageComponentExecutorAttribute(string componentName) : Attribute
{
	public string ComponentName { get; } = componentName;
}