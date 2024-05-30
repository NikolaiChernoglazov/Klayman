using CommandLine;
using Klayman.Application;
using Klayman.ConsoleApp.Extensions;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Klayman.ConsoleApp.Commands;

[Verb("list-layouts", HelpText = "Display the current set of keyboard layouts.")]
internal class ListLayoutsCommand : ICommand
{
    [Option(shortName:'a', longName: "all", SetName = "filter",
        HelpText = "Display all available keyboard layouts in the OS.")]
    public bool ListAllAvailableLayouts { get; init; }

    [Option(shortName: 'q', longName: "query", SetName = "filter",
        HelpText = "Display available keyboard layouts in the OS that match the query.")]
    public string Query { get; init; } = string.Empty;

    public void Execute(IKeyboardLayoutManager keyboardLayoutManager)
    {
        if (ListAllAvailableLayouts)
        {
            keyboardLayoutManager.GetAllAvailableKeyboardLayouts()
                .ToList().ForEach(Console.WriteLine);
        }
        else if (string.IsNullOrEmpty(Query))
        {
            var layoutsResult = keyboardLayoutManager.GetCurrentKeyboardLayoutSet();
            if (layoutsResult.IsFailed)
            {
                Console.WriteLine("ERROR: Could not get keyboard layouts. "
                                  + layoutsResult.GetCombinedErrorMessage());
                return;
            }

            layoutsResult.Value.ToList().ForEach(Console.WriteLine);
        }
        else
        {
            keyboardLayoutManager.GetAvailableKeyboardLayoutsByQuery(Query)
                .ToList().ForEach(Console.WriteLine);
        }
    }
}