using System.Reflection;
using BotManager.Bot.Modules.Definitions;
using BotManager.Bot.Modules.Models;

namespace BotManager.Bot.Modules.Constants;

public static class ModuleFinder
{
	private static readonly Dictionary<string, ModuleType> _modalTypes = new()
	{
		{ Modals.OrderModalId, ModuleType.Order },
		{ Modals.OrderModalAddId, ModuleType.Order },
		{ Modals.VoiceModalId, ModuleType.Voice },
		{ Modals.RoleRequestModalId, ModuleType.RoleRequest },
	};

	private static readonly Dictionary<string, ModuleType> _buttonTypes = new()
	{
		{ Components.OrderButtonClose, ModuleType.Order },
		{ Components.OrderButtonDelete, ModuleType.Order },
		{ Components.OrderButtonAdd, ModuleType.Order },
		{ Components.OrderButtonArrived, ModuleType.Order },
		{ Components.OrderButtonRemove, ModuleType.Order },
		{ Components.OrderButtonReload, ModuleType.Order },
		{ Components.RoleRequestButtonAccept, ModuleType.RoleRequest },
		{ Components.RoleRequestButtonDeny, ModuleType.RoleRequest },
	};

	private static readonly Dictionary<string, ModuleType> _selectTypes = new()
	{
		{ Components.OrderRemoveSelectMenu, ModuleType.Order },
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

	public static ModuleType GetModuleBySelect(string modalId)
		=> _selectTypes.GetValueOrDefault(modalId, ModuleType.None);
}