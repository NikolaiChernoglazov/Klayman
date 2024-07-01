using CommandLine;
using Klayman.Domain;
using Klayman.ServiceClient;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Klayman.ConsoleApp.Commands;

[Verb("add-set",
    HelpText = "Add a keyboard layout set. " +
               "Specify a set name and a KLID or a language tag for each layout you want to add.")]
public class AddLayoutSetCommand : ICommand
{
    [Option(shortName: 'n', longName: "name", Required = true)]
    public string Name { get; init; } = string.Empty;
    
    [Value(0)]
    public IEnumerable<string> LayoutDescriptors { get; init; } = [];

    
    public async Task ExecuteAsync(KlaymanServiceClient klaymanServiceClient)
    {
        var layoutIds = new List<KeyboardLayoutId>();
        foreach (var layoutDescriptor in LayoutDescriptors)
        {
            var layoutIdResult = LayoutDescriptorToIdConverter.GetKeyboardLayoutIdFromDescriptor(
                layoutDescriptor);
            if (layoutIdResult.IsFailed)
            {
                Console.WriteLine($"{layoutDescriptor} is not a valid ID or language tag.");
                return;
            }

            layoutIds.Add(layoutIdResult.Value);
        }
    
        var result = await klaymanServiceClient.AddLayoutSetAsync(
            new AddKeyboardLayoutSetRequest(Name, layoutIds));
        if (result.IsFailed)
        {
            Console.WriteLine("Failed to add a keyboard layout set. " +
                              result.ErrorMessage);
            return;
        }
        
        Console.WriteLine("Layout set has ben added.");
        Console.WriteLine(result.Value);
    }
}