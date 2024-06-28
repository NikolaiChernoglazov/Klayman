using CommandLine;
using Klayman.ConsoleApp.Extensions;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Klayman.ConsoleApp.Commands;

[Verb("add-layouts", aliases: ["add-layout"],
    HelpText = "Add one or more layouts to your current layout set. " +
                               "Specify a KLID or a language tag for each layout you want to add.")]
internal class AddLayoutsCommand : ICommand
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
                return;
            }

            var result = await serviceClient.AddLayoutAsync(layoutIdResult.Value);
            if (result.IsFailed)
            {
                Console.WriteLine($"ERROR: Failed to add the layout {layoutDescriptor}. " +
                                  result.GetCombinedErrorMessage());
                continue;
            }
            
            Console.WriteLine($"Added keyboard layout: {result.Value}");
        }
    }
}