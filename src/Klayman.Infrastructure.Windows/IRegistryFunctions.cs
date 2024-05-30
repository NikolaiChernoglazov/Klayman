using Klayman.Domain;

namespace Klayman.Infrastructure.Windows;

public interface IRegistryFunctions
{
    string GetLocalizedKeyboardLayoutName(KeyboardLayoutId layoutId);
    
    IEnumerable<KeyboardLayoutId> GetPresentKeyboardLayoutIds();
    
    KeyboardLayoutId FindMatchingKeyboardLayoutId(IntPtr layoutHandle);
}