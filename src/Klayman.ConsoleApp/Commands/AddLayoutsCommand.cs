using CommandLine;
using Klayman.Application;
using Klayman.ConsoleApp.Extensions;
using Klayman.Domain;
using static Klayman.Domain.KeyboardLayoutId;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Klayman.ConsoleApp.Commands;

[Verb("add-layouts", aliases: ["add-layout"],
    HelpText = "Add one or more layouts to your current layout set." +
                               "Specify a KLID or a language tag for each layout you want to add.")]
internal class AddLayoutsCommand : ICommand
{
    [Value(0)]
    public IEnumerable<string> LanguageTagOrKeyboardLayoutIds { get; init; } = [];
    
    public void Execute(IKeyboardLayoutManager keyboardLayoutManager)
    {
        foreach (var languageTagOrKeyboardLayoutId in LanguageTagOrKeyboardLayoutIds)
        {
            var addResult = IsValidKeyboardLayoutId(languageTagOrKeyboardLayoutId)
                ? keyboardLayoutManager.AddKeyboardLayoutById(
                    new KeyboardLayoutId(languageTagOrKeyboardLayoutId))
                : keyboardLayoutManager.AddKeyboardLayoutByLanguageTag(
                    languageTagOrKeyboardLayoutId);


            if (addResult.IsFailed)
            {
                Console.WriteLine($"ERROR: Failed to add the layout {languageTagOrKeyboardLayoutId}. " +
                                  addResult.GetCombinedErrorMessage());
                continue;
            }
            
            Console.WriteLine($"Added keyboard layout: {addResult.Value}");
        }
    }
}