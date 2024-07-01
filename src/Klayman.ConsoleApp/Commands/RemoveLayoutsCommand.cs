using CommandLine;
using Klayman.ServiceClient;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Klayman.ConsoleApp.Commands;

[Verb("remove-layouts", aliases: ["remove-layout"],
    HelpText = "Remove one or more layouts from your current layout set. " +
                               "Specify a KLID or a language tag for each layout you want to add.")]
internal class RemoveLayoutsCommand : ICommand
{
    [Value(0)]
    public IEnumerable<string> LayoutDescriptors { get; init; } = [];
    
    public async Task ExecuteAsync(KlaymanServiceClient serviceClient)
    {
        foreach (var layoutDescriptor in LayoutDescriptors)
        {
            var layoutIdResult = LayoutDescriptorToIdConverter.GetKeyboardLayoutIdFromDescriptor(
                layoutDescriptor);
            if (layoutIdResult.IsFailed)
            {
                Console.WriteLine($"{layoutDescriptor} is not a valid ID or language tag.");
            }

            var result = await serviceClient.RemoveLayoutAsync(layoutIdResult.Value);
            if (result.IsFailed)
            {
                Console.WriteLine($"ERROR: Failed to remove the layout {layoutDescriptor}. " +
                                  result.ErrorMessage);
                continue;
            }
            
            Console.WriteLine($"Removed keyboard layout: {result.Value}");
        }
    }
}