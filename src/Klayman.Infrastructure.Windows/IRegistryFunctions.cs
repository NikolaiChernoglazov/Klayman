using Klayman.Domain;

namespace Klayman.Infrastructure.Windows;

public interface IRegistryFunctions
{
    string GetLocalizedKeyboardLayoutName(KeyboardLayoutId layoutId);
}