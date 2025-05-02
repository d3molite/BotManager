namespace BotManager.Bot.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class CommandExecutorAttribute(string commandName) : Attribute
{
	public string CommandName { get; } = commandName;
}