using FluentResults;
using Klayman.Domain;

namespace Klayman.Application;

public interface IKeyboardLayoutManager
{
    /// <summary>
    /// Retrieves the currently used keyboard layout
    /// </summary>
    Result<KeyboardLayout> GetCurrentKeyboardLayout();
    
    /// <summary>
    /// Retrieves the currently used keyboard layout set,
    /// e. g. list of layouts between which the user can switch
    /// </summary>
    Result<List<KeyboardLayout>> GetCurrentKeyboardLayoutSet();

    /// <summary>
    /// Retrieves all available keyboard layouts in the OS,
    /// which can be added to the current layout set.
    /// </summary>
    Result<List<KeyboardLayout>> GetAllAvailableKeyboardLayouts();

    /// <summary>
    /// Retrieves available keyboard layouts in the OS by query
    /// </summary>
    Result<List<KeyboardLayout>> GetAvailableKeyboardLayoutsByQuery(string query);
}