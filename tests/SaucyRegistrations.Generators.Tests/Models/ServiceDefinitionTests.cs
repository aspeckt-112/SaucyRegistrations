using SaucyRegistrations.Generators.Models;
using SaucyRegistrations.Generators.Models.Contracts;

namespace SaucyRegistrations.Generators.Tests.Models;

public class ServiceDefinitionTests
{
    [Fact]
    public void HasContracts_WhenContractDefinitionsIsNull_ReturnsFalse()
    {
        // Arrange
        var serviceDefinition = new ServiceDefinition("SomeClass", 0, null, null);

        // Act
        var result = serviceDefinition.HasContracts;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasContracts_WhenContractDefinitionsIsEmpty_ReturnsFalse()
    {
        // Arrange
        var serviceDefinition = new ServiceDefinition("SomeClass", 0, new List<ContractDefinition>(), null);

        // Act
        var result = serviceDefinition.HasContracts;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasContracts_WhenContractDefinitionsIsNotEmpty_ReturnsTrue()
    {
        // Arrange
        var contractDefinition = new ContractDefinition("Some_Type_Name");
        var serviceDefinition = new ServiceDefinition("SomeClass", 0, new List<ContractDefinition> { contractDefinition }, null);

        // Act
        var result = serviceDefinition.HasContracts;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsKeyed_WhenKeyIsNull_ReturnsFalse()
    {
        // Arrange
        var serviceDefinition = new ServiceDefinition("SomeClass", 0, null, null);

        // Act
        var result = serviceDefinition.IsKeyed;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsKeyed_WhenKeyIsEmpty_ReturnsFalse()
    {
        // Arrange
        var serviceDefinition = new ServiceDefinition("SomeClass", 0, null, string.Empty);

        // Act
        var result = serviceDefinition.IsKeyed;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsKeyed_WhenKeyIsNotEmpty_ReturnsTrue()
    {
        // Arrange
        var serviceDefinition = new ServiceDefinition("SomeClass", 0, null, "SomeKey");

        // Act
        var result = serviceDefinition.IsKeyed;

        // Assert
        result.Should().BeTrue();
    }
}