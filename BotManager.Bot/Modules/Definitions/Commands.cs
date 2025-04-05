using BotManager.Bot.Modules.Constants;
using BotManager.Bot.Modules.Models;

namespace BotManager.Bot.Modules.Definitions;

public static class Commands
{
    [ModuleType(ModuleType.Order)]
    public const string Order = "order";

    [ModuleType(ModuleType.Birthdays)] 
    public const string Birthday = "birthday";

    [ModuleType(ModuleType.Birthdays)] 
    public const string ClearBirthday = "clearbirthday";

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
    public const string Gifspeed = "gifspeed";

    [ModuleType(ModuleType.Image)]
    public const string NeedsMoreJpeg = "needsmorejpeg";

    [ModuleType(ModuleType.Voice)]
    public const string Voice = "voice";

    [ModuleType(ModuleType.RoleRequest)]
    public const string LanRolle = "lanrolle";

    [ModuleType(ModuleType.Feedback)]
    public const string Feedback = "feedback";
}