using BotManager.Bot.Modules.Constants;

namespace BotManager.Bot.Modules.Models;

public class ModuleData(ModuleType moduleType, ulong clientId, ulong guildId)
{
    private ModuleType ModuleType { get; } = moduleType;

    private ulong GuildId { get; } = guildId;

    public ulong ClientId { get; } = clientId;
    
    public override bool Equals(object? obj)
    {
        if (obj is ModuleData mgp)
        {
            return ModuleType == mgp.ModuleType && GuildId == mgp.GuildId && ClientId == mgp.ClientId;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return ModuleType.GetHashCode() + GuildId.GetHashCode() + ClientId.GetHashCode();
    }
};