using SaucyRegistrations.Generators.Builders;

namespace SaucyRegistrations.Generators.Tests.Builders;

public class AttributeDefinitionBuilderTests
{
    private readonly AttributeDefinitionBuilder _builder = new();

    [Fact]
    public void AppendAttributeDefinition_When_SaucyIncludeAttributeDefinition_Then_ReturnsCorrectAttributeDefinition()
    {
        // Act
        _builder.AppendAttributeDefinition(SaucyInclude.SaucyIncludeAttributeDefinition);
        var result = _builder.ToString();

        // Assert
        result.Should().BeEquivalentTo($"{SaucyInclude.SaucyIncludeAttributeDefinition}\n\n");
    }

    [Fact]
    public void AppendAttributeDefinition_When_MultipleAttributeDefinitions_Then_ReturnsCorrectAttributeDefinitions()
    {
        // Act
        _builder.AppendAttributeDefinition(SaucyInclude.SaucyIncludeAttributeDefinition);
        _builder.AppendAttributeDefinition(SaucyInclude.SaucyIncludeAttributeDefinition);
        var result = _builder.ToString();

        // Assert
        result.Should().BeEquivalentTo($"{SaucyInclude.SaucyIncludeAttributeDefinition}\n\n{SaucyInclude.SaucyIncludeAttributeDefinition}\n\n");
    }
}