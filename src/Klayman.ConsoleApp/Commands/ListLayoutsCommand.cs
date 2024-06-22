using CommandLine;
using FluentResults;
using Klayman.ConsoleApp.Extensions;
using Klayman.Domain;
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

    public async Task ExecuteAsync(KlaymanServiceClient klaymanServiceClient)
    {
        Result<List<KeyboardLayout>> layoutsResult;
        if (ListAllAvailableLayouts)
            layoutsResult = await klaymanServiceClient.GetAllAvailableLayoutsAsync();
        else if (string.IsNullOrEmpty(Query))
            layoutsResult = await klaymanServiceClient.GetCurrentLayoutsAsync();
        else
            layoutsResult = await klaymanServiceClient.GetAvailableLayoutsByQueryAsync(Query);
        
        if (layoutsResult.IsFailed)
        {
            Console.WriteLine("ERROR: Could not get keyboard layouts. "
                              + layoutsResult.GetCombinedErrorMessage());
            return;
        }

        layoutsResult.Value.ForEach(Console.WriteLine);
    }
}