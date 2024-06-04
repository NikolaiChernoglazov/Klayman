using System.Globalization;
using FluentResults;
using Klayman.Domain;

namespace Klayman.Application;

public class LanguageTagFunctions : ILanguageTagFunctions
{
    private readonly Dictionary<string, string>
        _languageTagToLayoutIdMapping = new()
        {
            {"dvorak", "00010409"},
            {"es", "0000040A"},
            {"es-es", "0000040A"},
            {"uk", "00020422"},
            {"uk-ua", "00020422" }
        };
    
    public Result<KeyboardLayoutId> GetMatchingKeyboardLayoutId(string languageTag)
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