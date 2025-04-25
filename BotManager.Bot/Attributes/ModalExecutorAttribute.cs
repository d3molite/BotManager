namespace BotManager.Bot.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ModalExecutorAttribute(string modalName) : Attribute
{
	public string ModalName { get; } = modalName;
}