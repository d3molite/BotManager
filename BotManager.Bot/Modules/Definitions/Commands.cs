using BotManager.Bot.Modules.Constants;
using BotManager.Bot.Modules.Models;

namespace BotManager.Bot.Modules.Definitions;

public static class Commands
{
    [ModuleType(ModuleType.Order)]
    public const string Order = "order";

    [ModuleType(ModuleType.Image)]
    public const string Waaw = "waaw";

    [ModuleType(ModuleType.Image)]
    public const string Woow = "woow";

    [ModuleType(ModuleType.Image)]
    public const string Haah = "haah";

    [ModuleType(ModuleType.Image)]
    public const string Hooh = "hooh";

    [ModuleType(ModuleType.Image)]
    public const string Reverse = "reverse";

    [ModuleType(ModuleType.Image)]
    public const string NeedsMoreJpeg = "needsmorejpeg";
}