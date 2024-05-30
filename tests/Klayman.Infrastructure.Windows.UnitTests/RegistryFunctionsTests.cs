using System.Text;
using FluentAssertions;
using Klayman.Infrastructure.Windows.WinApi;
using NSubstitute;
// ReSharper disable ConvertToConstant.Local

namespace Klayman.Infrastructure.Windows.UnitTests;

#pragma warning disable CA1416
public class RegistryFunctionsTests
{
    private readonly IWinApiFunctions _winApiFunctions;
    private readonly RegistryFunctions _registryFunctions;
    
    public RegistryFunctionsTests()
    {
        _winApiFunctions = Substitute.For<IWinApiFunctions>();
        _registryFunctions = new RegistryFunctions(_winApiFunctions);
    }
    
    [Fact]
    public void LoadLocalizedRedirectedString_RegLoadMUIStringReturnsNotMoreDataOrSuccess_ReturnsNull()
    {
        // Arrange
        var keyHandle = IntPtr.MaxValue;
        var name = "Layout Text";

        _winApiFunctions.RegLoadMUIStringW(
            keyHandle, name, Arg.Any<StringBuilder>(),
            Arg.Any<int>(), out Arg.Any<int>(), Arg.Any<uint>(), Arg.Any<string>()
        ).Returns(1);

        // Act
        var actualString = _registryFunctions.LoadLocalizedRedirectedString(
            keyHandle, name);
    
        // Assert
        actualString.Should().BeNull();
    }
    
    [Fact]
    public void LoadLocalizedRedirectedString_RegLoadMUIStringReturnsSuccess_ReturnsString()
    {
        // Arrange
        var keyHandle = IntPtr.MaxValue;
        var name = "Layout Text";
        var expectedString = "Some Layout";

        _winApiFunctions.RegLoadMUIStringW(
            keyHandle, name, Arg.Do<StringBuilder>(
                buffer => buffer.Insert(0, expectedString)),
            Arg.Any<int>(), out Arg.Any<int>(), Arg.Any<uint>(), Arg.Any<string>()
        ).Returns(x =>
        {
            x[4] = expectedString.Length;
            return (int)ErrorCode.Success;
        });

        // Act
        var actualString = _registryFunctions.LoadLocalizedRedirectedString(
            keyHandle, name);
    
        // Assert
        actualString.Should().Be(expectedString);
    }
    
    [Fact]
    public void LoadLocalizedRedirectedString_RegLoadMUIStringReturnsMoreData_CallsFunctionWithUpdatedBufferCapacityAndndReturnsString()
    {
        // Arrange
        var keyHandle = IntPtr.MaxValue;
        var name = "Layout Text";
        var initialBufferCapacity = 2;
        var expectedBufferCapacity = 128;
        var expectedString = "Some Layout";

        _winApiFunctions.RegLoadMUIStringW(
            keyHandle, name, Arg.Any<StringBuilder>(),
            initialBufferCapacity, out Arg.Any<int>(), Arg.Any<uint>(), Arg.Any<string>()
        ).Returns(x =>
        {
            x[4] = expectedBufferCapacity;
            return (int)ErrorCode.MoreData;
        });
        _winApiFunctions.RegLoadMUIStringW(
            keyHandle, name, Arg.Do<StringBuilder>(
                buffer => buffer.Insert(0, expectedString)),
            expectedBufferCapacity, out Arg.Any<int>(), Arg.Any<uint>(), Arg.Any<string>()
        ).Returns(x =>
        {
            x[4] = expectedBufferCapacity;
            return (int)ErrorCode.Success;
        });

        // Act
        var actualString = _registryFunctions.LoadLocalizedRedirectedString(
            keyHandle, name, initialBufferCapacity);
    
        // Assert
        actualString.Should().Be(expectedString);
    }
}