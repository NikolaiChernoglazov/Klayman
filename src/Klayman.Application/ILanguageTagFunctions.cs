using FluentResults;
using Klayman.Domain;

namespace Klayman.Application;

public interface ILanguageTagFunctions
{
    Result<KeyboardLayoutId> GetMatchingKeyboardLayoutId(string languageTag);
}