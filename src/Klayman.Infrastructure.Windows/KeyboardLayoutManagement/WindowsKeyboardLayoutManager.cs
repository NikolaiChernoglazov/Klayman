using System.Security;
using System.Text;
using Klayman.Application;
using Klayman.Application.KeyboardLayoutManagement;
using Klayman.Domain;
using Klayman.Domain.Results;
using Klayman.Infrastructure.Windows.WinApi;

namespace Klayman.Infrastructure.Windows.KeyboardLayoutManagement;

public class WindowsKeyboardLayoutManager(
    IWinApiFunctions winApiFunctions,
    IRegistryFunctions registryFunctions,
    IKeyboardLayoutFactory keyboardLayoutFactory) : IKeyboardLayoutManager
{
    public Result<KeyboardLayout> GetCurrentLayout()
    {
        var layoutIdBuffer = new StringBuilder(KeyboardLayoutId.Length);
        if (!winApiFunctions.GetKeyboardLayoutNameW(layoutIdBuffer))
        {
            return Result.SystemFunctionFailed(GetWin32FunctionErrorMessage(
                nameof(winApiFunctions.GetKeyboardLayoutNameW)));
        }

        var layout = keyboardLayoutFactory.CreateFromKeyboardLayoutId(
            new KeyboardLayoutId(layoutIdBuffer.ToString()));
        return Result.Ok(layout);
    }

    public Result<List<KeyboardLayout>> GetCurrentLayouts()
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
            return Result.PermissionRequired(GetRegistryAccessRequiredErrorMessage());
        }
    }

    public Result<List<KeyboardLayout>> GetAllAvailableLayouts()
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
            return Result.PermissionRequired(GetRegistryAccessRequiredErrorMessage());
        }
    }
    
    public Result<List<KeyboardLayout>> GetAvailableLayoutsByQuery(string query)
    {
        return GetAllAvailableLayouts().Map(
            layouts => layouts
                .Where(l =>
                    (l.CultureName?.Contains(query, StringComparison.InvariantCultureIgnoreCase) ?? false)
                    || (l.Name?.Contains(query, StringComparison.InvariantCultureIgnoreCase) ?? false)
                    || l.Id.ToString().Contains(query, StringComparison.InvariantCultureIgnoreCase))
                .ToList());
    }

    public Result<KeyboardLayout> AddLayout(KeyboardLayoutId layoutId)
    {
        var canAddLayout = CanAddLayout(layoutId);
        if (canAddLayout.IsFailed)
        {
            return canAddLayout;
        }

        var layoutHandle = winApiFunctions.LoadKeyboardLayoutW(layoutId, 0);
        return layoutHandle == IntPtr.Zero
            ? Result.SystemFunctionFailed(GetWin32FunctionErrorMessage(
                nameof(winApiFunctions.LoadKeyboardLayoutW)))
            : Result.Ok(keyboardLayoutFactory.CreateFromKeyboardLayoutId(layoutId));
    }

    public Result<KeyboardLayout> RemoveLayout(KeyboardLayoutId layoutId)
    {
        var layoutHandlesResult = GetCurrentKeyboardLayoutHandles();
        if (layoutHandlesResult.IsFailed)
        {
            const string explanationMessage =
                "To remove a keyboard layout, we need to retrieve the " +
                "current keyboard layout set first. But that operation failed. ";
            return layoutHandlesResult.WithUpdatedErrorMessage(em =>
                explanationMessage + em);
        }

        try
        {
            var layoutIdToHandleMapping = layoutHandlesResult.Value.ToDictionary(
                registryFunctions.FindMatchingKeyboardLayoutId,
                h => h);
            if (!layoutIdToHandleMapping.TryGetValue(layoutId, out var handle))
            {
                return Result.NotFound(
                    $"Keyboard layout with ID {layoutId} is not present in the current keyboard layout set. ");
            }
            if (!winApiFunctions.UnloadKeyboardLayout(handle))
            {
                return Result.SystemFunctionFailed(GetWin32FunctionErrorMessage(
                    nameof(winApiFunctions.UnloadKeyboardLayout)));
            }

            return Result.Ok(keyboardLayoutFactory.CreateFromKeyboardLayoutId(layoutId));
        }
        catch (SecurityException)
        {
            return Result.PermissionRequired(GetRegistryAccessRequiredErrorMessage());
        }
    }

    public Result CanAddLayout(KeyboardLayoutId layoutId)
    {
        try
        {
               return registryFunctions.GetPresentKeyboardLayoutIds().Contains(layoutId)
                   ? Result.Ok()
                   : Result.NotFound($"Keyboard layout with ID {layoutId} is not registered in the OS. It should be present in the " +
                                         $"{registryFunctions.GetKeyboardLayoutRegistryKeyPath()} Windows Registry path.");
        }
        catch (SecurityException)
        {
            return Result.PermissionRequired(GetRegistryAccessRequiredErrorMessage());
        }
    }

    private Result<IntPtr[]> GetCurrentKeyboardLayoutHandles()
    {
        var layoutsCount = winApiFunctions.GetKeyboardLayoutList(0, null);
        if (layoutsCount == 0)
        {
            return Result.SystemFunctionFailed(GetWin32FunctionErrorMessage(
                nameof(winApiFunctions.GetKeyboardLayoutList)));
        }

        var layoutHandles = new IntPtr[layoutsCount];
        if (winApiFunctions.GetKeyboardLayoutList(layoutsCount, layoutHandles) == 0)
        {
            return Result.SystemFunctionFailed(GetWin32FunctionErrorMessage(
                nameof(winApiFunctions.GetKeyboardLayoutList)));
        }

        return Result.Ok(layoutHandles);
    }

    private string GetWin32FunctionErrorMessage(string functionName)
        => $"Function {functionName} returned an error {winApiFunctions.GetLastWin32Error()}";

    private string GetRegistryAccessRequiredErrorMessage()
        => $"Access to the registry key {registryFunctions.GetKeyboardLayoutRegistryKeyPath()} is required.";
}