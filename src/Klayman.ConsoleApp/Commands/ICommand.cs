using Klayman.Application;

namespace Klayman.ConsoleApp.Commands;

public interface ICommand
{
    void Execute(IKeyboardLayoutManager keyboardLayoutManager);
}