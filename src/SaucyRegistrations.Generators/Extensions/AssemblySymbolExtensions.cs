using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using Saucy.Common.Attributes;
using Saucy.Common.Enums;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// The extensions for the <see cref="IAssemblySymbol" /> type.
/// </summary>
/// <seealso cref="IAssemblySymbol" />
internal static class AssemblySymbolExtensions
{
    /// <summary>
    /// Gets the namespaces to exclude from the registration when scanning the assembly.
    /// </summary>
    /// <param name="assemblySymbol">The <see cref="IAssemblySymbol" /> to get the excluded namespaces from.</param>
    /// <returns>A <see cref="List{T}" /> of <see cref="string" />.</returns>
    internal static List<string> GetExcludedNamespaces(this IAssemblySymbol assemblySymbol)
    {
        return assemblySymbol.GetListOfStringsFromAttributeOnSymbol(
            nameof(WhenRegisteringExcludeClassesInNamespaceAttribute), nameof(WhenRegisteringExcludeClassesInNamespaceAttribute.Namespace)
        );
    }

    internal static bool ShouldIncludeMicrosoftNamespaces(this IAssemblySymbol assemblySymbol)
    {
        return assemblySymbol.GetFirstAttributeWithNameOrNull(nameof(WhenRegisteringShouldIncludeMicrosoftNamespaces)) is not null;
    }

    internal static bool ShouldIncludeSystemNamespaces(this IAssemblySymbol assemblySymbol)
    {
        return assemblySymbol.GetFirstAttributeWithNameOrNull(nameof(WhenRegisteringShouldIncludeSystemNamespaces)) is not null;
    }

    internal static ServiceScope GetDefaultServiceScope(this IAssemblySymbol assemblySymbol)
    {
        AttributeData saucyTargetAttribute = assemblySymbol.GetFirstAttributeWithName(nameof(IncludeInSourceGenerationRegistrationWithDefaultServiceScopeAttribute));

        return saucyTargetAttribute.GetValueForPropertyOfType<ServiceScope>(nameof(IncludeInSourceGenerationRegistrationWithDefaultServiceScopeAttribute.DefaultServiceScope));
    }
}