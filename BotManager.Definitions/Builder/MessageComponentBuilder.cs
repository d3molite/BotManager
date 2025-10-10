using System.Linq.Expressions;
using System.Reflection;
using BotManager.Metadata.Attributes;
using BotManager.Metadata.Helpers;
using BotManager.Resources;
using BotManager.Resources.Manager;
using Discord;

namespace BotManager.Metadata.Builder;

public static class MessageComponentBuilder
{
	public static void AddButton<T>(this ComponentBuilder builder, Expression<Func<T, object>> selector, string locale)
	{
		var objectInfo = ExpressionHelper.GetMemberInfo(selector);
		var objectKey = objectInfo.GetString();

		if (objectKey is null)
			return;

		var attribute = objectInfo.GetCustomAttribute<ButtonAttribute>();
		var objectResource = ResourceResolver.GetString(typeof(ButtonResource), objectKey, locale);

		builder.WithButton(label: objectResource, customId: objectKey, style: attribute?.Style ?? ButtonStyle.Primary);
	}
}