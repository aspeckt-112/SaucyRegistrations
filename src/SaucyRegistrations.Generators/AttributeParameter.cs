using Microsoft.CodeAnalysis;

namespace SaucyRegistrations.Generators;

public class AttributeParameter(string name, ITypeSymbol type, object? value)
{
	public string Name { get; private set; } = name;
	

	public ITypeSymbol Type { get; private set; } = type;

	public object? Value { get; private set; } = value;
}
