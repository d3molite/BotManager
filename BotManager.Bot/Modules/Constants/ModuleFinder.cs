using BotManager.Bot.Modules.Definitions;

namespace BotManager.Bot.Modules.Constants;

public static class ModuleFinder
{
    private static readonly Dictionary<string, ModuleType> _commandTypes = new()
    {
        { Commands.Order, ModuleType.Order },
        { Commands.Waaw, ModuleType.Image },
        { Commands.Woow, ModuleType.Image },
        { Commands.Haah, ModuleType.Image },
        { Commands.Hooh, ModuleType.Image },
    };

    private static readonly Dictionary<string, ModuleType> _modalTypes = new()
    {
        { ModalFields.OrderModalId, ModuleType.Order },
        { ModalFields.OrderModalAddId, ModuleType.Order },
    };

    private static readonly Dictionary<string, ModuleType> _buttonTypes = new()
    {
        { ControlNames.OrderButtonClose, ModuleType.Order },
        { ControlNames.OrderButtonDelete, ModuleType.Order },
        { ControlNames.OrderButtonAdd, ModuleType.Order },
        { ControlNames.OrderButtonArrived, ModuleType.Order },
        { ControlNames.OrderButtonRemove, ModuleType.Order },
        { ControlNames.OrderButtonReload, ModuleType.Order },
    };

    private static readonly Dictionary<string, ModuleType> _selectTypes = new()
    {
        { ControlNames.OrderRemoveSelectMenu, ModuleType.Order },
    };

    public static ModuleType GetModuleByCommand(string command) 
        => _commandTypes.GetValueOrDefault(command, ModuleType.None);

    public static ModuleType GetModuleByButton(string buttonName)
        => _buttonTypes.GetValueOrDefault(buttonName, ModuleType.None);
    public static ModuleType GetModuleByModal(string modalId)
        => _modalTypes.GetValueOrDefault(modalId, ModuleType.None);
    
    public static ModuleType GetModuleBySelect(string modalId)
        => _selectTypes.GetValueOrDefault(modalId, ModuleType.None);
}