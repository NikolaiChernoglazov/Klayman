using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.Text;
using Klayman.Domain;
using Klayman.Infrastructure.Windows.WinApi;
using Microsoft.Win32;

namespace Klayman.Infrastructure.Windows;

[SupportedOSPlatform("windows")]
public class RegistryFunctions(
    IWinApiFunctions winApiFunctions) : IRegistryFunctions
{
    private const string KeyboardLayoutsRegistryPath
        = @"SYSTEM\CurrentControlSet\Control\Keyboard Layouts";

    /// <summary>
    /// Retrieves the localized string associated with the specified name.
    /// </summary>
    /// <param name = "keyHandle">The handle of an opened registry key to load the string from.</param>
    /// <param name = "name">The name of the string to load.</param>
    /// <param name="initialBufferCapacity">Initial size of a buffer to receive the string.</param>
    /// <returns>The language-specific string, or <see langword="null"/>
    /// if the name/value pair does not exist in the registry.</returns>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/intl/locating-redirected-strings"/>
    public string? LoadLocalizedRedirectedString(IntPtr keyHandle, string name,
        int initialBufferCapacity = 128)
    {
        var output = new StringBuilder(initialBufferCapacity);
        
        var result = (ErrorCode)
            winApiFunctions.RegLoadMUIStringW(keyHandle,
                name, output, output.Capacity,
                out var requiredSize, 0, null);

        // ReSharper disable once InvertIf
        if (result == ErrorCode.MoreData)
        {
            output.EnsureCapacity(requiredSize);
            result = (ErrorCode)winApiFunctions.RegLoadMUIStringW(keyHandle, name, output, output.Capacity, out requiredSize,
                0, null);
        }

        return result == ErrorCode.Success ? output.ToString() : null;
    }

    [ExcludeFromCodeCoverage]
    public string GetLocalizedKeyboardLayoutName(KeyboardLayoutId layoutId)
    {
        // https://learn.microsoft.com/en-us/windows/win32/intl/using-registry-string-redirection#create-resources-for-keyboard-layout-strings
        using var key = Registry.LocalMachine.OpenSubKey($@"{KeyboardLayoutsRegistryPath}\{layoutId}");
        if (key == null)
        {
            return "Unknown";
        }
        return LoadLocalizedRedirectedString(
            key.Handle.DangerousGetHandle(), "Layout Display Name") ?? "Unknown";
    }
}