using FluentAssertions;
using Klayman.Domain;
using NSubstitute;
// ReSharper disable ConvertToConstant.Local

namespace Klayman.Infrastructure.Windows.UnitTests;

public class WindowsKeyboardLayoutNameProviderTests
{
    private readonly IRegistryFunctions _registryFunctions;
    private readonly WindowsKeyboardLayoutNameProvider _layoutNameProvider;
    
    public WindowsKeyboardLayoutNameProviderTests()
    {
        _registryFunctions = Substitute.For<IRegistryFunctions>();
        _layoutNameProvider = new WindowsKeyboardLayoutNameProvider(_registryFunctions);
    }
    
    [Fact]
    public void GetKeyboardLayoutName_ReturnsNameFromRegistry()
    {
        // Arrange
        var expectedLayoutName = "Some layout";

        _registryFunctions.GetLocalizedKeyboardLayoutName(
            Arg.Any<KeyboardLayoutId>()).Returns(expectedLayoutName);

        // Act
        var actualLayoutName = _layoutNameProvider.GetKeyboardLayoutName(
            new KeyboardLayoutId("00000000"));
        
        // Assert
        actualLayoutName.Should().Be(expectedLayoutName);
    }
}