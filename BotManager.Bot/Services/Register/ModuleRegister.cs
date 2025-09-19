using System.Collections.Concurrent;
using System.Reflection;
using BotManager.Bot.Extensions;
using BotManager.Bot.Interfaces.Modules;
using BotManager.Bot.Modules.Constants;
using BotManager.Bot.Modules.Models;

namespace BotManager.Bot.Services.Register;

public static class ModuleRegister
{
	public static ConcurrentBag<RefModuleInfo> RefCommandModules { get; } = new();

	public static ConcurrentDictionary<ModuleData, IUtilityModule> UtilityModules { get; } = new();

	public static List<string> GetModuleNames(ulong clientId)
	{
		var commandModules = RefCommandModules.Where(x => x.ClientId == clientId).Select(x => x.Module.ModuleName);

		var utilityModules = UtilityModules.Where(x => x.Key.ClientId == clientId).Select(x => x.Value.ModuleName);

		return commandModules.Union(utilityModules).ToList();
	}

	public static IUtilityModule? TryGetLogger(ulong clientId, ulong guildID)
	{
		return UtilityModules.GetValueOrDefault(new ModuleData(ModuleType.Logging, clientId, guildID));
	}

	public static IRefCommandModule? TryGetFromRefCommand(string commandName, ulong clientId, ulong commandGuildId)
	{
		var potentialCandidates = RefCommandModules.Where(x => x.GuildId == commandGuildId && x.ClientId == clientId);

		return potentialCandidates.FirstOrDefault(x => x.Module.GetType().GetCommandExecutor(commandName) != null)
			?.Module;
	}

	public static IRefCommandModule? TryGetFromRefModal(string modalName, ulong clientId, ulong modalGuildId)
	{
		var potentialCandidates = RefCommandModules.Where(x => x.GuildId == modalGuildId && x.ClientId == clientId);

		return potentialCandidates.FirstOrDefault(x => x.Module.GetType().GetModalExecutor(modalName) != null)?.Module;
	}

	public static IRefCommandModule? TryGetFromRefComponent(
		string componentName,
		ulong clientId,
		ulong componentGuildId
	)
	{
		var potentialCandidates = RefCommandModules.Where(x => x.GuildId == componentGuildId && x.ClientId == clientId);

		return potentialCandidates.FirstOrDefault(x => x.Module.GetType().GetComponentExecutor(componentName) != null)
			?.Module;
	}
}