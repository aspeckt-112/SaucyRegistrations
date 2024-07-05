using SaucyRegistrations.Generators.Helpers;

namespace SaucyRegistrations.Generators.Tests.Helpers;

public class StringHelpersTests
{
    [Fact]
    public void Normalize_ReplacesAllOccurrencesOfCrLfWithLf()
    {
        // Arrange
        var input = "Hello\r\nWorld\r\n";

        // Act
        var result = StringHelpers.Normalize(input);

        // Assert
        result.Should().Be("Hello\nWorld\n");
    }

    [Fact]
    public void Normalize_ReplacesAllOccurrencesOfCrLfWithLf_WhenInputContainsMultipleOccurrences()
    {
        // Arrange
        var input = "Hello\r\nWorld\r\n\r\n";

        // Act
        var result = StringHelpers.Normalize(input);

        // Assert
        result.Should().Be("Hello\nWorld\n\n");
    }

    [Fact]
    public void Normalize_ReplacesAllOccurrencesOfCrLfWithLf_WhenInputContainsNoOccurrences()
    {
        // Arrange
        var input = "Hello World";

        // Act
        var result = StringHelpers.Normalize(input);

        // Assert
        result.Should().Be("Hello World");
    }

    [Fact]
    public void Normalize_ReplacesAllOccurrencesOfCrLfWithLf_WhenInputIsEmpty()
    {
        // Arrange
        var input = string.Empty;

        // Act
        var result = StringHelpers.Normalize(input);

        // Assert
        result.Should().Be(string.Empty);
    }
}