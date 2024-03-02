namespace SaucyRegistrations.Generators.Models;

/// <summary>
/// Represents a parameter of an attribute.
/// </summary>
/// <param name="name">The name of the parameter.</param>
/// <param name="value">The value of the parameter.</param>
internal sealed class AttributeParameter(string name, object? value)
{
    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the value of the parameter.
    /// </summary>
    public object? Value { get; } = value;
}