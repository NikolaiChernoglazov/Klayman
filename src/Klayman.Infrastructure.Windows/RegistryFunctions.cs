using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.Security;
using System.Text;
using Klayman.Domain;
using Klayman.Infrastructure.Windows.Extensions;
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

    public string GetKeyboardLayoutRegistryKeyPath()
    {
        return KeyboardLayoutsRegistryPath;
    }

    [ExcludeFromCodeCoverage]
    public string? GetLocalizedKeyboardLayoutName(KeyboardLayoutId layoutId)
    {
        // https://learn.microsoft.com/en-us/windows/win32/intl/using-registry-string-redirection#create-resources-for-keyboard-layout-strings
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey($@"{KeyboardLayoutsRegistryPath}\{layoutId}");
            if (key is null)
            {
                return "Unknown";
            }

            return LoadLocalizedRedirectedString(
                key.Handle.DangerousGetHandle(), "Layout Display Name");
        }
        catch (SecurityException)
        {
            return null;
        }
    }
    
    [ExcludeFromCodeCoverage]
    public IEnumerable<KeyboardLayoutId> GetPresentKeyboardLayoutIds()
    {
        using var key = Registry.LocalMachine.OpenSubKey(KeyboardLayoutsRegistryPath);
        return key?.GetSubKeyNames().Select(k => new KeyboardLayoutId(k)) ?? [];
    }
    
    [ExcludeFromCodeCoverage]
    public KeyboardLayoutId FindMatchingKeyboardLayoutId(IntPtr layoutHandle)
    {
        // This method implementation is taken from the System.Windows.Forms.InputLanguage class
        
        // There is no good way to do this in Windows. GetKeyboardLayoutName does what we want, but only for the
        // current input language; setting and resetting the current input language would generate spurious
        // InputLanguageChanged events. Try to extract needed information manually.
        
        // High word of HKL contains a device handle to the physical layout of the keyboard but exact format of this
        // handle is not documented. For older keyboard layouts device handle seems contains keyboard layout
        // identifier.
        int deviceId = layoutHandle.HiWord();

        // But for newer keyboard layouts device handle contains special layout id if its high nibble is 0xF. This
        // id may be used to search for keyboard layout under registry.
        // NOTE: this logic may break in future versions of Windows since it is not documented.
        if ((deviceId & 0xF000) == 0xF000)
        {
            // Extract special layout id from the device handle
            var layoutId = deviceId & 0x0FFF;

            using var key = Registry.LocalMachine.OpenSubKey(KeyboardLayoutsRegistryPath);
            if (key is null)
                return new KeyboardLayoutId(deviceId);
            // Match keyboard layout by layout id
            foreach (var subKeyName in key.GetSubKeyNames())
            {
                using var subKey = key.OpenSubKey(subKeyName);
                if (subKey?.GetValue("Layout Id") is not string subKeyLayoutId
                    || Convert.ToInt32(subKeyLayoutId, 16) != layoutId) continue;
                Debug.Assert(subKeyName.Length == 8, $"unexpected key length in registry: {subKey.Name}");
                return new KeyboardLayoutId(subKeyName);
            }
        }
        else
        {
            // Use input language only if keyboard layout language is not available. This is crucial in cases when
            // keyboard is installed more than once or under different languages. For example when French keyboard
            // is installed under US input language we need to return French keyboard identifier.
            if (deviceId == 0)
            {
                // According to the GetKeyboardLayout API function docs low word of HKL contains input language.
                deviceId = layoutHandle.LoWord();
            }
        }

        return new KeyboardLayoutId(deviceId);
    }
}