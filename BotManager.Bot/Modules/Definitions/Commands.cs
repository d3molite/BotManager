using BotManager.Bot.Modules.Constants;
using BotManager.Bot.Modules.Models;

namespace BotManager.Bot.Modules.Definitions;

public static class Commands
{
    public const string Order = "order";

    public const string Birthday = "birthday";
    
    public const string ClearBirthday = "clearbirthday";

    #region Image Commands
    public const string Waaw = "waaw";

    public const string Woow = "woow";

    public const string Haah = "haah";

    public const string Hooh = "hooh";

    public const string Reverse = "reverse";

    public const string Gifspeed = "gifspeed";

    public const string NeedsMoreJpeg = "needsmorejpeg";
    
    #endregion Image Commands

    [ModuleType(ModuleType.Voice)]
    public const string Voice = "voice";
    
    public const string LanRolle = "lanrolle";

    public const string Feedback = "feedback";
    
    public const string WatchParty = "watchparty";
}