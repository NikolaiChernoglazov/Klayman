using System.Globalization;
using Klayman.Domain;
using Klayman.Domain.Results;

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
            return Result.Ok(new KeyboardLayoutId(layoutId));
        }
        
        try
        {
            var culture = CultureInfo.CreateSpecificCulture(languageTag);
            return culture.LCID == CultureInfo.InvariantCulture.LCID 
                ? Result.Fail($"Could not find a matching KLID for a language tag {languageTag}.")
                : Result.Ok(new KeyboardLayoutId(culture.LCID));
        }
        catch (CultureNotFoundException)
        {
            return Result.Fail($"Could not find a matching KLID for a language tag {languageTag}.");
        }
    }
}