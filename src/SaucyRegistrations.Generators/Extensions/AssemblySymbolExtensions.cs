using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Saucy.Common.Attributes;
using Saucy.Common.Enums;

namespace SaucyRegistrations.Generators.Extensions;

public static class AssemblySymbolExtensions
{
	internal static List<string> GetExcludedNamespaces(this IAssemblySymbol assemblySymbol)
		=> assemblySymbol.GetListOfStringsFromAttributeOnSymbol(
			nameof(WhenRegisteringExcludeClassesInNamespaceAttribute), nameof(WhenRegisteringExcludeClassesInNamespaceAttribute.Namespace)
		);

	internal static bool ShouldIncludeMicrosoftNamespaces(this IAssemblySymbol assemblySymbol)
		=> assemblySymbol.GetFirstAttributeWithNameOrNull(nameof(WhenRegisteringShouldIncludeMicrosoftNamespaces)) is not null;

	internal static bool ShouldIncludeSystemNamespaces(this IAssemblySymbol assemblySymbol)
		=> assemblySymbol.GetFirstAttributeWithNameOrNull(nameof(WhenRegisteringShouldIncludeSystemNamespaces)) is not null;
	
	internal static ServiceScope GetDefaultServiceScope(this IAssemblySymbol assemblySymbol)
	{
		AttributeData saucyTargetAttribute = assemblySymbol.GetFirstAttributeWithName(nameof(IncludeInSourceGenerationRegistrationWithDefaultServiceScopeAttribute));

		return saucyTargetAttribute.GetValueForPropertyOfType<ServiceScope>(nameof(IncludeInSourceGenerationRegistrationWithDefaultServiceScopeAttribute.DefaultServiceScope));
	}

	internal static bool ShouldBeIncludedInSourceGeneration(this ISymbol symbol)
		=> symbol.GetFirstAttributeWithNameOrNull(nameof(IncludeInSourceGenerationRegistrationWithDefaultServiceScopeAttribute)) is not null;
}
