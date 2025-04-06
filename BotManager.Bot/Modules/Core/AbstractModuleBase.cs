using BotManager.Db.Models;

namespace BotManager.Bot.Modules.Core;

public class AbstractModuleBase<T>(GuildConfig guildConfig)
	where T : class
{
	protected string Locale => guildConfig.GuildLocale;
	
	protected GuildConfig GuildConfig => guildConfig;
	
	protected T ModuleConfig => (guildConfig.GetType().GetProperty(typeof(T).Name)!.GetValue(guildConfig) as T)!;
}