namespace BotManager.Bot.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CommandExecutorAttribute(string commandName) : Attribute
{
	public string CommandName { get; } = commandName;
}