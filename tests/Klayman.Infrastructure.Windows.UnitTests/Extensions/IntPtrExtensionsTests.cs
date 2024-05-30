using FluentAssertions;
using Klayman.Infrastructure.Windows.Extensions;

namespace Klayman.Infrastructure.Windows.UnitTests.Extensions;

public class IntPtrExtensionsTests
{
    // We are using  hex strings to represent integers in their byte representation (including sign bit)
    // and to see the byte representation in a debugger.
    // We cannot use hex numerical constants, because they are interpreted aa a positive signed integer value
    
    [Theory]
    [InlineData("FFFFFFFF", "FFFF")]
    [InlineData("80000000", "8000")]
    [InlineData("00000000", "0000")]
    [InlineData("12345678", "1234")]
    public void HiWord_ReturnsExpectedValue(
        string value, string expectedValue)
    {
        // Act
        var actualValue = new IntPtr(Convert.ToInt32(value, 16))
            .HiWord()
            .ToString("X4");
        
        // Assert
        actualValue.Should().Be(expectedValue);
    }
    
    [Theory]
    [InlineData("FFFFFFFF", "FFFF")]
    [InlineData("80000000", "0000")]
    [InlineData("00000000", "0000")]
    [InlineData("12345678", "5678")]
    public void LoWord_ReturnsExpectedValue(
        string value, string expectedValue)
    {
        // Act
        var actualValue = new IntPtr(Convert.ToInt32(value, 16))
            .LoWord()
            .ToString("X4");
        
        // Assert
        actualValue.Should().Be(expectedValue);
    }
}