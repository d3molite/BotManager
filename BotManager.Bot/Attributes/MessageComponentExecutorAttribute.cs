namespace BotManager.Bot.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class MessageComponentExecutorAttribute(string componentName) : Attribute
{
	public string ComponentName { get; } = componentName;
}