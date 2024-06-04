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

[Verb("remove-layouts", aliases: ["remove-layout"],
    HelpText = "Remove one or more layouts from your current layout set." +
                               "Specify a KLID or a language tag for each layout you want to add.")]
internal class RemoveLayoutsCommand : ICommand
{
    [Value(0)]
    public IEnumerable<string> LanguageTagOrKeyboardLayoutIds { get; init; } = [];
    
    public void Execute(IKeyboardLayoutManager keyboardLayoutManager)
    {
        foreach (var languageTagOrKeyboardLayoutId in LanguageTagOrKeyboardLayoutIds)
        {
            var removeResult = IsValidKeyboardLayoutId(languageTagOrKeyboardLayoutId)
                ? keyboardLayoutManager.RemoveKeyboardLayoutById(
                    new KeyboardLayoutId(languageTagOrKeyboardLayoutId))
                : keyboardLayoutManager.RemoveKeyboardLayoutByLanguageTag(
                    languageTagOrKeyboardLayoutId);

            if (removeResult.IsFailed)
            {
                Console.WriteLine($"ERROR: Failed to remove the layout {languageTagOrKeyboardLayoutId}. " +
                                  removeResult.GetCombinedErrorMessage());
                continue;
            }
            
            Console.WriteLine($"Removed keyboard layout: {removeResult.Value}");
        }
    }
}