namespace BotManager.Bot.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CommandBuilderAttribute(string commandName) : Attribute
{
	public string CommandName { get; } = commandName;
}