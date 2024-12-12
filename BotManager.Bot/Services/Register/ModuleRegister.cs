using System.Collections.Concurrent;
using BotManager.Bot.Interfaces.Modules;
using BotManager.Bot.Modules.Constants;
using BotManager.Bot.Modules.Models;

namespace BotManager.Bot.Services.Register;

public static class ModuleRegister
{
	public static ConcurrentDictionary<ModuleData, ICommandModule> CommandModules { get; } = new();

	public static ConcurrentDictionary<ModuleData, IUtilityModule> UtilityModules { get; } = new();

	public static IUtilityModule? TryGetLogger(ulong clientId, ulong guildID)
	{
		return UtilityModules.GetValueOrDefault(new ModuleData(ModuleType.Logging, clientId, guildID));
	}
	
	public static ICommandModule? TryGetFromCommand(string name, ulong clientId, ulong guildId)
	{
		var moduleType = ModuleFinder.GetModuleByCommand(name);
		return CommandModules.GetValueOrDefault(new ModuleData(moduleType, clientId, guildId));
	}

	public static ICommandModule? TryGetFromModal(string name, ulong clientId, ulong guildId)
	{
		var moduleType = ModuleFinder.GetModuleByModal(name);
		return CommandModules.GetValueOrDefault(new ModuleData(moduleType, clientId, guildId));
	}

	public static ICommandModule? TryGetFromButton(string name, ulong clientId, ulong guildId)
	{
		var moduleType = ModuleFinder.GetModuleByButton(name);
		return CommandModules.GetValueOrDefault(new ModuleData(moduleType, clientId, guildId));
	}

	public static ICommandModule? TryGetFromSelect(string name, ulong clientId, ulong guildId)
	{
		var moduleType = ModuleFinder.GetModuleBySelect(name);
		return CommandModules.GetValueOrDefault(new ModuleData(moduleType, clientId, guildId));
	}
}