using System.Globalization;
using FluentAssertions;
using Klayman.Domain;
using NSubstitute;
// ReSharper disable ConvertToConstant.Local

namespace Klayman.Application.UnitTests;

public class KeyboardLayoutFactoryTests
{
    private readonly IKeyboardLayoutNameProvider _layoutNameProvider;
    private readonly KeyboardLayoutFactory _layoutFactory;

    public KeyboardLayoutFactoryTests()
    {
        _layoutNameProvider = Substitute.For<IKeyboardLayoutNameProvider>();
        _layoutFactory = new KeyboardLayoutFactory(_layoutNameProvider);
    }
    
    [Fact]
    public void CreateFromKeyboardLayoutId_CorrespondingCultureIsFound_CreatesKeyboardLayoutWithCulture()
    {
        // Arrange
        var layoutId = new KeyboardLayoutId("0000040C");
        var layoutName = "French";
        var culture = new CultureInfo("fr-FR");

        var expectedKeyboardLayout = new KeyboardLayout(layoutId, layoutName, culture.Name);
        
        _layoutNameProvider.GetKeyboardLayoutName(layoutId).Returns(layoutName);
        
        // Act
        var actualKeyboardLayout = _layoutFactory.CreateFromKeyboardLayoutId(layoutId);
        
        // Assert
        actualKeyboardLayout.Should().Be(expectedKeyboardLayout);
    }
    
    [Fact]
    public void CreateFromKeyboardLayoutId_CorrespondingCultureIsNotFound_CreatesKeyboardLayoutWithoutCulture()
    {
        // Arrange
        var layoutId = new KeyboardLayoutId("00000000");
        var layoutName = "Some layout";

        var expectedKeyboardLayout = new KeyboardLayout(layoutId, layoutName, null);
        
        _layoutNameProvider.GetKeyboardLayoutName(layoutId).Returns(layoutName);
        
        // Act
        var actualKeyboardLayout = _layoutFactory.CreateFromKeyboardLayoutId(layoutId);
        
        // Assert
        actualKeyboardLayout.Should().Be(expectedKeyboardLayout);
    }
}