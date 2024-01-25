using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Saucy.Common.Attributes;
using Saucy.Common.Enums;

namespace SaucyRegistrations.Generators.Extensions;

public static class AssemblySymbolExtensions
{
	internal static List<string> GetExcludedNamespaces(this IAssemblySymbol assemblySymbol)
		=> assemblySymbol.GetListOfStringsFromAttributeOnSymbol(
			nameof(SaucyExcludeNamespace), nameof(SaucyExcludeNamespace.Namespace)
		);

	internal static bool ShouldIncludeMicrosoftNamespaces(this IAssemblySymbol assemblySymbol)
		=> assemblySymbol.GetFirstAttributeWithNameOrNull(nameof(SaucyIncludeMicrosoftNamespaces)) is not null;

	internal static bool ShouldIncludeSystemNamespaces(this IAssemblySymbol assemblySymbol)
		=> assemblySymbol.GetFirstAttributeWithNameOrNull(nameof(SaucyIncludeSystemNamespaces)) is not null;
	
	internal static ServiceScope GetDefaultServiceScope(this IAssemblySymbol assemblySymbol)
	{
		AttributeData saucyTargetAttribute = assemblySymbol.GetFirstAttributeWithName(nameof(IncludeInSourceGeneration));

		return saucyTargetAttribute.GetParameter<ServiceScope>(nameof(IncludeInSourceGeneration.DefaultServiceScope));
	}

	internal static bool HasSaucyIncludeAttribute(this ISymbol symbol)
		=> symbol.GetFirstAttributeWithNameOrNull(nameof(IncludeInSourceGeneration)) is not null;
}
