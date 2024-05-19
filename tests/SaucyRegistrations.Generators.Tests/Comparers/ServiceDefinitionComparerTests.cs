using SaucyRegistrations.Generators.Comparers;
using SaucyRegistrations.Generators.Models;

namespace SaucyRegistrations.Generators.Tests.Comparers;

public class ServiceDefinitionComparerTests
{
    private readonly ServiceDefinitionComparer _comparer = new();

    [Fact]
    public void Equals_ReturnsTrue_WhenFullyQualifiedClassNamesMatch()
    {
        // Arrange
        var x = new ServiceDefinition("Fully.Qualified.ClassName", null, null);
        var y = new ServiceDefinition("Fully.Qualified.ClassName", null, null);

        // Act
        var result = _comparer.Equals(x, y);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenFullyQualifiedClassNamesDoNotMatch()
    {
        // Arrange
        var x = new ServiceDefinition("Fully.Qualified.ClassName", null, null);
        var y = new ServiceDefinition("Different.Fully.Qualified.ClassName", null, null);

        // Act
        var result = _comparer.Equals(x, y);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_ReturnsHashCodeOfFullyQualifiedClassName()
    {
        // Arrange
        var serviceDefinition = new ServiceDefinition("Fully.Qualified.ClassName", null, null);

        // Act
        var result = _comparer.GetHashCode(serviceDefinition);

        // Assert
        result.Should().Be(serviceDefinition.FullyQualifiedClassName.GetHashCode());
    }
}