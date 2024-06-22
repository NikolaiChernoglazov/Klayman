using System.Globalization;
using Klayman.Domain;

namespace Klayman.Application;

public class KeyboardLayoutFactory(
    IKeyboardLayoutNameProvider keyboardLayoutNameProvider) : IKeyboardLayoutFactory
{
    public KeyboardLayout CreateFromKeyboardLayoutId(KeyboardLayoutId layoutId)
    {
        return new KeyboardLayout(layoutId,
            keyboardLayoutNameProvider.GetKeyboardLayoutName(layoutId),
            GetCultureFromKeyboardLayoutId(layoutId)?.Name);
    }
    
    private static CultureInfo? GetCultureFromKeyboardLayoutId(
        KeyboardLayoutId layoutId)
    {
        try
        {
            return new CultureInfo(layoutId.GetLanguageId());
        }
        catch (CultureNotFoundException)
        {
            return null;
        }
    }
}