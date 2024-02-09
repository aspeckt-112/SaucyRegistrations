using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Saucy.Common.Enums;

namespace SaucyRegistrations.Generators.Configurations;

public class RunConfiguration(GenerationConfiguration generationConfiguration, Dictionary<ITypeSymbol, ServiceScope> typesToRegister)
{
	public GenerationConfiguration GenerationConfiguration { get; } = generationConfiguration;

	public Dictionary<ITypeSymbol, ServiceScope>  TypesToRegister { get; } = typesToRegister;
}
