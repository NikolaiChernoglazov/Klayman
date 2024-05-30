using System.Text;
using FluentResults;
using Klayman.Application;
using Klayman.Domain;
using Klayman.Infrastructure.Windows.WinApi;

namespace Klayman.Infrastructure.Windows;

public class WindowsKeyboardLayoutManager(
    IWinApiFunctions winApiFunctions,
    IKeyboardLayoutFactory keyboardLayoutFactory) : IKeyboardLayoutManager
{
    public Result<KeyboardLayout> GetCurrentKeyboardLayout()
    {
        var layoutIdBuffer = new StringBuilder(KeyboardLayoutId.Length);
        if (!winApiFunctions.GetKeyboardLayoutNameW(layoutIdBuffer))
        {
            return Result.Fail(GetWin32FunctionErrorMessage(
                nameof(winApiFunctions.GetKeyboardLayoutNameW)));
        }

        var layout = keyboardLayoutFactory.CreateFromKeyboardLayoutId(
            new KeyboardLayoutId(layoutIdBuffer.ToString()));
        return Result.Ok(layout);
    }

    private string GetWin32FunctionErrorMessage(string functionName)
        => $"Function {functionName} returned an error {winApiFunctions.GetLastWin32Error()}";
}