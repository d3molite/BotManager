using BotManager.Db.Models;

namespace BotManager.Bot.Modules.Core;

public class AbstractModuleBase<T>(GuildConfig config)
	where T : class
{
	protected string Locale => config.GuildLocale;
	
	protected GuildConfig GuildConfig => config;
	
	protected T ModuleConfig => (config.GetType().GetProperty(typeof(T).Name)!.GetValue(config) as T)!;
	
}