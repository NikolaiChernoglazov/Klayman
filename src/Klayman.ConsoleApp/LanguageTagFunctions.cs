using System.Globalization;
using FluentResults;
using Klayman.Domain;

namespace Klayman.ConsoleApp;

public static class LanguageTagFunctions
{
    private static readonly Dictionary<string, string>
        _languageTagToLayoutIdMapping = new()
        {
            {"dvorak", "00010409"},
            {"es", "0000040A"},
            {"es-es", "0000040A"},
            {"uk", "00020422"},
            {"uk-ua", "00020422" }
        };
    
    public static Result<KeyboardLayoutId> GetMatchingKeyboardLayoutId(string languageTag)
    {
        if (_languageTagToLayoutIdMapping.TryGetValue(languageTag.ToLowerInvariant(),
                 out var layoutId))
        {
            return new KeyboardLayoutId(layoutId);
        }
        
        try
        {
            var culture = CultureInfo.CreateSpecificCulture(languageTag);
            return new KeyboardLayoutId(culture.LCID);
        }
        catch (CultureNotFoundException)
        {
            return Result.Fail($"Could not find a matching KLID for a language tag {languageTag}.");
        }
    }
}