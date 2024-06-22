using FluentResults;
using Klayman.Domain;

namespace Klayman.ConsoleApp;

public static class LayoutDescriptorToIdConverter
{
    public static Result<KeyboardLayoutId> GetKeyboardLayoutIdFromDescriptor(string layoutDescriptor)
    {
        return KeyboardLayoutId.IsValid(layoutDescriptor)
            ? Result.Ok(new KeyboardLayoutId(layoutDescriptor))
            : LanguageTagFunctions.GetMatchingKeyboardLayoutId(layoutDescriptor);
    }
}