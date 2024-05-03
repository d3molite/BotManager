using BotManager.Bot.Modules.Constants;

namespace BotManager.Bot.Modules.Models;

public class ModuleTypeAttribute(ModuleType ModuleType) : Attribute
{
    public ModuleType ModuleType { get; } = ModuleType;
}