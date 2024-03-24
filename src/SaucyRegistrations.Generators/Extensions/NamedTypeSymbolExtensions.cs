using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace SaucyRegistrations.Generators.Extensions;

/// <summary>
/// Extension methods for the <see cref="INamedTypeSymbol" /> class.
/// </summary>
internal static class NamedTypeSymbolExtensions
{
    /// <summary>
    /// Gets the fully qualified name of the named type symbol.
    /// </summary>
    /// <param name="namedTypeSymbol">The named type symbol.</param>
    /// <returns>The fully qualified name of the named type symbol.</returns>
    internal static string GetFullyQualifiedName(this INamedTypeSymbol namedTypeSymbol)
    {
        var fullyQualifiedName = namedTypeSymbol.ContainingNamespace.ToDisplayString();

        if (!string.IsNullOrWhiteSpace(fullyQualifiedName))
        {
            fullyQualifiedName += ".";
        }

        fullyQualifiedName += namedTypeSymbol.Name;

        return fullyQualifiedName;
    }

    /// <summary>
    /// Gets the contracts for the named type symbol.
    /// </summary>
    /// <param name="namedTypeSymbol">The named type symbol.</param>
    /// <returns>The contracts for the named type symbol.</returns>
    internal static List<string> GetContracts(this INamedTypeSymbol namedTypeSymbol)
    {
        var contractNames = new List<string>();

        foreach (INamedTypeSymbol? interfaceType in namedTypeSymbol.Interfaces)
        {
            contractNames.Add(interfaceType.GetFullyQualifiedName());
        }

        var hasAbstractBaseClass = namedTypeSymbol.BaseType is { IsAbstract: true };

        if (hasAbstractBaseClass && namedTypeSymbol.BaseType is not null)
        {
            contractNames.Add(namedTypeSymbol.BaseType.GetFullyQualifiedName());
        }

        return contractNames;
    }
}