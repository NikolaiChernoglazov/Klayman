using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
// ReSharper disable InconsistentNaming

namespace Klayman.Infrastructure.Windows.WinApi;

[ExcludeFromCodeCoverage]
[SupportedOSPlatform("windows")]
internal static partial class WinApiFunctionImports
{
    [DllImport("advapi32.dll",
        CharSet = CharSet.Unicode)]
    public static extern int RegLoadMUIStringW(
        IntPtr hKey,
        string pszValue,
        StringBuilder pszOutBuf,
        int cbOutBuf,
        out int pcbData,
        uint flags,
        string? pszDirectory);
    
    [DllImport("user32.dll",
        CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetKeyboardLayoutNameW(
        [Out] StringBuilder pwszKLID);
    
    [LibraryImport("user32.dll")]
    public static partial int GetKeyboardLayoutList(
        int nBuff,
        [Out] IntPtr[]? lpList);
    
    [LibraryImport("user32.dll",
        StringMarshalling = StringMarshalling.Utf16)]
    public static partial IntPtr LoadKeyboardLayoutW(
        string pwszKLID,
        uint flags);
    
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool UnloadKeyboardLayout(
        IntPtr hkl
    );
}