using Klayman.Domain;

namespace Klayman.Application;

public interface IKeyboardLayoutNameProvider
{
    string GetKeyboardLayoutName(KeyboardLayoutId layoutId);
}