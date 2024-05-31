using System.Globalization;

namespace Klayman.Domain;

/// <summary>
/// Represents a keyboard layout.
/// </summary>
/// <param name="Id">The identifier of the layout (KLID).</param>
/// <param name="Name">The name of the layout.</param>
/// <param name="Culture">The culture corresponding to the layout.</param>
public record KeyboardLayout(
    KeyboardLayoutId Id,
    string? Name,
    CultureInfo? Culture)
{
    public override string ToString()
    {
        return $"{Id} {Culture?.ToString() ?? "     "}" +
               $" {Name ?? Culture?.DisplayName ?? "Unknown"}";
    }
}