using FluentResults;
using Klayman.Domain;

namespace Klayman.Application;

public interface IKeyboardLayoutManager
{
    /// <summary>
    /// Retrieves the currently used keyboard layout
    /// </summary>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the layout has been retrieved.
    /// In case of success the result will contain a current <see cref="KeyboardLayout"/>.
    /// </returns>
    Result<KeyboardLayout> GetCurrentLayout();
    
    /// <summary>
    /// Retrieves the currently used keyboard layout set,
    /// e. g. list of layouts between which the user can switch
    /// </summary>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the list of layout has been retrieved.
    /// In case of success the result will contain a list of current <see cref="KeyboardLayout"/>s.
    /// </returns>
    Result<List<KeyboardLayout>> GetCurrentLayouts();

    /// <summary>
    /// Retrieves all available keyboard layouts in the OS,
    /// which can be added to the current layout set.
    /// </summary>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the layouts have been retrieved.
    /// In case of success the result will contain a list of current <see cref="KeyboardLayout"/>s.
    /// </returns>
    Result<List<KeyboardLayout>> GetAllAvailableLayouts();

    /// <summary>
    /// Retrieves available keyboard layouts in the OS by query
    /// </summary>
    /// <param name="query">A query, by which available layouts will be filtered. It could be a keyboard layout identifier,
    /// name, or a languag3e tag.</param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the layouts have been retrieved.
    /// In case of success the result will contain a list of current <see cref="KeyboardLayout"/>s.
    /// </returns>
    Result<List<KeyboardLayout>> GetAvailableLayoutsByQuery(string query);
    
    /// <summary>
    /// Adds a new keyboard layout to the current keyboard layout set.
    /// </summary>
    /// <param name="layoutId">A keyboard layout identifier (KLID).</param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the layout has been added.
    /// In case of success the result will contain an added <see cref="KeyboardLayout"/>.
    /// </returns>
    Result<KeyboardLayout> AddLayout(KeyboardLayoutId layoutId);
    
    /// <summary>
    /// Removes a keyboard layout from the current keyboard layout set.
    /// </summary>
    /// <param name="layoutId">A keyboard layout identifier (KLID).</param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the layout has been removed.
    /// In case of success the result will contain an removed <see cref="KeyboardLayout"/>.
    /// </returns>
    Result<KeyboardLayout> RemoveLayout(KeyboardLayoutId layoutId);
}