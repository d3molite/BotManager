using System.Reflection;
using BotManager.Bot.Modules.Definitions;
using BotManager.Bot.Modules.Models;

namespace BotManager.Bot.Modules.Constants;

public static class ModuleFinder
{
	private static readonly Dictionary<string, ModuleType> _modalTypes = new()
	{
		{ Modals.VoiceModalId, ModuleType.Voice },
		{ Modals.RoleRequestModalId, ModuleType.RoleRequest },
	};

	private static readonly Dictionary<string, ModuleType> _buttonTypes = new()
	{
		{ Components.RoleRequestButtonAccept, ModuleType.RoleRequest },
		{ Components.RoleRequestButtonDeny, ModuleType.RoleRequest },
	};

	public static ModuleType GetModuleByCommand(string command)
	{
		var field = typeof(Commands).GetField(
			command,
			BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public
		);

		var attribute = field?.GetCustomAttribute<ModuleTypeAttribute>();
		return attribute?.ModuleType ?? ModuleType.None;
	}

	public static ModuleType GetModuleByButton(string buttonName)
		=> _buttonTypes.GetValueOrDefault(buttonName, ModuleType.None);

	public static ModuleType GetModuleByModal(string modalId)
		=> _modalTypes.GetValueOrDefault(modalId, ModuleType.None);
	
}