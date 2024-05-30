using System.Text.RegularExpressions;

namespace Klayman.Domain;

/// <summary>
/// Represents a keyboard layout identifier (KLID). It is a string composed of the hexadecimal value
/// of the Language identifier (low word) and a device identifier (high word).
/// </summary>
public partial class KeyboardLayoutId : IEquatable<KeyboardLayoutId>
{
    /// <summary>
    /// Represents the number of characters in a valid KLID.
    /// </summary>
    public const int Length = 8;
    
    private readonly string _value;

    public KeyboardLayoutId(string layoutId)
    {
        if (!IsValidKeyboardLayoutId(layoutId))
            throw new ArgumentException($"{layoutId} is not a valid KLID.");
        
        _value = layoutId.ToUpperInvariant();
    }
    
    
    public int GetLanguageId()
    {
        // Note: we cannot use Convert.ToInt32(), because it will interpret
        // the hexadecimal value as a signed binary form of an integer
        return Convert.ToUInt16(_value.Substring(4, 4), 16);
    }
    
    
    public static implicit operator string(KeyboardLayoutId layoutId)
    {
        return layoutId._value;
    }
    
    public static bool IsValidKeyboardLayoutId(string value)
    {
        return ValidKeyboardLayoutIdRegex().IsMatch(value);
    }
    
    [GeneratedRegex("^[0-9a-fA-F]{8}$")]
    private static partial Regex ValidKeyboardLayoutIdRegex();
    
    
    public override string ToString() => _value;
    
    public bool Equals(KeyboardLayoutId? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _value == other._value;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((KeyboardLayoutId)obj);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
}