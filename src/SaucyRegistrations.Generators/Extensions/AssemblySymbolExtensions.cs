using System.Collections.Generic;
using System.Linq;

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
    /// <param name="assembly">The <see cref="IAssemblySymbol" /> to get the excluded namespaces from.</param>
    /// <returns>A <see cref="List{T}" /> of <see cref="string" />.</returns>
    internal static List<string> GetExcludedNamespaces(this IAssemblySymbol assembly)
    {
        var result = new List<string>();

        var attributes = assembly.GetAttributesOfType<WhenRegisteringExcludeClassesInNamespaceAttribute>();

        if (!attributes.Any())
        {
            return result;
        }

        foreach (AttributeData attribute in attributes)
        {
            var excludedNamespace = attribute.GetValueForPropertyOfType<string>(nameof(WhenRegisteringExcludeClassesInNamespaceAttribute.Namespace));
            result.Add(excludedNamespace);
        }

        return result;
    }

    internal static bool ShouldIncludeMicrosoftNamespaces(this IAssemblySymbol assembly)
    {
        return assembly.HasAttributeOfType<WhenRegisteringShouldIncludeMicrosoftNamespacesAttribute>();
    }

    internal static bool ShouldIncludeSystemNamespaces(this IAssemblySymbol assembly)
    {
        return assembly.HasAttributeOfType<WhenRegisteringShouldIncludeSystemNamespacesAttribute>();
    }
}