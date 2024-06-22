using FluentAssertions;
// ReSharper disable ConvertToConstant.Local

namespace Klayman.Domain.UnitTests;

public class KeyboardLayoutIdTests
{
    [Fact]
    public void ConstructorFromString_NotValidKeyboardLayoutId_ThrowsArgumentNullException()
    {
        // Arrange
        var layoutId = "0";
        var expectedErrorMessage = $"{layoutId} is not a valid KLID.";
        Action creationAction = () => _ = new KeyboardLayoutId(layoutId);
        
        // Act & Assert
        creationAction.Should().Throw<ArgumentException>(expectedErrorMessage);
    }
    
    [Fact]
    public void ConstructorFromString_ValidKeyboardLayoutId_Passes()
    {
        // Arrange
        var layoutId = "00000000";
        KeyboardLayoutId? createdLayoutId = null;
        Action creationAction = () => createdLayoutId = new KeyboardLayoutId(layoutId);
        
        // Act & Assert
        creationAction.Should().NotThrow();
        createdLayoutId?.ToString().Should().Be(layoutId);
    }
    
    
    [Theory]
    [InlineData("0123ABCD", 0xABCD)]
    [InlineData("ABCD0123", 0x0123)]
    [InlineData("01230000", 0x0000)]
    [InlineData("0123FFFF", 0xFFFF)]
    public void GetLanguageId_ReturnsExpectedValue(
        string layoutId, int expectedLanguageId)
    {
        // Arrange
        var actualLanguageId = new KeyboardLayoutId(layoutId).GetLanguageId();
        
        // Assert
        actualLanguageId.Should().Be(expectedLanguageId);
    }
    
    
    [Theory]
    [InlineData("00000000")]
    [InlineData("01234567")]
    [InlineData("01ABCDEF")]
    [InlineData("01abcdef")]
    public void IsValidKeyboardLayoutId_ValidId_ReturnsTrue(string layoutId)
    {
        // Act
        var actualValue = KeyboardLayoutId.IsValid(layoutId);
        
        // Assert
        actualValue.Should().BeTrue();
    }
    
    [Theory]
    [InlineData("0123456789")]
    [InlineData("0123")]
    [InlineData("0ABCDEFG")]
    [InlineData("0")]
    public void IsValidKeyboardLayoutId_NotValidId_ReturnsFalse(string layoutId)
    {
        // Act
        var actualValue = KeyboardLayoutId.IsValid(layoutId);
        
        // Assert
        actualValue.Should().BeFalse();
    }
}