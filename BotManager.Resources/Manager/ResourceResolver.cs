using System.Globalization;
using System.Linq.Expressions;
using System.Resources;

namespace BotManager.Resources.Manager;

public static class ResourceResolver
{
	public static string GetString(Expression<Func<object, string>> resourceSelector, string locale)
	{
		if (resourceSelector.Body is not MemberExpression memberExpression) 
			return "Invalid expression.";

		var type = memberExpression.Member.DeclaringType!;
		var name = memberExpression.Member.Name;
		var manager = new ResourceManager(type);
		return manager.GetString(name, new CultureInfo(locale)) ?? $"No resource found for {name}";

	}
}