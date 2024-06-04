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
    ILanguageTagFunctions languageTagFunctions,
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
        try
        {
            return GetCurrentKeyboardLayoutHandles().Map(handles => handles
                .Select(registryFunctions.FindMatchingKeyboardLayoutId)
                .Select(keyboardLayoutFactory.CreateFromKeyboardLayoutId)
                .ToList());
        }
        catch (SecurityException)
        {
            return Result.Fail(GetRegistryAccessRequiredErrorMessage());
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
            return Result.Fail(GetRegistryAccessRequiredErrorMessage());
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

    public Result<KeyboardLayout> AddKeyboardLayoutById(KeyboardLayoutId layoutId)
    {
        try
        {
            if (!registryFunctions.GetPresentKeyboardLayoutIds().Contains(layoutId))
            {
                return Result.Fail(
                    $"Keyboard layout with ID {layoutId} is not registered in the OS. It should be present in the " +
                    $"{registryFunctions.GetKeyboardLayoutRegistryKeyPath()} Windows Registry path.");
            }
        }
        catch (SecurityException)
        {
            return Result.Fail(GetRegistryAccessRequiredErrorMessage());
        }

        var layoutHandle = winApiFunctions.LoadKeyboardLayoutW(layoutId, 0);
        return layoutHandle == IntPtr.Zero
            ? Result.Fail(GetWin32FunctionErrorMessage(
                nameof(winApiFunctions.LoadKeyboardLayoutW)))
            : Result.Ok(keyboardLayoutFactory.CreateFromKeyboardLayoutId(layoutId));
    }

    public Result<KeyboardLayout> AddKeyboardLayoutByLanguageTag(string languageTag)
    {
        var layoutIdResult = languageTagFunctions.GetMatchingKeyboardLayoutId(languageTag);
        return layoutIdResult.IsFailed
            ? layoutIdResult.ToResult<KeyboardLayout>()
            : AddKeyboardLayoutById(layoutIdResult.Value);
    }

    public Result<KeyboardLayout> RemoveKeyboardLayoutById(KeyboardLayoutId layoutId)
    {
        var layoutHandlesResult = GetCurrentKeyboardLayoutHandles();
        if (layoutHandlesResult.IsFailed)
        {
            const string explanationMessage =
                "To remove a keyboard layout, we need to retrieve the " +
                "current keyboard layout set first. But that operation failed. ";
            return layoutHandlesResult
                .MapErrors(e => new Error(explanationMessage + e.Message))
                .ToResult<KeyboardLayout>();
        }

        try
        {
            var layoutIdToHandleMapping = layoutHandlesResult.Value.ToDictionary(
                registryFunctions.FindMatchingKeyboardLayoutId,
                h => h);
            if (!layoutIdToHandleMapping.TryGetValue(layoutId, out var handle))
            {
                return Result.Fail(
                    $"Keyboard layout with ID {layoutId} is not present in the current keyboard layout set.");
            }
            if (!winApiFunctions.UnloadKeyboardLayout(handle))
            {
                return Result.Fail(GetWin32FunctionErrorMessage(
                    nameof(winApiFunctions.UnloadKeyboardLayout)));
            }

            return Result.Ok(keyboardLayoutFactory.CreateFromKeyboardLayoutId(layoutId));
        }
        catch (SecurityException)
        {
            return Result.Fail(GetRegistryAccessRequiredErrorMessage());
        }
    }

    public Result<KeyboardLayout> RemoveKeyboardLayoutByLanguageTag(string languageTag)
    {
        var layoutIdResult = languageTagFunctions.GetMatchingKeyboardLayoutId(languageTag);
        return layoutIdResult.IsFailed
            ? layoutIdResult.ToResult<KeyboardLayout>()
            : RemoveKeyboardLayoutById(layoutIdResult.Value);
    }

    private Result<IntPtr[]> GetCurrentKeyboardLayoutHandles()
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

        return Result.Ok(layoutHandles);
    }

    private string GetWin32FunctionErrorMessage(string functionName)
        => $"Function {functionName} returned an error {winApiFunctions.GetLastWin32Error()}";

    private string GetRegistryAccessRequiredErrorMessage()
        => $"Access to the registry key {registryFunctions.GetKeyboardLayoutRegistryKeyPath()} is required.";
}