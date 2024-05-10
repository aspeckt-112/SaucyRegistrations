using System.Text;

namespace SaucyRegistrations.Generators.Builders;

/// <summary>
/// A class that builds attribute definitions.
/// </summary>
public class AttributeDefinitionBuilder
{
    private readonly StringBuilder _sb = new();

    /// <inheritdoc />
    public override string ToString() => _sb.ToString();

    /// <summary>
    /// Appends an attribute definition to the source code.
    /// </summary>
    /// <param name="attributeDefinition">The attribute definition to append.</param>
    /// <returns>The current instance of the <see cref="AttributeDefinitionBuilder"/>.</returns>
    public AttributeDefinitionBuilder AppendAttributeDefinition(string attributeDefinition)
    {
        _sb.AppendLine(attributeDefinition);
        _sb.AppendLine();

        return this;
    }
}