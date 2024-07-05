using SaucyRegistrations.Generators.Extensions;

namespace SaucyRegistrations.Generators.Tests.Extensions;

public class IntExtensionsTests
{
    [Fact]
    public void GetArityString_WhenArityIsZero_ReturnsEmptyString()
    {
        // Arrange
        var arity = 0;

        // Act
        var result = arity.GetArityString();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetArityString_WhenArityIsOne_ReturnsExpectedString()
    {
        // Arrange
        var arity = 1;

        // Act
        var result = arity.GetArityString();

        // Assert
        result.Should().Be("<>");
    }

    [Fact]
    public void GetArityString_WhenArityIsGreaterThanOne_ReturnsExpectedString()
    {
        // Arrange
        var arity = 2;

        // Act
        var result = arity.GetArityString();

        // Assert
        result.Should().Be("<,>");
    }
}