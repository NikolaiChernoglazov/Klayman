using CommandLine;
using Klayman.ConsoleApp.Extensions;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Klayman.ConsoleApp.Commands;

[Verb("remove-set", HelpText = "Remove a keyboard layout set.")]
public class RemoveLayoutSetCommand : ICommand
{
    [Value(0)]
    public string Name { get; init; } = string.Empty;
    
    public async Task ExecuteAsync(KlaymanServiceClient klaymanServiceClient)
    {
        var result = await klaymanServiceClient.RemoveLayoutSetAsync(Name);
        if (result.IsFailed)
        {
            Console.WriteLine(result.GetCombinedErrorMessage());
            return;
        }
        
        Console.WriteLine($"Keyboard layout set {Name} has been removed successfully.");
    }
}