using CommandLine;
using Klayman.Application;
using Klayman.ConsoleApp.Extensions;
// ReSharper disable UnusedType.Global

namespace Klayman.ConsoleApp.Commands;

[Verb("status", HelpText = "Display the current keyboard layout.")]
internal class StatusCommand : ICommand
{
    public void Execute(IKeyboardLayoutManager keyboardLayoutManager)
    {
        var currentLayoutResult = keyboardLayoutManager.GetCurrentKeyboardLayout();
        if (currentLayoutResult.IsFailed)
        {
            Console.WriteLine("ERROR: Could not get current keyboard layout. "
                              + currentLayoutResult.GetCombinedErrorMessage());
            return;
        }
        
        Console.WriteLine("Current keyboard layout: " + currentLayoutResult.Value);
    }
}