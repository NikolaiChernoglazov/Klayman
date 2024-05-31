using Klayman.Application;
using Klayman.Domain;

namespace Klayman.Infrastructure.Windows;

public class WindowsKeyboardLayoutNameProvider(
    IRegistryFunctions registryFunctions) : IKeyboardLayoutNameProvider
{
    public string? GetKeyboardLayoutName(KeyboardLayoutId layoutId)
    {
        return registryFunctions.GetLocalizedKeyboardLayoutName(layoutId);
    }
}