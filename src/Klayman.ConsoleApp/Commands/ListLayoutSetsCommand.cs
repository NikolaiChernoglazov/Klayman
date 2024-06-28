using CommandLine;
using Klayman.ConsoleApp.Extensions;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Klayman.ConsoleApp.Commands;

[Verb("list-sets", HelpText = "Display the list of keyboard layout sets.")]
public class ListLayoutSetsCommand : ICommand
{
    public async Task ExecuteAsync(KlaymanServiceClient klaymanServiceClient)
    {
        var result = await klaymanServiceClient.GetLayoutSetsAsync();
        
        if (result.IsFailed)
        {
            Console.WriteLine("ERROR: Could not get keyboard layout sets. "
                              + result.GetCombinedErrorMessage());
            return;
        }

        result.Value.ForEach(Console.WriteLine);
    }
}