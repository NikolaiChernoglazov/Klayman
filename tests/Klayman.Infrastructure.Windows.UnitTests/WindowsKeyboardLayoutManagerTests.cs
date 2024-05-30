using System.Text;
using FluentResults.Extensions.FluentAssertions;
using Klayman.Application;
using Klayman.Domain;
using Klayman.Infrastructure.Windows.WinApi;
using NSubstitute;
// ReSharper disable ConvertToConstant.Local

namespace Klayman.Infrastructure.Windows.UnitTests;
 
public class WindowsKeyboardLayoutManagerTests
{
    private readonly IWinApiFunctions _winApiFunctions;
    private readonly IKeyboardLayoutFactory _layoutFactory;
    private readonly WindowsKeyboardLayoutManager _layoutManager;
    
    public WindowsKeyboardLayoutManagerTests()
    {
        _winApiFunctions = Substitute.For<IWinApiFunctions>();
        _layoutFactory = Substitute.For<IKeyboardLayoutFactory>();
        _layoutManager = new WindowsKeyboardLayoutManager(
            _winApiFunctions, _layoutFactory);
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
    public void GetCurrentKeyboardLayout_GetKeyboardLayoutNameWSucceeds_ReturnsKeyboardLayout()
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
}