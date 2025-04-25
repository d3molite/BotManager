using System.Reflection;
using BotManager.Bot.Attributes;

namespace BotManager.Bot.Extensions;

public static class CommandLocatorExtension
{
	/// <summary>
	/// Fetches a list of all static tasks which are decorated with the <see cref="CommandBuilderAttribute"/> in a given type.
	/// </summary>
	/// <param name="type">The type to reflect over.</param>
	/// <returns>An enumerable with matching methods.</returns>
	public static IEnumerable<MethodInfo> GetCommandBuilders(this Type type)
	{
		return type
			.GetMethods(BindingFlags.Static | BindingFlags.Public)
			.Where(method => method.GetCustomAttribute<CommandBuilderAttribute>() != null);
	}

	/// <summary>
	/// Tries to fetch a task with a matching <see cref="CommandExecutorAttribute"/> from a specific type.
	/// </summary>
	/// <param name="type">The class type to search in.</param>
	/// <param name="commandName">The command name to look for.</param>
	/// <returns>A MethodInfo object for the specified command, or null if none was found.</returns>
	public static MethodInfo? GetCommandExecutor(this Type type, string commandName)
	{
		var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);

		var matchingTask = methods.FirstOrDefault(method => HasMatchingCommandName(method, commandName));

		return matchingTask;
	}

	private static bool HasMatchingCommandName(MethodInfo methodInfo, string commandName)
		=> methodInfo.GetCustomAttribute<CommandExecutorAttribute>()?.CommandName == commandName;
}