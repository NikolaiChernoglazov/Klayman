using CommandLine;
using Klayman.ConsoleApp.Extensions;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Klayman.ConsoleApp.Commands;

[Verb("apply-set", HelpText = "Apply a keyboard layout set, e. g. use keyboard layouts it contains")]
public class ApplyLayoutSetCommand : ICommand
{
    [Value(0)]
    public string Name { get; init; } = string.Empty;
    
    public async Task ExecuteAsync(KlaymanServiceClient klaymanServiceClient)
    {
        var result = await klaymanServiceClient.ApplyLayoutSetAsync(Name);
        if (result.IsFailed)
        {
            Console.WriteLine(result.GetCombinedErrorMessage());
            return;
        }

        Console.WriteLine($"Keyboard layout set {Name} has been applied successfully.");
    }
}