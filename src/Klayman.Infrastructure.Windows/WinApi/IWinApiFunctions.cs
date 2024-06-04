using System.Runtime.InteropServices;
using System.Text;
// ReSharper disable InconsistentNaming

namespace Klayman.Infrastructure.Windows.WinApi;

public interface IWinApiFunctions
{
    /// <summary>
    /// Get the last platform invoke error on the current thread
    /// </summary>
    /// <returns>The last platform invoke error</returns>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/errhandlingapi/nf-errhandlingapi-getlasterror"/>
    int GetLastWin32Error();
    
    /// <summary>
    /// Loads the specified string from the specified key and subkey.
    /// </summary>
    /// <param name="hKey">A handle to an open registry key.</param>
    /// <param name="pszValue">The name of the registry value.</param>
    /// <param name="pszOutBuf">A buffer that receives the string.</param>
    /// <param name="cbOutBuf">The size of the pszOutBuf buffer, in bytes.</param>
    /// <param name="pcbData">
    /// A pointer to a variable that receives the size of the data copied to the pszOutBuf buffer, in bytes.
    /// </param>
    /// <param name="flags">Flags.</param>
    /// <param name="pszDirectory">The directory path.</param>
    /// <returns>
    /// If the function succeeds, the return value is <see cref="ErrorCode.Success"/>.
    /// If the function fails, the return value is a system error code.
    /// If the pcbData buffer is too small to receive the string, the function returns <see cref="ErrorCode.MoreData"/>.
    /// </returns>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winreg/nf-winreg-regloadmuistringw"/>
    int RegLoadMUIStringW(
        IntPtr hKey,
        string pszValue,
        StringBuilder pszOutBuf,
        int cbOutBuf,
        out int pcbData,
        uint flags,
        string? pszDirectory);
    
    /// <summary>
    /// Retrieves the identifier of the active keyboard layout (KLID)
    /// for the calling thread.
    /// </summary>
    /// <param name="pwszKLID">The buffer that receives the KLID.</param>
    /// <returns>
    /// If the function succeeds, the return value is <see langword="true"/>.
    /// If the function fails, the return value is <see langword="false"/>.
    /// To get extended error information, call <see cref="GetLastWin32Error"/>.
    /// </returns>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getkeyboardlayoutnamew"/>
    bool GetKeyboardLayoutNameW(
        StringBuilder pwszKLID);
    
    /// <summary>
    /// Retrieves the keyboard layout handles (HKLs) corresponding to the current
    /// set of input locales in the system.
    /// The function copies the handles to the specified buffer.
    /// </summary>
    /// <param name="nBuff">The maximum number of handles that the buffer can hold.</param>
    /// <param name="lpList">The buffer that receives the array of keyboard layout handles.</param>
    /// <returns>
    /// If the function succeeds, the return value is the number of keyboard layout handles copied to the buffer or,
    /// if nBuff is zero, the return value is the size, in array elements, of the buffer needed to receive all
    /// current keyboard layout handles.
    /// If the function fails, the return value is zero.
    /// To get extended error information, call <see cref="GetLastWin32Error"/>.
    /// </returns>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getkeyboardlayoutlist"/>
    int GetKeyboardLayoutList(
        int nBuff,
        [Out] IntPtr[]? lpList);
    
    /// <summary>
    /// Loads a new keyboard layout into the system.
    /// </summary>
    /// <param name="pwszKLID">The keyboard identifier to load.</param>
    /// <param name="flags">Specifies how the input locale identifier is to be loaded. </param>
    /// <returns>
    /// If the function succeeds, the return value is the input locale identifier.
    /// If no matching locale is available, the return value is the default language of the system.
    /// If the function fails, the return value is zero.
    /// To get extended error information, call <see cref="GetLastWin32Error"/>.
    /// </returns>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-loadkeyboardlayoutw"/>
    IntPtr LoadKeyboardLayoutW(
        string pwszKLID,
        uint flags);
    
    /// <summary>
    /// Unloads a keyboard layout.
    /// </summary>
    /// <param name="hkl">A handle of the keyboard layout to be unloaded.</param>
    /// <returns>
    /// If the function succeeds, the return value is <see langword="true"/>.
    /// If the function fails, the return value is <see langword="false"/>.
    /// To get extended error information, call <see cref="GetLastWin32Error"/>.
    /// </returns>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-unloadkeyboardlayout"/>
    bool UnloadKeyboardLayout(
        IntPtr hkl
    );
}