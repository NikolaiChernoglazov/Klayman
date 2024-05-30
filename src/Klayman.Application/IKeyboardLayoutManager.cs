using FluentResults;
using Klayman.Domain;

namespace Klayman.Application;

public interface IKeyboardLayoutManager
{
    Result<KeyboardLayout> GetCurrentKeyboardLayout();
}