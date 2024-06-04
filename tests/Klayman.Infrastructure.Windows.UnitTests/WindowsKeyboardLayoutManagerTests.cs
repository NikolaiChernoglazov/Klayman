using System.Security;
using System.Text;
using FluentAssertions;
using FluentResults;
using FluentResults.Extensions.FluentAssertions;
using Klayman.Application;
using Klayman.Domain;
using Klayman.Infrastructure.Windows.WinApi;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
// ReSharper disable ConvertToConstant.Local

namespace Klayman.Infrastructure.Windows.UnitTests;

public class WindowsKeyboardLayoutManagerTests
{
    private readonly IWinApiFunctions _winApiFunctions;
    private readonly IRegistryFunctions _registryFunctions;
    private readonly ILanguageTagFunctions _languageTagFunctions;
    private readonly IKeyboardLayoutFactory _layoutFactory;
    private readonly WindowsKeyboardLayoutManager _layoutManager;
    
    public WindowsKeyboardLayoutManagerTests()
    {
        _winApiFunctions = Substitute.For<IWinApiFunctions>();
        _registryFunctions = Substitute.For<IRegistryFunctions>();
        _languageTagFunctions = Substitute.For<ILanguageTagFunctions>();
        _layoutFactory = Substitute.For<IKeyboardLayoutFactory>();
        _layoutManager = new WindowsKeyboardLayoutManager(
            _winApiFunctions, _registryFunctions, _languageTagFunctions, _layoutFactory);
    }
    
    
    [Fact]
    public void GetCurrentKeyboardLayout_GetKeyboardLayoutNameWFails_ReturnsError()
    {
        // Arrange
        var errorCode = 1;
        var expectedErrorMessage = $"Function GetKeyboardLayoutNameW returned an error {errorCode}";

        _winApiFunctions.GetKeyboardLayoutNameW(Arg.Any<StringBuilder>())
            .Returns(false);
        _winApiFunctions.GetLastWin32Error().Returns(errorCode);

        // Act
        var actualResult = _layoutManager.GetCurrentKeyboardLayout();

        // Assert
        actualResult.Should().BeFailure().And.HaveError(expectedErrorMessage);
    }
    
    [Fact]
    public void GetCurrentKeyboardLayout_NoErrors_ReturnsKeyboardLayout()
    {
        // Arrange
        var layoutId = new KeyboardLayoutId("00000000");
        var expectedLayout = new KeyboardLayout(
            layoutId, "Some layout", null);

        _winApiFunctions.GetKeyboardLayoutNameW(
                Arg.Do<StringBuilder>(
                    buffer => buffer.Insert(0, layoutId)))
            .Returns(true);
        _layoutFactory.CreateFromKeyboardLayoutId(layoutId)
            .Returns(expectedLayout);

        // Act
        var actualResult = _layoutManager.GetCurrentKeyboardLayout();

        // Assert
        actualResult.Should().BeSuccess().And.HaveValue(expectedLayout);
    }
    
    
    [Fact]
    public void GetCurrentKeyboardLayoutSet_GetKeyboardLayoutListFailsOnGettingLayoutsCount_ReturnsError()
    {
        // Arrange
        var errorCode = 1;
        var expectedErrorMessage = $"Function GetKeyboardLayoutList returned an error {errorCode}";

        _winApiFunctions.GetKeyboardLayoutList(0, null)
            .Returns(0);
        _winApiFunctions.GetLastWin32Error().Returns(errorCode);

        // Act
        var actualResult = _layoutManager.GetCurrentKeyboardLayoutSet();

        // Assert
        actualResult.Should().BeFailure().And.HaveError(expectedErrorMessage);
    }
    
    [Fact]
    public void GetCurrentKeyboardLayoutSet_GetKeyboardLayoutListFailsOnGettingLayouts_ReturnsError()
    {
        // Arrange
        var errorCode = 1;
        var expectedErrorMessage = $"Function GetKeyboardLayoutList returned an error {errorCode}";
        var layoutsCount = 1;

        _winApiFunctions.GetKeyboardLayoutList(0, null)
            .Returns(layoutsCount);
        _winApiFunctions.GetKeyboardLayoutList(layoutsCount, Arg.Any<IntPtr[]>())
            .Returns(0);
        _winApiFunctions.GetLastWin32Error().Returns(errorCode);

        // Act
        var actualResult = _layoutManager.GetCurrentKeyboardLayoutSet();

        // Assert
        actualResult.Should().BeFailure().And.HaveError(expectedErrorMessage);
    }
    
    [Fact]
    public void GetCurrentKeyboardLayoutSet_NoErrors_ReturnsKeyboardLayouts()
    {
        var layoutsCount = 1;
        var layoutHandle = new IntPtr(0);
        var layoutId = new KeyboardLayoutId("00000000");
        var layout = new KeyboardLayout(
            layoutId, string.Empty, null);
        var expectedLayouts = new List<KeyboardLayout> { layout };

        _winApiFunctions.GetKeyboardLayoutList(0, null)
            .Returns(layoutsCount);
        _winApiFunctions.GetKeyboardLayoutList(layoutsCount, Arg.Do<IntPtr[]>(
                buffer => buffer[0] = layoutHandle))
            .Returns(layoutsCount);
        _registryFunctions.FindMatchingKeyboardLayoutId(layoutHandle)
            .Returns(layoutId);
        _layoutFactory.CreateFromKeyboardLayoutId(layoutId)
            .Returns(layout);

        // Act
        var actualResult = _layoutManager.GetCurrentKeyboardLayoutSet();

        // Assert
        actualResult.Should().BeSuccess().And
            .Subject.Value.Should().BeEquivalentTo(expectedLayouts);
    }
    
    [Fact]
    public void GetCurrentKeyboardLayoutSet_RegistryThrowsSecurityException_ReturnsError()
    {
        var layoutsCount = 1;
        var layoutHandle = new IntPtr(0);
        var layoutsKeyPath = "Layouts";
        var expectedErrorMessage = $"Access to the registry key {layoutsKeyPath} is required.";

        _winApiFunctions.GetKeyboardLayoutList(0, null)
            .Returns(layoutsCount);
        _winApiFunctions.GetKeyboardLayoutList(layoutsCount, Arg.Do<IntPtr[]>(
                buffer => buffer[0] = layoutHandle))
            .Returns(layoutsCount);
        _registryFunctions.FindMatchingKeyboardLayoutId(layoutHandle)
            .Throws<SecurityException>();
        _registryFunctions.GetKeyboardLayoutRegistryKeyPath().Returns(layoutsKeyPath);

        // Act
        var actualResult = _layoutManager.GetCurrentKeyboardLayoutSet();

        // Assert
        actualResult.Should().BeFailure(expectedErrorMessage);
    }
    
    
    [Fact]
    public void GetAllAvailableKeyboardLayouts_NoErrors_ReturnsKeyboardLayouts()
    {
        // Arrange
        var layoutId = new KeyboardLayoutId("00000000");
        var layoutIds = new List<KeyboardLayoutId> { layoutId };
        var layout = new KeyboardLayout(
            layoutId, string.Empty, null);
        var expectedLayouts = new List<KeyboardLayout> { layout };

        _registryFunctions.GetPresentKeyboardLayoutIds()
            .Returns(layoutIds);
        _layoutFactory.CreateFromKeyboardLayoutId(layoutId)
            .Returns(layout);

        // Act
        var actualLayouts = _layoutManager.GetAllAvailableKeyboardLayouts();

        // Assert
        actualLayouts.Should().BeSuccess()
            .And.Subject.Value.Should().BeEquivalentTo(expectedLayouts);
    }
    
    [Fact]
    public void GetAllAvailableKeyboardLayouts_RegistryThrowsSecurityException_ReturnsError()
    {
        // Arrange
        var layoutsKeyPath = "Layouts";
        var expectedErrorMessage = $"Access to the registry key {layoutsKeyPath} is required.";

        _registryFunctions.GetPresentKeyboardLayoutIds()
            .Throws(new SecurityException());
        _registryFunctions.GetKeyboardLayoutRegistryKeyPath().Returns(layoutsKeyPath);
        

        // Act
        var actualLayouts = _layoutManager.GetAllAvailableKeyboardLayouts();

        // Assert
        actualLayouts.Should().BeFailure(expectedErrorMessage);
    }


    [Fact]
    public void AddKeyboardLayoutById_LayoutIdIsNotPresentInRegistry_ReturnsError()
    {
        // Arrange
        var layoutId = new KeyboardLayoutId("00000000");
        var layoutsKeyPath = "Layouts";
        var expectedErrorMessage =
            $"Keyboard layout with ID {layoutId} is not registered in the OS. It should be present in the " +
            $"{layoutsKeyPath} Windows Registry path.";

        _registryFunctions.GetPresentKeyboardLayoutIds()
            .Returns(new List<KeyboardLayoutId>());
        _registryFunctions.GetKeyboardLayoutRegistryKeyPath()
            .Returns(layoutsKeyPath);

        // Act
        var actualResult = _layoutManager.AddKeyboardLayoutById(layoutId);

        // Assert
        actualResult.Should().BeFailure().And.HaveError(expectedErrorMessage);
    }

    [Fact]
    public void AddKeyboardLayoutById_RegistryThrowsSecurityException_ReturnsError()
    {
        // Arrange
        var layoutId = new KeyboardLayoutId("00000000");
        var layoutsKeyPath = "Layouts";
        var expectedErrorMessage =
            $"Access to the registry key {layoutsKeyPath} is required.";

        _registryFunctions.GetPresentKeyboardLayoutIds()
            .Throws<SecurityException>();
        _registryFunctions.GetKeyboardLayoutRegistryKeyPath()
            .Returns(layoutsKeyPath);

        // Act
        var actualResult = _layoutManager.AddKeyboardLayoutById(layoutId);

        // Assert
        actualResult.Should().BeFailure().And.HaveError(expectedErrorMessage);
    }
    
    [Fact]
    public void AddKeyboardLayoutById_LoadKeyboardLayoutWFails_ReturnsError()
    {
        // Arrange
        var errorCode = 1;
        var expectedErrorMessage = $"Function LoadKeyboardLayoutW returned an error {errorCode}";
        var layoutId = new KeyboardLayoutId("00000000");

        _registryFunctions.GetPresentKeyboardLayoutIds()
            .Returns(new List<KeyboardLayoutId> { layoutId });
        _winApiFunctions.LoadKeyboardLayoutW(layoutId, Arg.Any<uint>())
            .Returns(IntPtr.Zero);
        _winApiFunctions.GetLastWin32Error().Returns(errorCode);

        // Act
        var actualResult = _layoutManager.AddKeyboardLayoutById(layoutId);

        // Assert
        actualResult.Should().BeFailure().And.HaveError(expectedErrorMessage);
    }

    [Fact]
    public void AddKeyboardLayoutById_NoErrors_ReturnsKeyboardLayout()
    {
        // Arrange
        var layoutId = new KeyboardLayoutId("00000001");
        var layoutHandle = new IntPtr(1);
        var expectedLayout = new KeyboardLayout(
            layoutId, string.Empty, null);

        _registryFunctions.GetPresentKeyboardLayoutIds()
            .Returns(new List<KeyboardLayoutId> { layoutId });
        _winApiFunctions.LoadKeyboardLayoutW(layoutId, Arg.Any<uint>())
            .Returns(layoutHandle);
        _layoutFactory.CreateFromKeyboardLayoutId(layoutId)
            .Returns(expectedLayout);

        // Act
        var actualResult = _layoutManager.AddKeyboardLayoutById(layoutId);

        // Assert
        actualResult.Should().BeSuccess().And.HaveValue(expectedLayout);
    }
    
    
    [Fact]
    public void AddKeyboardLayoutByLanguageTag_LayoutIdIsNotPresentInRegistry_ReturnsError()
    {
        // Arrange
        var languageTag = "en";
        var layoutId = new KeyboardLayoutId("00000000");
        var layoutsKeyPath = "Layouts";
        var expectedErrorMessage =
            $"Keyboard layout with ID {layoutId} is not registered in the OS. It should be present in the " +
            $"{layoutsKeyPath} Windows Registry path.";

        _languageTagFunctions.GetMatchingKeyboardLayoutId(languageTag)
            .Returns(Result.Ok(layoutId));
        _registryFunctions.GetPresentKeyboardLayoutIds()
            .Returns(new List<KeyboardLayoutId>());
        _registryFunctions.GetKeyboardLayoutRegistryKeyPath()
            .Returns(layoutsKeyPath);

        // Act
        var actualResult = _layoutManager.AddKeyboardLayoutByLanguageTag(languageTag);

        // Assert
        actualResult.Should().BeFailure().And.HaveError(expectedErrorMessage);
    }

    [Fact]
    public void AddKeyboardLayoutByLanguageTag_RegistryThrowsSecurityException_ReturnsError()
    {
        // Arrange
        var languageTag = "en";
        var layoutId = new KeyboardLayoutId("00000000");
        var layoutsKeyPath = "Layouts";
        var expectedErrorMessage =
            $"Access to the registry key {layoutsKeyPath} is required.";

        _languageTagFunctions.GetMatchingKeyboardLayoutId(languageTag)
            .Returns(Result.Ok(layoutId));
        _registryFunctions.GetPresentKeyboardLayoutIds()
            .Throws<SecurityException>();
        _registryFunctions.GetKeyboardLayoutRegistryKeyPath()
            .Returns(layoutsKeyPath);

        // Act
        var actualResult = _layoutManager.AddKeyboardLayoutByLanguageTag(languageTag);

        // Assert
        actualResult.Should().BeFailure().And.HaveError(expectedErrorMessage);
    }
    
    [Fact]
    public void AddKeyboardLayoutByLanguageTag_LoadKeyboardLayoutWFails_ReturnsError()
    {
        // Arrange
        var languageTag = "en";
        var errorCode = 1;
        var expectedErrorMessage = $"Function LoadKeyboardLayoutW returned an error {errorCode}";
        var layoutId = new KeyboardLayoutId("00000000");

        _languageTagFunctions.GetMatchingKeyboardLayoutId(languageTag)
            .Returns(Result.Ok(layoutId));
        _registryFunctions.GetPresentKeyboardLayoutIds()
            .Returns(new List<KeyboardLayoutId> { layoutId });
        _winApiFunctions.LoadKeyboardLayoutW(layoutId, Arg.Any<uint>())
            .Returns(IntPtr.Zero);
        _winApiFunctions.GetLastWin32Error().Returns(errorCode);

        // Act
        var actualResult = _layoutManager.AddKeyboardLayoutByLanguageTag(languageTag);

        // Assert
        actualResult.Should().BeFailure().And.HaveError(expectedErrorMessage);
    }

    [Fact]
    public void AddKeyboardLayoutByLanguageTag_NoErrors_ReturnsKeyboardLayout()
    {
        // Arrange
        var languageTag = "en";
        var layoutId = new KeyboardLayoutId("00000001");
        var layoutHandle = new IntPtr(1);
        var expectedLayout = new KeyboardLayout(
            layoutId, string.Empty, null);

        _languageTagFunctions.GetMatchingKeyboardLayoutId(languageTag)
            .Returns(Result.Ok(layoutId));
        _registryFunctions.GetPresentKeyboardLayoutIds()
            .Returns(new List<KeyboardLayoutId> { layoutId });
        _winApiFunctions.LoadKeyboardLayoutW(layoutId, Arg.Any<uint>())
            .Returns(layoutHandle);
        _layoutFactory.CreateFromKeyboardLayoutId(layoutId)
            .Returns(expectedLayout);

        // Act
        var actualResult = _layoutManager.AddKeyboardLayoutByLanguageTag(languageTag);

        // Assert
        actualResult.Should().BeSuccess().And.HaveValue(expectedLayout);
    }
    
    
    [Fact]
    public void RemoveKeyboardLayoutById_GetKeyboardLayoutListFailsOnGettingLayoutsCount_ReturnsError()
    {
        // Arrange
        var layoutId = new KeyboardLayoutId("00000000");
        var explanationMessage =  "To remove a keyboard layout, we need to retrieve the " +
                                  "current keyboard layout set first. But that operation failed. ";
        var errorCode = 1;
        var expectedErrorMessage = $"{explanationMessage}Function GetKeyboardLayoutList returned an error {errorCode}";

        _winApiFunctions.GetKeyboardLayoutList(0, null)
            .Returns(0);
        _winApiFunctions.GetLastWin32Error().Returns(errorCode);

        // Act
        var actualResult = _layoutManager.RemoveKeyboardLayoutById(layoutId);

        // Assert
        actualResult.Should().BeFailure().And.HaveError(expectedErrorMessage);
    }
    
    [Fact]
    public void RemoveKeyboardLayoutById_GetKeyboardLayoutListFailsOnGettingLayouts_ReturnsError()
    {
        // Arrange
        var layoutId = new KeyboardLayoutId("00000000");
        var explanationMessage =  "To remove a keyboard layout, we need to retrieve the " +
                                  "current keyboard layout set first. But that operation failed. ";
        var errorCode = 1;
        var expectedErrorMessage = $"{explanationMessage}Function GetKeyboardLayoutList returned an error {errorCode}";
        var layoutsCount = 1;

        _winApiFunctions.GetKeyboardLayoutList(0, null)
            .Returns(layoutsCount);
        _winApiFunctions.GetKeyboardLayoutList(layoutsCount, Arg.Any<IntPtr[]>())
            .Returns(0);
        _winApiFunctions.GetLastWin32Error().Returns(errorCode);

        // Act
        var actualResult = _layoutManager.RemoveKeyboardLayoutById(layoutId);

        // Assert
        actualResult.Should().BeFailure().And.HaveError(expectedErrorMessage);
    }
    
    [Fact]
    public void RemoveKeyboardLayoutById_LayoutIsNotInCurrentSet_ReturnsError()
    {
        var layoutsCount = 1;
        var layoutHandle = new IntPtr(0);
        var layoutId = new KeyboardLayoutId("00000000");
        var expectedErrorMessage =
            $"Keyboard layout with ID {layoutId} is not present in the current keyboard layout set.";

        _winApiFunctions.GetKeyboardLayoutList(0, null)
            .Returns(layoutsCount);
        _winApiFunctions.GetKeyboardLayoutList(layoutsCount, Arg.Do<IntPtr[]>(
                buffer => buffer[0] = layoutHandle))
            .Returns(layoutsCount);
        _registryFunctions.FindMatchingKeyboardLayoutId(layoutHandle)
            .Returns(new KeyboardLayoutId("00000001"));
      
        // Act
        var actualResult = _layoutManager.RemoveKeyboardLayoutById(layoutId);

        // Assert
        actualResult.Should().BeFailure().And.HaveError(expectedErrorMessage);
    }

    [Fact]
    public void RemoveKeyboardLayoutById_RegistryThrowsSecurityException_ReturnsError()
    {
        var layoutsCount = 1;
        var layoutHandle = new IntPtr(0);
        var layoutId = new KeyboardLayoutId("00000000");
        var layoutsKeyPath = "Layouts";
        var expectedErrorMessage = $"Access to the registry key {layoutsKeyPath} is required.";

        _winApiFunctions.GetKeyboardLayoutList(0, null)
            .Returns(layoutsCount);
        _winApiFunctions.GetKeyboardLayoutList(layoutsCount, Arg.Do<IntPtr[]>(
                buffer => buffer[0] = layoutHandle))
            .Returns(layoutsCount);
        _registryFunctions.FindMatchingKeyboardLayoutId(layoutHandle)
            .Throws<SecurityException>();
        _registryFunctions.GetKeyboardLayoutRegistryKeyPath().Returns(layoutsKeyPath);

        // Act
        var actualResult = _layoutManager.RemoveKeyboardLayoutById(layoutId);

        // Assert
        actualResult.Should().BeFailure(expectedErrorMessage);
    }

    [Fact]
    public void RemoveKeyboardLayoutById_UnloadKeyboardLayoutReturnsError_ReturnsError()
    {
        var layoutsCount = 1;
        var layoutHandle = new IntPtr(0);
        var layoutId = new KeyboardLayoutId("00000000");
        var errorCode = 1;
        var expectedErrorMessage = $"Function UnloadKeyboardLayout returned an error {errorCode}";

        _winApiFunctions.GetKeyboardLayoutList(0, null)
            .Returns(layoutsCount);
        _winApiFunctions.GetKeyboardLayoutList(layoutsCount, Arg.Do<IntPtr[]>(
                buffer => buffer[0] = layoutHandle))
            .Returns(layoutsCount);
        _registryFunctions.FindMatchingKeyboardLayoutId(layoutHandle)
            .Returns(layoutId);
        _winApiFunctions.UnloadKeyboardLayout(layoutHandle)
            .Returns(false);    
        _winApiFunctions.GetLastWin32Error().Returns(errorCode);
      
        // Act
        var actualResult = _layoutManager.RemoveKeyboardLayoutById(layoutId);

        // Assert
        actualResult.Should().BeFailure().And.HaveError(expectedErrorMessage);
    }
    
    [Fact]
    public void RemoveKeyboardLayoutById_NoErrors_ReturnsKeyboardLayout()
    {
        var layoutsCount = 1;
        var layoutHandle = new IntPtr(0);
        var layoutId = new KeyboardLayoutId("00000000");
        var expectedLayout = new KeyboardLayout(
            layoutId, null, null);

        _winApiFunctions.GetKeyboardLayoutList(0, null)
            .Returns(layoutsCount);
        _winApiFunctions.GetKeyboardLayoutList(layoutsCount, Arg.Do<IntPtr[]>(
                buffer => buffer[0] = layoutHandle))
            .Returns(layoutsCount);
        _registryFunctions.FindMatchingKeyboardLayoutId(layoutHandle)
            .Returns(layoutId);
        _winApiFunctions.UnloadKeyboardLayout(layoutHandle)
            .Returns(true);
        _layoutFactory.CreateFromKeyboardLayoutId(layoutId)
            .Returns(expectedLayout);
        
        // Act
        var actualResult = _layoutManager.RemoveKeyboardLayoutById(layoutId);

        // Assert
        actualResult.Should().BeSuccess().And.HaveValue(expectedLayout);
    }
}