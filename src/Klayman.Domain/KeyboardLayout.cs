namespace Klayman.Domain;

/// <summary>
/// Represents a keyboard layout.
/// </summary>
/// <param name="Id">The identifier of the layout (KLID).</param>
/// <param name="Name">The name of the layout.</param>
/// <param name="CultureName">The name of a culture corresponding to the layout.</param>
public record KeyboardLayout(
    KeyboardLayoutId Id,
    string? Name,
    string? CultureName)
{
    public override string ToString()
    {
        return $"{Id} {CultureName ?? "     "}" +
               $" {Name ?? "Unknown"}";
    }
}