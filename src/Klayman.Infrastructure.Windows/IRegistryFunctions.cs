using Klayman.Domain;

namespace Klayman.Infrastructure.Windows;

public interface IRegistryFunctions
{
    string GetKeyboardLayoutRegistryKeyPath();
    
    string? GetLocalizedKeyboardLayoutName(KeyboardLayoutId layoutId);
    
    /// <summary>
    /// Retrieves the keyboard layout identifiers present in the registry.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.Security.SecurityException">
    /// The user does not have required permissions to read the keyboard layouts registry key
    /// </exception>
    IEnumerable<KeyboardLayoutId> GetPresentKeyboardLayoutIds();
    
    /// <summary>
    /// Searches for the keyboard layout identifier in registry,
    /// corresponding to a given keyboard layout handle (HKL).
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.Security.SecurityException">
    /// The user does not have required permissions to read the keyboard layouts registry key
    /// </exception>
    KeyboardLayoutId FindMatchingKeyboardLayoutId(IntPtr layoutHandle);
}