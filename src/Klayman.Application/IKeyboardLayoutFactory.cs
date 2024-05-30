using Klayman.Domain;

namespace Klayman.Application;

public interface IKeyboardLayoutFactory
{
    KeyboardLayout CreateFromKeyboardLayoutId(KeyboardLayoutId layoutId);
}