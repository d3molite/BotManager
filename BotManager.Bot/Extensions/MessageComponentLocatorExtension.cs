using System.Reflection;
using BotManager.Bot.Attributes;

namespace BotManager.Bot.Extensions;

public static class MessageComponentLocatorExtension
{
	/// <summary>
	/// Tries to fetch a task with a matching <see cref="MessageComponentExecutorAttribute"/> from a specific type.
	/// </summary>
	/// <param name="type">The class type to search in.</param>
	/// <param name="componentName">The component name to look for.</param>
	/// <returns>A MethodInfo object for the specified modal, or null if none was found.</returns>
	public static MethodInfo? GetComponentExecutor(this Type type, string componentName)
	{
		var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

		var matchingTask = methods.FirstOrDefault(method => HasMatchingComponentName(method, componentName));

		return matchingTask;
	}

	private static bool HasMatchingComponentName(MethodInfo methodInfo, string componentName)
		=> methodInfo.GetCustomAttribute<MessageComponentExecutorAttribute>()?.ComponentName == componentName;
}