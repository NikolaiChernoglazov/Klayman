using System.Security;
using System.Text;
using FluentResults;
using Klayman.Application;
using Klayman.Domain;
using Klayman.Infrastructure.Windows.WinApi;

namespace Klayman.Infrastructure.Windows;

public class WindowsKeyboardLayoutManager(
    IWinApiFunctions winApiFunctions,
    IRegistryFunctions registryFunctions,
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

    public Result<List<KeyboardLayout>> GetCurrentKeyboardLayoutSet()
    {
        var layoutsCount = winApiFunctions.GetKeyboardLayoutList(0, null);
        if (layoutsCount == 0)
        {
            return Result.Fail(GetWin32FunctionErrorMessage(
                nameof(winApiFunctions.GetKeyboardLayoutList)));
        }

        var layoutHandles = new IntPtr[layoutsCount];
        if (winApiFunctions.GetKeyboardLayoutList(layoutsCount, layoutHandles) == 0)
        {
            return Result.Fail(GetWin32FunctionErrorMessage(
                nameof(winApiFunctions.GetKeyboardLayoutList)));
        }

        try
        {
            var layouts = layoutHandles
                .Select(registryFunctions.FindMatchingKeyboardLayoutId)
                .Select(keyboardLayoutFactory.CreateFromKeyboardLayoutId)
                .ToList();
            return Result.Ok(layouts);
        }
        catch (SecurityException)
        {
            return Result.Fail(GetRegistryAccessRequiredErrorMessage);
        }
    }

    public Result<List<KeyboardLayout>> GetAllAvailableKeyboardLayouts()
    {
        try
        {
            var layouts = registryFunctions.GetPresentKeyboardLayoutIds()
                .Select(keyboardLayoutFactory.CreateFromKeyboardLayoutId)
                .ToList();
            return Result.Ok(layouts);
        }
        catch (SecurityException)
        {
            return Result.Fail(GetRegistryAccessRequiredErrorMessage);
        }
    }
    
    public Result<List<KeyboardLayout>> GetAvailableKeyboardLayoutsByQuery(string query)
    {
        return GetAllAvailableKeyboardLayouts().Map(
            layouts => layouts
                .Where(l =>
                    (l.Culture?.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase) ?? false)
                    || (l.Name?.Contains(query, StringComparison.InvariantCultureIgnoreCase) ?? false)
                    || l.Id.ToString().Contains(query, StringComparison.InvariantCultureIgnoreCase))
                .ToList());
    }

    private string GetWin32FunctionErrorMessage(string functionName)
        => $"Function {functionName} returned an error {winApiFunctions.GetLastWin32Error()}";

    private string GetRegistryAccessRequiredErrorMessage
        => $"Access to the registry key {registryFunctions.GetKeyboardLayoutRegistryKeyPath()} is required.";
}