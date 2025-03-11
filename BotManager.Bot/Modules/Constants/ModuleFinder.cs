using System.Reflection;
using BotManager.Bot.Modules.Definitions;
using BotManager.Bot.Modules.Models;

namespace BotManager.Bot.Modules.Constants;

public static class ModuleFinder
{
	private static readonly Dictionary<string, ModuleType> _modalTypes = new()
	{
		{ ModalFields.OrderModalId, ModuleType.Order },
		{ ModalFields.OrderModalAddId, ModuleType.Order },
		{ ModalFields.VoiceModalId, ModuleType.Voice },
		{ ModalFields.RoleRequestModalId, ModuleType.RoleRequest },
	};

	private static readonly Dictionary<string, ModuleType> _buttonTypes = new()
	{
		{ ControlNames.OrderButtonClose, ModuleType.Order },
		{ ControlNames.OrderButtonDelete, ModuleType.Order },
		{ ControlNames.OrderButtonAdd, ModuleType.Order },
		{ ControlNames.OrderButtonArrived, ModuleType.Order },
		{ ControlNames.OrderButtonRemove, ModuleType.Order },
		{ ControlNames.OrderButtonReload, ModuleType.Order },
		{ ControlNames.RoleRequestButtonAccept, ModuleType.RoleRequest },
		{ ControlNames.RoleRequestButtonDeny, ModuleType.RoleRequest },
	};

	private static readonly Dictionary<string, ModuleType> _selectTypes = new()
	{
		{ ControlNames.OrderRemoveSelectMenu, ModuleType.Order },
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