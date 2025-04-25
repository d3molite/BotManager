using System.Reflection;
using BotManager.Bot.Attributes;

namespace BotManager.Bot.Extensions;

public static class ModalLocatorExtension
{
	/// <summary>
	/// Tries to fetch a task with a matching <see cref="ModalExecutorAttribute"/> from a specific type.
	/// </summary>
	/// <param name="type">The class type to search in.</param>
	/// <param name="modalName">The modal name to look for.</param>
	/// <returns>A MethodInfo object for the specified modal, or null if none was found.</returns>
	public static MethodInfo? GetModalExecutor(this Type type, string modalName)
	{
		var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);

		var matchingTask = methods.FirstOrDefault(method => HasMatchingModalName(method, modalName));

		return matchingTask;
	}

	private static bool HasMatchingModalName(MethodInfo methodInfo, string modalName)
		=> methodInfo.GetCustomAttribute<ModalExecutorAttribute>()?.ModalName == modalName;
}