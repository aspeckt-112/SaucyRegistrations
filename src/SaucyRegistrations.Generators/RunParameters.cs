using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Saucy.Common.Enums;

namespace SaucyRegistrations.Generators;

public class RunParameters(string @namespace, string partialClass, string generationMethod)
{
	public string Namespace { get; private set; } = @namespace;

	public string PartialClass { get; private set; } = partialClass;

	public string GenerationMethod { get; private set; } = generationMethod;

	public List<(ITypeSymbol typeSymbol, ServiceScope typeScope)> Types { get; private set; } = new();
}
