using System.Collections.Concurrent;
using BotManager.Bot.Interfaces.Services;
using BotManager.Bot.Modules.Constants;
using BotManager.Bot.Modules.Models;

namespace BotManager.Bot.Services;

public static class ModuleRegister
{
    public static ConcurrentDictionary<ModuleData, ICommandModule> Modules { get; } = new();

    public static ICommandModule? TryGetFromCommand(string name, ulong clientId, ulong guildId)
    {
        var moduleType = ModuleFinder.GetModuleByCommand(name);
        return Modules.GetValueOrDefault(new(moduleType, clientId, guildId));
    }
	
    public static ICommandModule? TryGetFromModal(string name, ulong clientId, ulong guildId)
    {
        var moduleType = ModuleFinder.GetModuleByModal(name);
        return Modules.GetValueOrDefault(new(moduleType, clientId, guildId));
    }
	
    public static ICommandModule? TryGetFromButton(string name, ulong clientId, ulong guildId)
    {
        var moduleType = ModuleFinder.GetModuleByButton(name);
        return Modules.GetValueOrDefault(new(moduleType, clientId, guildId));
    }
	
    public static ICommandModule? TryGetFromSelect(string name, ulong clientId, ulong guildId)
    {
        var moduleType = ModuleFinder.GetModuleBySelect(name);
        return Modules.GetValueOrDefault(new(moduleType, clientId, guildId));
    }
}