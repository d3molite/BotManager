using Discord;

namespace BotManager.Metadata.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ButtonAttribute(ButtonStyle style) : Attribute
{
	public ButtonStyle Style { get; set; } = style;
}